import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
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
  // User info
  userName: string = '';
  currentDate: Date = new Date();
  currentLanguage: 'en' | 'sr' = 'en';

  // Tab management
  activeTab: 'analyses' | 'researchers' = 'analyses';

  // Master search
  masterSearchTerm: string = '';
  showSearchSuggestions: boolean = false;
  filteredSearchResults: SearchResult[] = [];

  // Guided search - selections
  selectedInstitution: number | null = null;
  selectedAnalysis: number | null = null;
  selectedMicroorganism: number | null = null;
  selectedInstrument: number | null = null;
  selectedResearcher: number | null = null;
  selectedKeyword: number | null = null;

  // Guided search - filtered options
  filteredInstitutions: Institution[] = [];
  filteredAnalyses: Analysis[] = [];
  filteredMicroorganisms: Microorganism[] = [];
  filteredInstruments: Instrument[] = [];
  filteredResearchers: Employee[] = [];
  filteredKeywords: Keyword[] = [];

  // Selected item details
  selectedInstitutionDetails: Institution | null = null;
  selectedAnalysisDetails: Analysis | null = null;
  selectedResearcherDetails: Employee | null = null;

  // Pinning functionality
  pinnedCategory: string | null = null;

  // Results
  filteredResults: FilteredResults = {
    institutions: [],
    analyses: [],
    instruments: [],
    keywords: [],
    researchers: [],
    microorganisms: []
  };

  // Modal
  showModal: boolean = false;
  modalTitle: string = '';
  modalType: string = '';
  selectedItem: any = null;

  // All data
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
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadInitialData();
    this.setupSearchDebouncing();
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
      analyses: this.dataService.getAllAnalyses().pipe(catchError(() => of([]))),
      microorganisms: this.dataService.getAllMicroorganisms().pipe(catchError(() => of([]))),
      instruments: this.dataService.getAllInstruments().pipe(catchError(() => of([]))),
      researchers: this.dataService.getAllEmployees().pipe(catchError(() => of([]))),
      keywords: this.dataService.getAllKeywords().pipe(catchError(() => of([])))
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
    // This would be implemented with a proper search service
    // For now, we'll handle it in the input event
  }

  // Tab management
  setActiveTab(tab: 'analyses' | 'researchers'): void {
    this.activeTab = tab;
    this.updateResults();
  }

  // Master search functionality
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

  // Pinning functionality
  pinCategory(category: string): void {
    if (this.pinnedCategory === category) {
      this.pinnedCategory = null;
    } else {
      this.pinnedCategory = category;
    }
    this.updateFilteredOptions();
    this.updateResults();
  }

  // Filter change handlers
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

  // Update filtered options based on selections
  private updateFilteredOptions(): void {
    // This is a simplified version - in a real implementation,
    // you would make API calls to get filtered data based on relationships
    
    if (this.pinnedCategory === 'institution' && this.selectedInstitution) {
      // Filter based on selected institution
      const institution = this.allInstitutions.find(inst => inst.id === this.selectedInstitution);
      if (institution) {
        // Filter analyses, instruments, etc. based on institution relationships
        // This would require API calls to get related data
      }
    } else if (this.pinnedCategory === 'analysis' && this.selectedAnalysis) {
      // Filter based on selected analysis
      const analysis = this.allAnalyses.find(anal => anal.id === this.selectedAnalysis);
      if (analysis) {
        // Filter institutions, instruments, etc. based on analysis relationships
      }
    }
    // Add similar logic for other pinned categories
  }

  // Update results based on current filters
  private updateResults(): void {
    // This is a simplified version - in a real implementation,
    // you would make API calls to get filtered results
    
    this.filteredResults = {
      institutions: this.selectedInstitution ? 
        this.allInstitutions.filter(inst => inst.id === this.selectedInstitution) : 
        this.allInstitutions,
      analyses: this.selectedAnalysis ? 
        this.allAnalyses.filter(anal => anal.id === this.selectedAnalysis) : 
        this.allAnalyses,
      instruments: this.selectedInstrument ? 
        this.allInstruments.filter(inst => inst.id === this.selectedInstrument) : 
        this.allInstruments,
      keywords: this.selectedKeyword ? 
        this.allKeywords.filter(kw => kw.id === this.selectedKeyword) : 
        this.allKeywords,
      researchers: this.selectedResearcher ? 
        this.allResearchers.filter(res => res.id === this.selectedResearcher) : 
        this.allResearchers,
      microorganisms: this.selectedMicroorganism ? 
        this.allMicroorganisms.filter(micro => micro.id === this.selectedMicroorganism) : 
        this.allMicroorganisms
    };
  }

  // Utility methods
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

  // Language switching
  toggleLanguage(): void {
    this.currentLanguage = this.currentLanguage === 'en' ? 'sr' : 'en';
    this.translationService.setLanguage(this.currentLanguage);
  }

  // Modal functionality
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

  closeModal(): void {
    this.showModal = false;
    this.selectedItem = null;
    this.modalTitle = '';
    this.modalType = '';
  }

  // Display name helpers
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

  // Utility methods
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
} 