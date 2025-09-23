import { Component, OnInit, OnDestroy, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { Subject, takeUntil, forkJoin, of, debounceTime, distinctUntilChanged } from 'rxjs';
import { catchError, finalize, switchMap } from 'rxjs/operators';
import { AppStateService } from '../shared/app-state/app-state.service';
import { DataService } from '../shared/services/data.service';
import { TranslationService } from '../shared/services/translation.service';

import { TranslatePipe } from '../shared/pipes/translate.pipe';
import { 
  Institution, 
  Analysis, 
  Instrument, 
  Keyword, 
  Employee, 
  Microorganism,
  FilterState 
} from '../shared/models/data-models';

interface SearchResult {
  type: 'institution' | 'analysis' | 'instrument' | 'keyword' | 'researcher' | 'microorganism';
  category: string;
  items: any[];
}

interface FilteredResults {
  institutions: Institution[];
  analyses: Analysis[];
  instruments: Instrument[];
  keywords: Keyword[];
  researchers: Employee[];
  microorganisms: Microorganism[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  userName: string = '';
  currentDate: Date = new Date();
  currentLanguage: 'en' | 'sr' = 'en';
  
  isDarkTheme: boolean = false;

  activeTab: 'analyses' | 'researchers' = 'analyses';

  masterSearchTerm: string = '';
  showSearchSuggestions: boolean = false;
  filteredSearchResults: SearchResult[] = [];

  selectedInstitution: number | null = null;
  selectedAnalysis: number | null = null;
  selectedMicroorganism: number | null = null;
  selectedInstrument: number | null = null;
  selectedResearcher: number | null = null;
  selectedKeyword: number | null = null;

  filteredInstitutions: Institution[] = [];
  filteredAnalyses: Analysis[] = [];
  filteredMicroorganisms: Microorganism[] = [];
  filteredInstruments: Instrument[] = [];
  filteredResearchers: Employee[] = [];
  filteredKeywords: Keyword[] = [];

  selectedInstitutionDetails: Institution | null = null;
  selectedAnalysisDetails: Analysis | null = null;
  selectedResearcherDetails: Employee | null = null;

  pinnedCategory: string | null = null;

  filteredResults: FilteredResults = {
    institutions: [],
    analyses: [],
    instruments: [],
    keywords: [],
    researchers: [],
    microorganisms: []
  };

  showModal: boolean = false;
  modalTitle: string = '';
  modalType: string = '';
  selectedItem: any = null;

  allInstitutions: Institution[] = [];
  allAnalyses: Analysis[] = [];
  allMicroorganisms: Microorganism[] = [];
  allInstruments: Instrument[] = [];
  allResearchers: Employee[] = [];
  allKeywords: Keyword[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private appStateService: AppStateService,
    private dataService: DataService,
    private translationService: TranslationService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadInitialData();
    this.setupSearchDebouncing();
    if (isPlatformBrowser(this.platformId)) {
      this.loadThemePreference();
      this.applyTheme();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadUserInfo(): void {
    this.appStateService.getAppState()
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => {
        this.userName = state.firstName && state.lastName 
          ? `${state.firstName} ${state.lastName}` 
          : state.username || 'User';
      });
  }

  private loadInitialData(): void {
    forkJoin({
      institutions: this.dataService.getAllInstitutionsWithRelatedData().pipe(catchError(() => of([]))),
      analyses: this.dataService.getAllAnalysesWithRelatedData().pipe(catchError(() => of([]))),
      microorganisms: this.dataService.getAllMicroorganismsWithRelatedData().pipe(catchError(() => of([]))),
      instruments: this.dataService.getAllInstrumentsWithRelatedData().pipe(catchError(() => of([]))),
      researchers: this.dataService.getAllEmployeesWithRelatedData().pipe(catchError(() => of([]))),
      keywords: this.dataService.getAllKeywordsWithRelatedData().pipe(catchError(() => of([])))
    })
    .pipe(takeUntil(this.destroy$))
    .subscribe(data => {
      this.allInstitutions = data.institutions;
      this.allAnalyses = data.analyses;
      this.allMicroorganisms = data.microorganisms;
      this.allInstruments = data.instruments;
      this.allResearchers = data.researchers;
      this.allKeywords = data.keywords;


      // Initialize filtered lists
      this.filteredInstitutions = [...this.allInstitutions];
      this.filteredAnalyses = [...this.allAnalyses];
      this.filteredMicroorganisms = [...this.allMicroorganisms];
      this.filteredInstruments = [...this.allInstruments];
      this.filteredResearchers = [...this.allResearchers];
      this.filteredKeywords = [...this.allKeywords];
    });
  }

  private setupSearchDebouncing(): void {
  }

  setActiveTab(tab: 'analyses' | 'researchers'): void {
    this.activeTab = tab;
    this.updateResults();
  }
  onSearchBlur(): void {
    setTimeout(() => {
      this.showSearchSuggestions = false;
    }, 200);
  }

  onMasterSearchInput(event: any): void {
    const searchTerm = event.target.value.toLowerCase();
    if (searchTerm.length < 2) {
      this.filteredSearchResults = [];
      return;
    }


    // Helper function to get selected item by ID
    const getSelectedItem = (id: number | null, items: any[]): any => {
      return id ? items.find(item => item.id === id) : null;
    };

    // Get selected items
    const selectedInstitution = getSelectedItem(this.selectedInstitution, this.allInstitutions);
    const selectedAnalysis = getSelectedItem(this.selectedAnalysis, this.allAnalyses);
    const selectedResearcher = getSelectedItem(this.selectedResearcher, this.allResearchers);
    const selectedKeyword = getSelectedItem(this.selectedKeyword, this.allKeywords);
    const selectedInstrument = getSelectedItem(this.selectedInstrument, this.allInstruments);
    const selectedMicroorganism = getSelectedItem(this.selectedMicroorganism, this.allMicroorganisms);

    // Helper function to filter items and include selected item if it matches
    const filterAndIncludeSelected = (items: any[], selectedItem: any, searchFn: (item: any) => boolean): any[] => {
      const filtered = items.filter(searchFn);
      
      // If there's a selected item and it matches the search term, add it to the results
      if (selectedItem && searchFn(selectedItem)) {
        // Check if selected item is already in the filtered results
        const isAlreadyIncluded = filtered.some(item => item.id === selectedItem.id);
        if (!isAlreadyIncluded) {
          filtered.unshift(selectedItem); // Add to beginning to highlight it
        }
      }
      
      return filtered.slice(0, 5);
    };

    this.filteredSearchResults = [
      {
        type: 'institution' as const,
        category: 'INSTITUTIONS',
        items: filterAndIncludeSelected(
          this.allInstitutions, 
          selectedInstitution,
          inst => inst.name.toLowerCase().includes(searchTerm) ||
                  (inst.address && inst.address.toLowerCase().includes(searchTerm))
        )
      },
      {
        type: 'analysis' as const,
        category: 'ANALYSES',
        items: filterAndIncludeSelected(
          this.allAnalyses, 
          selectedAnalysis,
          analysis => analysis.name.toLowerCase().includes(searchTerm)
        )
      },
      {
        type: 'researcher' as const,
        category: 'RESEARCHERS',
        items: filterAndIncludeSelected(
          this.allResearchers, 
          selectedResearcher,
          researcher => `${researcher.firstName} ${researcher.lastName}`.toLowerCase().includes(searchTerm)
        )
      },
      {
        type: 'keyword' as const,
        category: 'KEYWORDS',
        items: filterAndIncludeSelected(
          this.allKeywords, 
          selectedKeyword,
          keyword => keyword.name.toLowerCase().includes(searchTerm)
        )
      },
      {
        type: 'instrument' as const,
        category: 'INSTRUMENTS',
        items: filterAndIncludeSelected(
          this.allInstruments, 
          selectedInstrument,
          instrument => instrument.name.toLowerCase().includes(searchTerm) ||
                       (instrument.description && instrument.description.toLowerCase().includes(searchTerm)) ||
                       (instrument.manufacturer && instrument.manufacturer.toLowerCase().includes(searchTerm)) ||
                       (instrument.model && instrument.model.toLowerCase().includes(searchTerm))
        )
      },
      {
        type: 'microorganism' as const,
        category: 'MICROORGANISMS',
        items: filterAndIncludeSelected(
          this.allMicroorganisms, 
          selectedMicroorganism,
          micro => micro.name.toLowerCase().includes(searchTerm)
        )
      }
    ].filter(group => group.items.length > 0);
  }

  onSearchSuggestionClick(item: any, type: string): void {
    switch (type) {
      case 'institution':
        this.selectedInstitution = item.id;
        this.onInstitutionChange();
        break;
      case 'analysis':
        this.selectedAnalysis = item.id;
        this.onAnalysisChange();
        break;
      case 'researcher':
        this.selectedResearcher = item.id;
        this.onResearcherChange();
        break;
      case 'keyword':
        this.selectedKeyword = item.id;
        this.onKeywordChange();
        break;
      case 'instrument':
        this.selectedInstrument = item.id;
        this.onInstrumentChange();
        break;
      case 'microorganism':
        this.selectedMicroorganism = item.id;
        this.onMicroorganismChange();
        break;
    }
    this.masterSearchTerm = '';
    this.filteredSearchResults = [];
    this.showSearchSuggestions = false;
  }

  getCategoryIcon(type: string): string {
    const icons: { [key: string]: string } = {
      institution: 'fas fa-hospital',
      analysis: 'fas fa-flask',
      researcher: 'fas fa-user-md',
      keyword: 'fas fa-tags',
      instrument: 'fas fa-microscope',
      microorganism: 'fas fa-bug'
    };
    return icons[type] || 'fas fa-question';
  }

  isItemSelected(item: any, type: string): boolean {
    switch (type) {
      case 'institution':
        return this.selectedInstitution === item.id;
      case 'analysis':
        return this.selectedAnalysis === item.id;
      case 'researcher':
        return this.selectedResearcher === item.id;
      case 'keyword':
        return this.selectedKeyword === item.id;
      case 'instrument':
        return this.selectedInstrument === item.id;
      case 'microorganism':
        return this.selectedMicroorganism === item.id;
      default:
        return false;
    }
  }

  pinCategory(category: string): void {
    if (this.pinnedCategory === category) {
      this.pinnedCategory = null;
    } else {
      this.pinnedCategory = category;
    }
    this.updateFilteredOptions();
    this.updateResults();
  }
  onInstitutionChange(): void {
    const institution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
    this.selectedInstitutionDetails = institution || null;
    
    if (institution?.name.includes('Tech')) {
      console.log('Selected Tech University:', {
        id: institution.id,
        name: institution.name,
        analyses: institution.analyses?.map(a => ({ id: a.id, name: a.name })),
        instruments: institution.instruments?.map(i => ({ id: i.id, name: i.name })),
        microorganisms: institution.microorganisms?.map(m => ({ id: m.id, name: m.name }))
      });
    }
    
    this.updateFilteredOptions();
    this.updateResults();
  }

  onAnalysisChange(): void {
    const analysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
    this.selectedAnalysisDetails = analysis || null;
    this.updateFilteredOptions();
    this.updateResults();
  }

  onMicroorganismChange(): void {
    this.updateFilteredOptions();
    this.updateResults();
  }

  onInstrumentChange(): void {
    this.updateFilteredOptions();
    this.updateResults();
  }

  onResearcherChange(): void {
    const researcher = this.allResearchers.find(res => res.id === this.selectedResearcher);
    this.selectedResearcherDetails = researcher || null;
    this.updateFilteredOptions();
    this.updateResults();
  }

  onKeywordChange(): void {
    this.updateFilteredOptions();
    this.updateResults();
  }

  private updateFilteredOptions(): void {
    // Keep the original simple implementation
    if (this.pinnedCategory === 'institution' && this.selectedInstitution) {
      const institution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
      if (institution) {
        // Could add logic here if needed for pinned categories
      }
    } else if (this.pinnedCategory === 'analysis' && this.selectedAnalysis) {
      const analysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
      if (analysis) {
        // Could add logic here if needed for pinned categories
      }
    }
  }


  private updateResults(): void {
    this.filteredResults = {
      institutions: this.getFilteredInstitutions(),
      analyses: this.getFilteredAnalyses(),
      instruments: this.getFilteredInstruments(),
      keywords: this.getFilteredKeywords(),
      researchers: this.getFilteredResearchers(),
      microorganisms: this.getFilteredMicroorganisms()
    };

    if (this.hasActiveFilters()) {
      this.addRelatedEntities();
    }
  }

  private addRelatedEntities(): void {
    // When filtering by institution, we don't want to add other selected entities
    // because we want to show only entities that belong to the selected institution
    if (this.selectedInstitution) {
      // Only add the selected institution itself
      const institutionId = typeof this.selectedInstitution === 'string' ? parseInt(this.selectedInstitution) : this.selectedInstitution;
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      if (selectedInstitution && !this.filteredResults.institutions.some(inst => inst.id === institutionId)) {
        this.filteredResults.institutions = [selectedInstitution, ...this.filteredResults.institutions];
      }
      return; // Don't add other selected entities when filtering by institution
    }

    // For other types of filters, add the selected entities
    if (this.selectedAnalysis) {
      const analysisId = typeof this.selectedAnalysis === 'string' ? parseInt(this.selectedAnalysis) : this.selectedAnalysis;
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      if (selectedAnalysis && !this.filteredResults.analyses.some(anal => anal.id === analysisId)) {
        this.filteredResults.analyses = [selectedAnalysis, ...this.filteredResults.analyses];
      }
    }

    if (this.selectedMicroorganism) {
      const microorganismId = typeof this.selectedMicroorganism === 'string' ? parseInt(this.selectedMicroorganism) : this.selectedMicroorganism;
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === microorganismId);
      if (selectedMicroorganism && !this.filteredResults.microorganisms.some(micro => micro.id === microorganismId)) {
        this.filteredResults.microorganisms = [selectedMicroorganism, ...this.filteredResults.microorganisms];
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = typeof this.selectedInstrument === 'string' ? parseInt(this.selectedInstrument) : this.selectedInstrument;
      const selectedInstrument = this.allInstruments.find(inst => inst.id === instrumentId);
      if (selectedInstrument && !this.filteredResults.instruments.some(inst => inst.id === instrumentId)) {
        this.filteredResults.instruments = [selectedInstrument, ...this.filteredResults.instruments];
      }
    }

    if (this.selectedResearcher) {
      const researcherId = typeof this.selectedResearcher === 'string' ? parseInt(this.selectedResearcher) : this.selectedResearcher;
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      if (selectedResearcher && !this.filteredResults.researchers.some(res => res.id === researcherId)) {
        this.filteredResults.researchers = [selectedResearcher, ...this.filteredResults.researchers];
      }
    }

    if (this.selectedKeyword) {
      const keywordId = typeof this.selectedKeyword === 'string' ? parseInt(this.selectedKeyword) : this.selectedKeyword;
      const selectedKeyword = this.allKeywords.find(kw => kw.id === keywordId);
      if (selectedKeyword && !this.filteredResults.keywords.some(kw => kw.id === keywordId)) {
        this.filteredResults.keywords = [selectedKeyword, ...this.filteredResults.keywords];
      }
    }
  }

  private getFilteredInstitutions(): Institution[] {
    let institutions = [...this.allInstitutions];

    if (!this.hasActiveFilters()) {
      return institutions;
    }

    // Apply filters based on selected dropdown items
    // Show all institutions that match the selected criteria
    if (this.selectedInstitution) {
      const institutionId = typeof this.selectedInstitution === 'string' ? parseInt(this.selectedInstitution) : this.selectedInstitution;
      institutions = institutions.filter(inst => inst.id === institutionId);
    }

    if (this.selectedAnalysis) {
      const analysisId = typeof this.selectedAnalysis === 'string' ? parseInt(this.selectedAnalysis) : this.selectedAnalysis;
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      if (selectedAnalysis?.institutions) {
        const analysisInstitutionIds = selectedAnalysis.institutions.map(inst => inst.id);
        institutions = institutions.filter(inst => analysisInstitutionIds.includes(inst.id));
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = typeof this.selectedInstrument === 'string' ? parseInt(this.selectedInstrument) : this.selectedInstrument;
      const selectedInstrument = this.allInstruments.find(inst => inst.id === instrumentId);
      if (selectedInstrument) {
        institutions = institutions.filter(inst => 
          inst.instruments?.some(instrument => instrument.id === instrumentId)
        );
      }
    }

    if (this.selectedMicroorganism) {
      const microorganismId = typeof this.selectedMicroorganism === 'string' ? parseInt(this.selectedMicroorganism) : this.selectedMicroorganism;
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === microorganismId);
      if (selectedMicroorganism) {
        institutions = institutions.filter(inst => 
          inst.microorganisms?.some(micro => micro.id === microorganismId)
        );
      }
    }

    if (this.selectedResearcher) {
      const researcherId = typeof this.selectedResearcher === 'string' ? parseInt(this.selectedResearcher) : this.selectedResearcher;
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      if (selectedResearcher?.institution) {
        institutions = institutions.filter(inst => inst.id === selectedResearcher.institution?.id);
      }
    }

    if (this.selectedKeyword) {
      const keywordId = typeof this.selectedKeyword === 'string' ? parseInt(this.selectedKeyword) : this.selectedKeyword;
      const selectedKeyword = this.allKeywords.find(kw => kw.id === keywordId);
      if (selectedKeyword) {
        const keywordResearcherIds = selectedKeyword.researchers?.map(res => res.id) || [];
        const keywordInstitutionIds = this.allResearchers
          .filter(res => keywordResearcherIds.includes(res.id))
          .map(res => res.institution?.id)
          .filter(id => id !== undefined);
        institutions = institutions.filter(inst => keywordInstitutionIds.includes(inst.id));
      }
    }

    return institutions;
  }

  private getFilteredAnalyses(): Analysis[] {
    let analyses = [...this.allAnalyses];

    if (!this.hasActiveFilters()) {
      return analyses;
    }

    // When institution is selected, ONLY use institution filter and ignore all other filters
    if (this.selectedInstitution) {
      const institutionId = typeof this.selectedInstitution === 'string' ? parseInt(this.selectedInstitution) : this.selectedInstitution;
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      if (selectedInstitution) {
        if (selectedInstitution.analyses && selectedInstitution.analyses.length > 0) {
          // Show only analyses that are directly associated with the selected institution
          const institutionAnalysisIds = selectedInstitution.analyses.map(a => a.id);
          analyses = analyses.filter(analysis => institutionAnalysisIds.includes(analysis.id));
        } else {
          analyses = [];
        }
        
        return analyses; // Return early, don't apply other filters
      }
    }

    // Apply other filters only if institution is not selected
    if (this.selectedAnalysis) {
      const analysisId = typeof this.selectedAnalysis === 'string' ? parseInt(this.selectedAnalysis) : this.selectedAnalysis;
      analyses = analyses.filter(anal => anal.id === analysisId);
    }

    if (this.selectedMicroorganism) {
      const microorganismId = typeof this.selectedMicroorganism === 'string' ? parseInt(this.selectedMicroorganism) : this.selectedMicroorganism;
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === microorganismId);
      if (selectedMicroorganism) {
        analyses = analyses.filter(analysis => 
          analysis.microorganisms?.some(micro => micro.id === microorganismId)
        );
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = typeof this.selectedInstrument === 'string' ? parseInt(this.selectedInstrument) : this.selectedInstrument;
      const selectedInstrument = this.allInstruments.find(inst => inst.id === instrumentId);
      if (selectedInstrument) {
        const instrumentInstitutionIds = this.allInstitutions
          .filter(inst => inst.instruments?.some(instrument => instrument.id === instrumentId))
          .map(inst => inst.id);
        analyses = analyses.filter(analysis => 
          analysis.institutions?.some(inst => instrumentInstitutionIds.includes(inst.id))
        );
      }
    }

    if (this.selectedResearcher) {
      const researcherId = typeof this.selectedResearcher === 'string' ? parseInt(this.selectedResearcher) : this.selectedResearcher;
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      if (selectedResearcher?.institution) {
        analyses = analyses.filter(analysis => 
          analysis.institutions?.some(inst => inst.id === selectedResearcher.institution?.id)
        );
      }
    }

    if (this.selectedKeyword) {
      const keywordId = typeof this.selectedKeyword === 'string' ? parseInt(this.selectedKeyword) : this.selectedKeyword;
      const selectedKeyword = this.allKeywords.find(kw => kw.id === keywordId);
      if (selectedKeyword) {
        const keywordResearcherIds = selectedKeyword.researchers?.map(res => res.id) || [];
        const keywordInstitutionIds = this.allResearchers
          .filter(res => keywordResearcherIds.includes(res.id))
          .map(res => res.institution?.id)
          .filter(id => id !== undefined);
        analyses = analyses.filter(analysis => 
          analysis.institutions?.some(inst => keywordInstitutionIds.includes(inst.id))
        );
      }
    }

    return analyses;
  }

  private getFilteredInstruments(): Instrument[] {
    let instruments = [...this.allInstruments];

    if (!this.hasActiveFilters()) {
      return instruments;
    }

    // When institution is selected, ONLY use institution filter and ignore all other filters
    if (this.selectedInstitution) {
      const institutionId = typeof this.selectedInstitution === 'string' ? parseInt(this.selectedInstitution) : this.selectedInstitution;
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      if (selectedInstitution) {
        if (selectedInstitution.instruments && selectedInstitution.instruments.length > 0) {
          const institutionInstrumentIds = selectedInstitution.instruments.map(inst => inst.id);
          instruments = instruments.filter(inst => institutionInstrumentIds.includes(inst.id));
        } else {
          instruments = [];
        }
        
        return instruments; // Return early, don't apply other filters
      }
    }

    // Apply other filters only if institution is not selected
    if (this.selectedInstrument) {
      const instrumentId = typeof this.selectedInstrument === 'string' ? parseInt(this.selectedInstrument) : this.selectedInstrument;
      instruments = instruments.filter(inst => inst.id === instrumentId);
    }

    if (this.selectedAnalysis) {
      const analysisId = typeof this.selectedAnalysis === 'string' ? parseInt(this.selectedAnalysis) : this.selectedAnalysis;
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      if (selectedAnalysis?.institutions) {
        const analysisInstitutionIds = selectedAnalysis.institutions.map(inst => inst.id);
        const analysisInstitutionInstruments = this.allInstitutions
          .filter(inst => analysisInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.instruments || []);
        const analysisInstrumentIds = analysisInstitutionInstruments.map(inst => inst.id);
        instruments = instruments.filter(inst => analysisInstrumentIds.includes(inst.id));
      }
    }

    if (this.selectedMicroorganism) {
      const microorganismId = typeof this.selectedMicroorganism === 'string' ? parseInt(this.selectedMicroorganism) : this.selectedMicroorganism;
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === microorganismId);
      if (selectedMicroorganism) {
        const microorganismInstitutionIds = this.allInstitutions
          .filter(inst => inst.microorganisms?.some(micro => micro.id === microorganismId))
          .map(inst => inst.id);
        const microorganismInstitutionInstruments = this.allInstitutions
          .filter(inst => microorganismInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.instruments || []);
        const microorganismInstrumentIds = microorganismInstitutionInstruments.map(inst => inst.id);
        instruments = instruments.filter(inst => microorganismInstrumentIds.includes(inst.id));
      }
    }

    if (this.selectedResearcher) {
      const researcherId = typeof this.selectedResearcher === 'string' ? parseInt(this.selectedResearcher) : this.selectedResearcher;
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      if (selectedResearcher?.institution) {
        const researcherInstitution = this.allInstitutions.find(inst => inst.id === selectedResearcher.institution?.id);
        if (researcherInstitution?.instruments) {
          const researcherInstrumentIds = researcherInstitution.instruments.map(inst => inst.id);
          instruments = instruments.filter(inst => researcherInstrumentIds.includes(inst.id));
        }
      }
    }

    if (this.selectedKeyword) {
      const keywordId = typeof this.selectedKeyword === 'string' ? parseInt(this.selectedKeyword) : this.selectedKeyword;
      const selectedKeyword = this.allKeywords.find(kw => kw.id === keywordId);
      if (selectedKeyword) {
        const keywordResearcherIds = selectedKeyword.researchers?.map(res => res.id) || [];
        const keywordInstitutionIds = this.allResearchers
          .filter(res => keywordResearcherIds.includes(res.id))
          .map(res => res.institution?.id)
          .filter(id => id !== undefined);
        const keywordInstitutionInstruments = this.allInstitutions
          .filter(inst => keywordInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.instruments || []);
        const keywordInstrumentIds = keywordInstitutionInstruments.map(inst => inst.id);
        instruments = instruments.filter(inst => keywordInstrumentIds.includes(inst.id));
      }
    }

    return instruments;
  }

  private getFilteredKeywords(): Keyword[] {
    let keywords = [...this.allKeywords];

    if (!this.hasActiveFilters()) {
      return keywords;
    }

    if (this.selectedKeyword) {
      const keywordId = typeof this.selectedKeyword === 'string' ? parseInt(this.selectedKeyword) : this.selectedKeyword;
      keywords = keywords.filter(kw => kw.id === keywordId);
    }

    if (this.selectedResearcher) {
      const researcherId = typeof this.selectedResearcher === 'string' ? parseInt(this.selectedResearcher) : this.selectedResearcher;
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      if (selectedResearcher?.keywords && selectedResearcher.keywords.length > 0) {
        // Show keywords that are directly associated with this specific researcher
        const researcherKeywordIds = selectedResearcher.keywords.map(kw => kw.id);
        keywords = keywords.filter(kw => researcherKeywordIds.includes(kw.id));
      }
    }

    if (this.selectedInstitution) {
      const institutionId = typeof this.selectedInstitution === 'string' ? parseInt(this.selectedInstitution) : this.selectedInstitution;
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      if (selectedInstitution) {
        if (selectedInstitution.keywords && selectedInstitution.keywords.length > 0) {
          // Use keywords directly associated with the institution
          const institutionKeywordIds = selectedInstitution.keywords.map(kw => kw.id);
          keywords = keywords.filter(kw => institutionKeywordIds.includes(kw.id));
        } else {
          // Fallback: find keywords through researchers at this institution
          const institutionResearcherIds = this.allResearchers
            .filter(res => res.institution?.id === institutionId)
            .map(res => res.id);
          const institutionKeywordIds = this.allKeywords
            .filter(kw => kw.researchers?.some(res => institutionResearcherIds.includes(res.id)))
            .map(kw => kw.id);
          keywords = keywords.filter(kw => institutionKeywordIds.includes(kw.id));
        }
      }
    }

    if (this.selectedAnalysis) {
      const analysisId = typeof this.selectedAnalysis === 'string' ? parseInt(this.selectedAnalysis) : this.selectedAnalysis;
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      if (selectedAnalysis?.institutions) {
        const analysisInstitutionIds = selectedAnalysis.institutions.map(inst => inst.id);
        const analysisResearcherIds = this.allResearchers
          .filter(res => analysisInstitutionIds.includes(res.institution?.id || 0))
          .map(res => res.id);
        const analysisKeywordIds = this.allKeywords
          .filter(kw => kw.researchers?.some(res => analysisResearcherIds.includes(res.id)))
          .map(kw => kw.id);
        keywords = keywords.filter(kw => analysisKeywordIds.includes(kw.id));
      }
    }

    if (this.selectedMicroorganism) {
      const microorganismId = typeof this.selectedMicroorganism === 'string' ? parseInt(this.selectedMicroorganism) : this.selectedMicroorganism;
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === microorganismId);
      if (selectedMicroorganism) {
        const microorganismInstitutionIds = this.allInstitutions
          .filter(inst => inst.microorganisms?.some(micro => micro.id === microorganismId))
          .map(inst => inst.id);
        const microorganismResearcherIds = this.allResearchers
          .filter(res => microorganismInstitutionIds.includes(res.institution?.id || 0))
          .map(res => res.id);
        const microorganismKeywordIds = this.allKeywords
          .filter(kw => kw.researchers?.some(res => microorganismResearcherIds.includes(res.id)))
          .map(kw => kw.id);
        keywords = keywords.filter(kw => microorganismKeywordIds.includes(kw.id));
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = typeof this.selectedInstrument === 'string' ? parseInt(this.selectedInstrument) : this.selectedInstrument;
      const selectedInstrument = this.allInstruments.find(inst => inst.id === instrumentId);
      if (selectedInstrument) {
        const instrumentInstitutionIds = this.allInstitutions
          .filter(inst => inst.instruments?.some(instrument => instrument.id === instrumentId))
          .map(inst => inst.id);
        const instrumentResearcherIds = this.allResearchers
          .filter(res => instrumentInstitutionIds.includes(res.institution?.id || 0))
          .map(res => res.id);
        const instrumentKeywordIds = this.allKeywords
          .filter(kw => kw.researchers?.some(res => instrumentResearcherIds.includes(res.id)))
          .map(kw => kw.id);
        keywords = keywords.filter(kw => instrumentKeywordIds.includes(kw.id));
      }
    }

    return keywords;
  }

  private getFilteredResearchers(): Employee[] {
    let researchers = [...this.allResearchers];

    if (!this.hasActiveFilters()) {
      return researchers;
    }

    if (this.selectedResearcher) {
      const researcherId = typeof this.selectedResearcher === 'string' ? parseInt(this.selectedResearcher) : this.selectedResearcher;
      researchers = researchers.filter(res => res.id === researcherId);
    }

    if (this.selectedInstitution) {
      const institutionId = typeof this.selectedInstitution === 'string' ? parseInt(this.selectedInstitution) : this.selectedInstitution;
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      if (selectedInstitution) {
        if (selectedInstitution.employees && selectedInstitution.employees.length > 0) {
          // Use employees directly associated with the institution
          const institutionEmployeeIds = selectedInstitution.employees.map(emp => emp.id);
          researchers = researchers.filter(researcher => institutionEmployeeIds.includes(researcher.id));
        } else {
          // Fallback: filter by institution relationship
          researchers = researchers.filter(researcher => 
            researcher.institution?.id === institutionId
          );
        }
      }
    }

    if (this.selectedKeyword) {
      const keywordId = typeof this.selectedKeyword === 'string' ? parseInt(this.selectedKeyword) : this.selectedKeyword;
      const selectedKeyword = this.allKeywords.find(kw => kw.id === keywordId);
      if (selectedKeyword) {
        researchers = researchers.filter(researcher => 
          researcher.keywords?.some(kw => kw.id === keywordId)
        );
      }
    }

    if (this.selectedAnalysis) {
      const analysisId = typeof this.selectedAnalysis === 'string' ? parseInt(this.selectedAnalysis) : this.selectedAnalysis;
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      if (selectedAnalysis?.institutions) {
        const analysisInstitutionIds = selectedAnalysis.institutions.map(inst => inst.id);
        researchers = researchers.filter(researcher => 
          analysisInstitutionIds.includes(researcher.institution?.id || 0)
        );
      }
    }

    if (this.selectedMicroorganism) {
      const microorganismId = typeof this.selectedMicroorganism === 'string' ? parseInt(this.selectedMicroorganism) : this.selectedMicroorganism;
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === microorganismId);
      if (selectedMicroorganism) {
        const microorganismInstitutionIds = this.allInstitutions
          .filter(inst => inst.microorganisms?.some(micro => micro.id === microorganismId))
          .map(inst => inst.id);
        researchers = researchers.filter(researcher => 
          microorganismInstitutionIds.includes(researcher.institution?.id || 0)
        );
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = typeof this.selectedInstrument === 'string' ? parseInt(this.selectedInstrument) : this.selectedInstrument;
      const selectedInstrument = this.allInstruments.find(inst => inst.id === instrumentId);
      if (selectedInstrument) {
        const instrumentInstitutionIds = this.allInstitutions
          .filter(inst => inst.instruments?.some(instrument => instrument.id === instrumentId))
          .map(inst => inst.id);
        researchers = researchers.filter(researcher => 
          instrumentInstitutionIds.includes(researcher.institution?.id || 0)
        );
      }
    }

    return researchers;
  }

  private getFilteredMicroorganisms(): Microorganism[] {
    let microorganisms = [...this.allMicroorganisms];

    if (!this.hasActiveFilters()) {
      return microorganisms;
    }

    // When institution is selected, ONLY use institution filter and ignore all other filters
    if (this.selectedInstitution) {
      const institutionId = typeof this.selectedInstitution === 'string' ? parseInt(this.selectedInstitution) : this.selectedInstitution;
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      if (selectedInstitution) {
        if (selectedInstitution.microorganisms && selectedInstitution.microorganisms.length > 0) {
          const institutionMicroorganismIds = selectedInstitution.microorganisms.map(micro => micro.id);
          microorganisms = microorganisms.filter(micro => institutionMicroorganismIds.includes(micro.id));
        } else {
          microorganisms = [];
        }
        
        return microorganisms; // Return early, don't apply other filters
      }
    }

    // Apply other filters only if institution is not selected
    if (this.selectedMicroorganism) {
      const microorganismId = typeof this.selectedMicroorganism === 'string' ? parseInt(this.selectedMicroorganism) : this.selectedMicroorganism;
      microorganisms = microorganisms.filter(micro => micro.id === microorganismId);
    }

    if (this.selectedAnalysis) {
      const analysisId = typeof this.selectedAnalysis === 'string' ? parseInt(this.selectedAnalysis) : this.selectedAnalysis;
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      if (selectedAnalysis?.microorganisms) {
        const analysisMicroorganismIds = selectedAnalysis.microorganisms.map(micro => micro.id);
        microorganisms = microorganisms.filter(micro => analysisMicroorganismIds.includes(micro.id));
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = typeof this.selectedInstrument === 'string' ? parseInt(this.selectedInstrument) : this.selectedInstrument;
      const selectedInstrument = this.allInstruments.find(inst => inst.id === instrumentId);
      if (selectedInstrument) {
        const instrumentInstitutionIds = this.allInstitutions
          .filter(inst => inst.instruments?.some(instrument => instrument.id === instrumentId))
          .map(inst => inst.id);
        const instrumentInstitutionMicroorganismIds = this.allInstitutions
          .filter(inst => instrumentInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.microorganisms || [])
          .map(micro => micro.id);
        microorganisms = microorganisms.filter(micro => instrumentInstitutionMicroorganismIds.includes(micro.id));
      }
    }

    if (this.selectedResearcher) {
      const researcherId = typeof this.selectedResearcher === 'string' ? parseInt(this.selectedResearcher) : this.selectedResearcher;
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      if (selectedResearcher?.institution) {
        const researcherInstitution = this.allInstitutions.find(inst => inst.id === selectedResearcher.institution?.id);
        if (researcherInstitution?.microorganisms) {
          const researcherMicroorganismIds = researcherInstitution.microorganisms.map(micro => micro.id);
          microorganisms = microorganisms.filter(micro => researcherMicroorganismIds.includes(micro.id));
        }
      }
    }

    if (this.selectedKeyword) {
      const keywordId = typeof this.selectedKeyword === 'string' ? parseInt(this.selectedKeyword) : this.selectedKeyword;
      const selectedKeyword = this.allKeywords.find(kw => kw.id === keywordId);
      if (selectedKeyword) {
        const keywordResearcherIds = selectedKeyword.researchers?.map(res => res.id) || [];
        const keywordInstitutionIds = this.allResearchers
          .filter(res => keywordResearcherIds.includes(res.id))
          .map(res => res.institution?.id)
          .filter(id => id !== undefined);
        const keywordInstitutionMicroorganismIds = this.allInstitutions
          .filter(inst => keywordInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.microorganisms || [])
          .map(micro => micro.id);
        microorganisms = microorganisms.filter(micro => keywordInstitutionMicroorganismIds.includes(micro.id));
      }
    }

    return microorganisms;
  }

  hasActiveFilters(): boolean {
    return !!(this.selectedInstitution || this.selectedAnalysis || 
              this.selectedMicroorganism || this.selectedInstrument || 
              this.selectedResearcher || this.selectedKeyword);
  }


  clearAllFilters(): void {
    this.selectedInstitution = null;
    this.selectedAnalysis = null;
    this.selectedMicroorganism = null;
    this.selectedInstrument = null;
    this.selectedResearcher = null;
    this.selectedKeyword = null;
    this.selectedInstitutionDetails = null;
    this.selectedAnalysisDetails = null;
    this.selectedResearcherDetails = null;
    this.pinnedCategory = null;
    this.updateFilteredOptions();
    this.updateResults();
  }

  toggleLanguage(): void {
    this.currentLanguage = this.currentLanguage === 'en' ? 'sr' : 'en';
    this.translationService.setLanguage(this.currentLanguage);
  }
  showInstitutionDetails(institution: Institution): void {
    this.selectedItem = institution;
    this.modalTitle = institution.name;
    this.modalType = 'institution';
    this.showModal = true;
  }

  showResearcherDetails(researcher: Employee): void {
    this.selectedItem = researcher;
    this.modalTitle = `${researcher.firstName} ${researcher.lastName}`;
    this.modalType = 'researcher';
    this.showModal = true;
  }

  showAnalysisDetails(analysis: Analysis): void {
    this.selectedItem = analysis;
    this.modalTitle = analysis.name;
    this.modalType = 'analysis';
    this.showModal = true;
  }

  showKeywordDetails(keyword: Keyword): void {
    this.selectedItem = keyword;
    this.modalTitle = keyword.name;
    this.modalType = 'keyword';
    this.showModal = true;
  }

  showMicroorganismDetails(microorganism: Microorganism): void {
    this.selectedItem = microorganism;
    this.modalTitle = microorganism.name;
    this.modalType = 'microorganism';
    this.showModal = true;
  }

  showInstrumentDetails(instrument: Instrument): void {
    this.selectedItem = instrument;
    this.modalTitle = instrument.name;
    this.modalType = 'instrument';
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedItem = null;
    this.modalTitle = '';
    this.modalType = '';
  }

  getDisplayName(item: any): string {
    if (item.name) return item.name;
    if (item.firstName && item.lastName) return `${item.firstName} ${item.lastName}`;
    return 'Unknown';
  }

  getInstitutionNames(institutions: Institution[] | undefined): string {
    return institutions?.map(inst => inst.name).join(', ') || '';
  }

  getKeywordNames(keywords: Keyword[] | undefined): string {
    return keywords?.map(kw => kw.name).join(', ') || '';
  }

  getResearcherNames(researchers: Employee[] | undefined): string {
    return researchers?.map(res => `${res.firstName} ${res.lastName}`).join(', ') || '';
  }

  getMicroorganismNames(microorganisms: Microorganism[] | undefined): string {
    return microorganisms?.map(micro => micro.name).join(', ') || '';
  }

  getMicroorganismInstitutions(microorganism: Microorganism): string {
    const institutions = this.allInstitutions.filter(inst => 
      inst.microorganisms?.some(micro => micro.id === microorganism.id)
    );
    return institutions.map(inst => inst.name).join(', ') || '';
  }

  getMicroorganismAnalyses(microorganism: Microorganism): string {
    const analyses = this.allAnalyses.filter(analysis => 
      analysis.microorganisms?.some(micro => micro.id === microorganism.id)
    );
    return analyses.map(analysis => analysis.name).join(', ') || '';
  }

  getInstrumentInstitutions(instrument: Instrument): string {
    const institutions = this.allInstitutions.filter(inst => 
      inst.instruments?.some(instInstrument => instInstrument.id === instrument.id)
    );
    return institutions.map(inst => inst.name).join(', ') || '';
  }

  getGreeting(): string {
    const hour = this.currentDate.getHours();
    if (hour < 12) return 'Good Morning';
    if (hour < 17) return 'Good Afternoon';
    return 'Good Evening';
  }

  onLogout(): void {
    this.appStateService.clearAppState();
    this.router.navigate(['/identity/login']);
  }

  private loadThemePreference(): void {
    if (isPlatformBrowser(this.platformId)) {
      try {
        const savedTheme = localStorage.getItem('sciConnectTheme');
        this.isDarkTheme = savedTheme === 'dark';
      } catch (error) {
        console.warn('Could not load theme preference:', error);
      }
    }
  }

  private saveThemePreference(): void {
    if (isPlatformBrowser(this.platformId)) {
      try {
        localStorage.setItem('sciConnectTheme', this.isDarkTheme ? 'dark' : 'light');
      } catch (error) {
        console.warn('Could not save theme preference:', error);
      }
    }
  }

  private applyTheme(): void {
    if (isPlatformBrowser(this.platformId)) {
      if (this.isDarkTheme) {
        document.body.classList.add('dark-theme');
      } else {
        document.body.classList.remove('dark-theme');
      }
    }
  }

  public toggleTheme(): void {
    this.isDarkTheme = !this.isDarkTheme;
    this.applyTheme();
    this.saveThemePreference();
  }

  public getLogoPath(): string {
    return this.isDarkTheme 
      ? 'assets/images/dark_theme_logo.png' 
      : 'assets/images/white_theme_logo.png';
  }
} 