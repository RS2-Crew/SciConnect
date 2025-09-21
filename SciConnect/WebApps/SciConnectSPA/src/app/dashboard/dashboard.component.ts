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
      institutions: this.dataService.getAllInstitutions().pipe(catchError(() => of([]))),
      analyses: this.dataService.getAllAnalysesWithRelatedData().pipe(catchError(() => of([]))),
      microorganisms: this.dataService.getAllMicroorganisms().pipe(catchError(() => of([]))),
      instruments: this.dataService.getAllInstruments().pipe(catchError(() => of([]))),
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

    this.filteredSearchResults = [
      {
        type: 'institution' as const,
        category: 'INSTITUTIONS',
        items: this.allInstitutions.filter(inst => 
          inst.name.toLowerCase().includes(searchTerm) ||
          (inst.address && inst.address.toLowerCase().includes(searchTerm))
        ).slice(0, 5)
      },
      {
        type: 'analysis' as const,
        category: 'ANALYSES',
        items: this.allAnalyses.filter(analysis => 
          analysis.name.toLowerCase().includes(searchTerm)
        ).slice(0, 5)
      },
      {
        type: 'researcher' as const,
        category: 'RESEARCHERS',
        items: this.allResearchers.filter(researcher => 
          `${researcher.firstName} ${researcher.lastName}`.toLowerCase().includes(searchTerm)
        ).slice(0, 5)
      },
      {
        type: 'keyword' as const,
        category: 'KEYWORDS',
        items: this.allKeywords.filter(keyword => 
          keyword.name.toLowerCase().includes(searchTerm)
        ).slice(0, 5)
      },
      {
        type: 'instrument' as const,
        category: 'INSTRUMENTS',
        items: this.allInstruments.filter(instrument => 
          instrument.name.toLowerCase().includes(searchTerm)
        ).slice(0, 5)
      },
      {
        type: 'microorganism' as const,
        category: 'MICROORGANISMS',
        items: this.allMicroorganisms.filter(micro => 
          micro.name.toLowerCase().includes(searchTerm)
        ).slice(0, 5)
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
    if (this.selectedInstitution) {
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
      if (selectedInstitution) {
        // Ensure the selected institution is included in results
        if (!this.filteredResults.institutions.some(inst => inst.id === this.selectedInstitution)) {
          this.filteredResults.institutions = [selectedInstitution, ...this.filteredResults.institutions];
        }
        
        const institutionAnalyses = this.allAnalyses.filter(analysis => 
          analysis.institutions?.some(inst => inst.id === this.selectedInstitution)
        );
        this.filteredResults.analyses = institutionAnalyses;

        if (selectedInstitution.microorganisms) {
          const institutionMicroorganisms = this.allMicroorganisms.filter(micro => 
            selectedInstitution.microorganisms?.some(instMicro => instMicro.id === micro.id)
          );
          this.filteredResults.microorganisms = institutionMicroorganisms;
        }

        if (selectedInstitution.instruments) {
          const institutionInstruments = this.allInstruments.filter(inst => 
            selectedInstitution.instruments?.some(instInstrument => instInstrument.id === inst.id)
          );
          this.filteredResults.instruments = institutionInstruments;
        }

        const institutionResearchers = this.allResearchers.filter(researcher => 
          researcher.institution?.id === this.selectedInstitution
        );
        this.filteredResults.researchers = institutionResearchers;
      }
    }

    if (this.selectedAnalysis) {
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
      if (selectedAnalysis) {
        // Ensure the selected analysis is included in results
        if (!this.filteredResults.analyses.some(anal => anal.id === this.selectedAnalysis)) {
          this.filteredResults.analyses = [selectedAnalysis, ...this.filteredResults.analyses];
        }
        
        if (selectedAnalysis.institutions) {
          const analysisInstitutions = this.allInstitutions.filter(inst => 
            selectedAnalysis.institutions?.some(analysisInst => analysisInst.id === inst.id)
          );
          this.filteredResults.institutions = analysisInstitutions;
        }

        if (selectedAnalysis.microorganisms) {
          const analysisMicroorganisms = this.allMicroorganisms.filter(micro => 
            selectedAnalysis.microorganisms?.some(analysisMicro => analysisMicro.id === micro.id)
          );
          this.filteredResults.microorganisms = analysisMicroorganisms;
        }
      }
    }

    if (this.selectedMicroorganism) {
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === this.selectedMicroorganism);
      if (selectedMicroorganism) {
        // Ensure the selected microorganism is included in results
        if (!this.filteredResults.microorganisms.some(micro => micro.id === this.selectedMicroorganism)) {
          this.filteredResults.microorganisms = [selectedMicroorganism, ...this.filteredResults.microorganisms];
        }
        
        const microorganismAnalyses = this.allAnalyses.filter(analysis => 
          analysis.microorganisms?.some(micro => micro.id === this.selectedMicroorganism)
        );
        this.filteredResults.analyses = microorganismAnalyses;
      }
    }

    if (this.selectedInstrument) {
      const selectedInstrument = this.allInstruments.find(inst => inst.id === this.selectedInstrument);
      if (selectedInstrument) {
        // Ensure the selected instrument is included in results
        if (!this.filteredResults.instruments.some(inst => inst.id === this.selectedInstrument)) {
          this.filteredResults.instruments = [selectedInstrument, ...this.filteredResults.instruments];
        }
        
        const instrumentInstitutions = this.allInstitutions.filter(inst => 
          inst.instruments?.some(instrument => instrument.id === this.selectedInstrument)
        );
        this.filteredResults.institutions = [...new Set([...this.filteredResults.institutions, ...instrumentInstitutions])];
      }
    }

    if (this.selectedResearcher) {
      const selectedResearcher = this.allResearchers.find(res => res.id === this.selectedResearcher);
      if (selectedResearcher) {
        // Ensure the selected researcher is included in results
        if (!this.filteredResults.researchers.some(res => res.id === this.selectedResearcher)) {
          this.filteredResults.researchers = [selectedResearcher, ...this.filteredResults.researchers];
        }
        
        if (selectedResearcher.institution) {
          const researcherInstitution = this.allInstitutions.find(inst => 
            inst.id === selectedResearcher.institution?.id
          );
          if (researcherInstitution) {
            this.filteredResults.institutions = [...new Set([...this.filteredResults.institutions, researcherInstitution])];
          }
        }

        if (selectedResearcher.keywords && selectedResearcher.keywords.length > 0) {
          // Only show keywords that are directly associated with this specific researcher
          const researcherKeywords = this.allKeywords.filter(kw => 
            selectedResearcher.keywords?.some(resKeyword => resKeyword.id === kw.id)
          );
          console.log(`Researcher ${selectedResearcher.firstName} ${selectedResearcher.lastName} keywords:`, selectedResearcher.keywords);
          console.log(`Filtered keywords for display:`, researcherKeywords);
          this.filteredResults.keywords = researcherKeywords;
        } else {
          // If researcher has no keywords, show no keywords
          console.log(`Researcher ${selectedResearcher.firstName} ${selectedResearcher.lastName} has no keywords`);
          this.filteredResults.keywords = [];
        }
      }
    }

    if (this.selectedKeyword) {
      const selectedKeyword = this.allKeywords.find(kw => kw.id === this.selectedKeyword);
      if (selectedKeyword) {
        // Ensure the selected keyword is included in results
        if (!this.filteredResults.keywords.some(kw => kw.id === this.selectedKeyword)) {
          this.filteredResults.keywords = [selectedKeyword, ...this.filteredResults.keywords];
        }
        
        const keywordResearchers = this.allResearchers.filter(researcher => 
          researcher.keywords?.some(kw => kw.id === this.selectedKeyword)
        );
        this.filteredResults.researchers = [...new Set([...this.filteredResults.researchers, ...keywordResearchers])];
      }
    }
  }

  private getFilteredInstitutions(): Institution[] {
    let institutions = [...this.allInstitutions];

    if (!this.hasActiveFilters()) {
      return institutions;
    }

    if (this.selectedInstitution) {
      institutions = institutions.filter(inst => inst.id === this.selectedInstitution);
    }

    if (this.selectedAnalysis) {
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
      if (selectedAnalysis?.institutions) {
        const analysisInstitutionIds = selectedAnalysis.institutions.map(inst => inst.id);
        institutions = institutions.filter(inst => analysisInstitutionIds.includes(inst.id));
      }
    }

    if (this.selectedInstrument) {
      const selectedInstrument = this.allInstruments.find(inst => inst.id === this.selectedInstrument);
      if (selectedInstrument) {
        institutions = institutions.filter(inst => 
          inst.instruments?.some(instrument => instrument.id === this.selectedInstrument)
        );
      }
    }

    if (this.selectedMicroorganism) {
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === this.selectedMicroorganism);
      if (selectedMicroorganism) {
        institutions = institutions.filter(inst => 
          inst.microorganisms?.some(micro => micro.id === this.selectedMicroorganism)
        );
      }
    }

    if (this.selectedResearcher) {
      const selectedResearcher = this.allResearchers.find(res => res.id === this.selectedResearcher);
      if (selectedResearcher?.institution) {
        institutions = institutions.filter(inst => inst.id === selectedResearcher.institution?.id);
      }
    }

    if (this.selectedKeyword) {
      const selectedKeyword = this.allKeywords.find(kw => kw.id === this.selectedKeyword);
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

    if (this.selectedAnalysis) {
      analyses = analyses.filter(anal => anal.id === this.selectedAnalysis);
    }

    if (this.selectedInstitution) {
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
      if (selectedInstitution) {
        analyses = analyses.filter(analysis => 
          analysis.institutions?.some(inst => inst.id === this.selectedInstitution)
        );
      }
    }

    if (this.selectedMicroorganism) {
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === this.selectedMicroorganism);
      if (selectedMicroorganism) {
        analyses = analyses.filter(analysis => 
          analysis.microorganisms?.some(micro => micro.id === this.selectedMicroorganism)
        );
      }
    }

    if (this.selectedInstrument) {
      const selectedInstrument = this.allInstruments.find(inst => inst.id === this.selectedInstrument);
      if (selectedInstrument) {
        const instrumentInstitutionIds = this.allInstitutions
          .filter(inst => inst.instruments?.some(instrument => instrument.id === this.selectedInstrument))
          .map(inst => inst.id);
        analyses = analyses.filter(analysis => 
          analysis.institutions?.some(inst => instrumentInstitutionIds.includes(inst.id))
        );
      }
    }

    if (this.selectedResearcher) {
      const selectedResearcher = this.allResearchers.find(res => res.id === this.selectedResearcher);
      if (selectedResearcher?.institution) {
        analyses = analyses.filter(analysis => 
          analysis.institutions?.some(inst => inst.id === selectedResearcher.institution?.id)
        );
      }
    }

    if (this.selectedKeyword) {
      const selectedKeyword = this.allKeywords.find(kw => kw.id === this.selectedKeyword);
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

    if (this.selectedInstrument) {
      instruments = instruments.filter(inst => inst.id === this.selectedInstrument);
    }

    if (this.selectedInstitution) {
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
      if (selectedInstitution?.instruments) {
        const institutionInstrumentIds = selectedInstitution.instruments.map(inst => inst.id);
        instruments = instruments.filter(inst => institutionInstrumentIds.includes(inst.id));
      }
    }

    if (this.selectedAnalysis) {
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
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
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === this.selectedMicroorganism);
      if (selectedMicroorganism) {
        const microorganismInstitutionIds = this.allInstitutions
          .filter(inst => inst.microorganisms?.some(micro => micro.id === this.selectedMicroorganism))
          .map(inst => inst.id);
        const microorganismInstitutionInstruments = this.allInstitutions
          .filter(inst => microorganismInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.instruments || []);
        const microorganismInstrumentIds = microorganismInstitutionInstruments.map(inst => inst.id);
        instruments = instruments.filter(inst => microorganismInstrumentIds.includes(inst.id));
      }
    }

    if (this.selectedResearcher) {
      const selectedResearcher = this.allResearchers.find(res => res.id === this.selectedResearcher);
      if (selectedResearcher?.institution) {
        const researcherInstitution = this.allInstitutions.find(inst => inst.id === selectedResearcher.institution?.id);
        if (researcherInstitution?.instruments) {
          const researcherInstrumentIds = researcherInstitution.instruments.map(inst => inst.id);
          instruments = instruments.filter(inst => researcherInstrumentIds.includes(inst.id));
        }
      }
    }

    if (this.selectedKeyword) {
      const selectedKeyword = this.allKeywords.find(kw => kw.id === this.selectedKeyword);
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
      keywords = keywords.filter(kw => kw.id === this.selectedKeyword);
    }

    if (this.selectedResearcher) {
      const selectedResearcher = this.allResearchers.find(res => res.id === this.selectedResearcher);
      if (selectedResearcher?.keywords && selectedResearcher.keywords.length > 0) {
        // Only show keywords that are directly associated with this specific researcher
        const researcherKeywordIds = selectedResearcher.keywords.map(kw => kw.id);
        keywords = keywords.filter(kw => researcherKeywordIds.includes(kw.id));
      } else {
        // If researcher has no keywords, return empty array
        keywords = [];
      }
    }

    if (this.selectedInstitution) {
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
      if (selectedInstitution) {
        const institutionResearcherIds = this.allResearchers
          .filter(res => res.institution?.id === this.selectedInstitution)
          .map(res => res.id);
        const institutionKeywordIds = this.allKeywords
          .filter(kw => kw.researchers?.some(res => institutionResearcherIds.includes(res.id)))
          .map(kw => kw.id);
        keywords = keywords.filter(kw => institutionKeywordIds.includes(kw.id));
      }
    }

    if (this.selectedAnalysis) {
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
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
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === this.selectedMicroorganism);
      if (selectedMicroorganism) {
        const microorganismInstitutionIds = this.allInstitutions
          .filter(inst => inst.microorganisms?.some(micro => micro.id === this.selectedMicroorganism))
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
      const selectedInstrument = this.allInstruments.find(inst => inst.id === this.selectedInstrument);
      if (selectedInstrument) {
        const instrumentInstitutionIds = this.allInstitutions
          .filter(inst => inst.instruments?.some(instrument => instrument.id === this.selectedInstrument))
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
      researchers = researchers.filter(res => res.id === this.selectedResearcher);
    }

    if (this.selectedInstitution) {
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
      if (selectedInstitution) {
        researchers = researchers.filter(researcher => 
          researcher.institution?.id === this.selectedInstitution
        );
      }
    }

    if (this.selectedKeyword) {
      const selectedKeyword = this.allKeywords.find(kw => kw.id === this.selectedKeyword);
      if (selectedKeyword) {
        researchers = researchers.filter(researcher => 
          researcher.keywords?.some(kw => kw.id === this.selectedKeyword)
        );
      }
    }

    if (this.selectedAnalysis) {
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
      if (selectedAnalysis?.institutions) {
        const analysisInstitutionIds = selectedAnalysis.institutions.map(inst => inst.id);
        researchers = researchers.filter(researcher => 
          analysisInstitutionIds.includes(researcher.institution?.id || 0)
        );
      }
    }

    if (this.selectedMicroorganism) {
      const selectedMicroorganism = this.allMicroorganisms.find(micro => micro.id === this.selectedMicroorganism);
      if (selectedMicroorganism) {
        const microorganismInstitutionIds = this.allInstitutions
          .filter(inst => inst.microorganisms?.some(micro => micro.id === this.selectedMicroorganism))
          .map(inst => inst.id);
        researchers = researchers.filter(researcher => 
          microorganismInstitutionIds.includes(researcher.institution?.id || 0)
        );
      }
    }

    if (this.selectedInstrument) {
      const selectedInstrument = this.allInstruments.find(inst => inst.id === this.selectedInstrument);
      if (selectedInstrument) {
        const instrumentInstitutionIds = this.allInstitutions
          .filter(inst => inst.instruments?.some(instrument => instrument.id === this.selectedInstrument))
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

    if (this.selectedMicroorganism) {
      microorganisms = microorganisms.filter(micro => micro.id === this.selectedMicroorganism);
    }

    if (this.selectedAnalysis) {
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
      if (selectedAnalysis?.microorganisms) {
        const analysisMicroorganismIds = selectedAnalysis.microorganisms.map(micro => micro.id);
        microorganisms = microorganisms.filter(micro => analysisMicroorganismIds.includes(micro.id));
      }
    }

    if (this.selectedInstitution) {
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
      if (selectedInstitution?.microorganisms) {
        const institutionMicroorganismIds = selectedInstitution.microorganisms.map(micro => micro.id);
        microorganisms = microorganisms.filter(micro => institutionMicroorganismIds.includes(micro.id));
      }
    }

    if (this.selectedInstrument) {
      const selectedInstrument = this.allInstruments.find(inst => inst.id === this.selectedInstrument);
      if (selectedInstrument) {
        const instrumentInstitutionIds = this.allInstitutions
          .filter(inst => inst.instruments?.some(instrument => instrument.id === this.selectedInstrument))
          .map(inst => inst.id);
        const instrumentInstitutionMicroorganismIds = this.allInstitutions
          .filter(inst => instrumentInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.microorganisms || [])
          .map(micro => micro.id);
        microorganisms = microorganisms.filter(micro => instrumentInstitutionMicroorganismIds.includes(micro.id));
      }
    }

    if (this.selectedResearcher) {
      const selectedResearcher = this.allResearchers.find(res => res.id === this.selectedResearcher);
      if (selectedResearcher?.institution) {
        const researcherInstitution = this.allInstitutions.find(inst => inst.id === selectedResearcher.institution?.id);
        if (researcherInstitution?.microorganisms) {
          const researcherMicroorganismIds = researcherInstitution.microorganisms.map(micro => micro.id);
          microorganisms = microorganisms.filter(micro => researcherMicroorganismIds.includes(micro.id));
        }
      }
    }

    if (this.selectedKeyword) {
      const selectedKeyword = this.allKeywords.find(kw => kw.id === this.selectedKeyword);
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