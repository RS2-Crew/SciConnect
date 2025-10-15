import {
  Component,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  Subject,
  takeUntil,
  forkJoin,
  of,
} from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AppStateService } from '../shared/app-state/app-state.service';
import { DataService } from '../shared/services/data.service';
import { AnalyticsService, SummaryAnalyticsResponse, InstitutionBreakdownResponse, TopInstitutionResponse } from '../shared/services/analytics.service';
import { ThemeService } from '../shared/services/theme.service';
import { AuthentificationFacadeService } from '../identity/domain/application-services/authentification-facade.service';
import { FilterUtils } from '../shared/utils/filter-utils';

import {
  Institution,
  Analysis,
  Instrument,
  Keyword,
  Employee,
  Microorganism,
  FilterState,
} from '../shared/models/data-models';

interface SearchResult {
  type:
    | 'institution'
    | 'analysis'
    | 'instrument'
    | 'keyword'
    | 'researcher'
    | 'microorganism';
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
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit, OnDestroy {
  userName: string = '';
  userRoles: string[] = [];
  currentDate: Date = new Date();

  activeTab: 'analyses' | 'researchers' | 'database' = 'analyses';

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

  filteredResults: FilteredResults = {
    institutions: [],
    analyses: [],
    instruments: [],
    keywords: [],
    researchers: [],
    microorganisms: [],
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

  newInstitutionName: string = '';
  newInstitutionDesc: string = '';
  newInstitutionCity: string = '';
  newInstitutionStreet: string = '';
  newInstitutionCountry: string = '';
  newInstitutionStreetNumber: string = '';
  newInstrumentName: string = '';
  newInstrumentDesc: string = '';
  newKeywordName: string = '';
  newAnalysisName: string = '';
  newAnalysisDesc: string = '';
  newEmployeeFirstName: string = '';
  newEmployeeLastName: string = '';
  newEmployeeEmail: string = '';
  newEmployeeInstitutionId: number | null = null;
  newMicroorganismName: string = '';

  selectedConnectionInstitution: number | null = null;
  selectedConnectionAnalysis: number | null = null;
  selectedConnectionInstrument: number | null = null;
  selectedConnectionMicroorganism: number | null = null;
  selectedConnectionResearcher: number | null = null;
  selectedConnectionKeyword: number | null = null;

  analyticsData: SummaryAnalyticsResponse | null = null;
  showAnalytics: boolean = false;
  topInstitutionsData: TopInstitutionResponse[] = [];
  institutionBreakdownData: InstitutionBreakdownResponse | null = null;
  selectedInstitutionId: number | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private appStateService: AppStateService,
    private dataService: DataService,
    private analyticsService: AnalyticsService,
    public themeService: ThemeService,
    private router: Router,
    private authentificationFacadeService: AuthentificationFacadeService
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadInitialData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private handleCrudOperation(
    operation$: ReturnType<typeof this.dataService.createInstitution>,
    onSuccess?: () => void
  ): void {
    operation$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          if (onSuccess) onSuccess();
          this.loadInitialData();
          if (this.showAnalytics) {
            this.refreshAnalyticsData();
          }
        },
        error: () => {}
      });
  }

  private handleConnectionOperation(
    operation$: ReturnType<typeof this.dataService.connectInstitutionToAnalysis>,
    onSuccess?: () => void
  ): void {
    operation$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          if (onSuccess) onSuccess();
          this.loadInitialData();
          if (this.showAnalytics) {
            this.refreshAnalyticsData();
          }
        },
        error: () => {}
      });
  }

  private connect(
    operation$: ReturnType<typeof this.dataService.connectInstitutionToAnalysis>,
    resetIds: () => void
  ): void {
    this.handleConnectionOperation(operation$, resetIds);
  }

  private getRelatedIds<T extends { id: number }>(entities: T[] | undefined): number[] {
    return entities?.map(e => e.id) || [];
  }

  private getInstitutionIdsByMicroorganism(microorganismId: number): number[] {
    return this.allInstitutions
      .filter(inst => inst.microorganisms?.some(m => m.id === microorganismId))
      .map(inst => inst.id);
  }

  private getInstitutionIdsByInstrument(instrumentId: number): number[] {
    return this.allInstitutions
      .filter(inst => inst.instruments?.some(i => i.id === instrumentId))
      .map(inst => inst.id);
  }

  private getInstitutionIdsByKeyword(keywordId: number): number[] {
    const keyword = this.allKeywords.find(kw => kw.id === keywordId);
    if (!keyword) return [];
    
    const researcherIds = this.getRelatedIds(keyword.researchers);
    return this.allResearchers
      .filter(res => researcherIds.includes(res.id))
      .map(res => res.institution?.id)
      .filter((id): id is number => id !== undefined);
  }

  private getResearcherIdsByInstitution(institutionId: number): number[] {
    return this.allResearchers
      .filter(res => res.institution?.id === institutionId)
      .map(res => res.id);
  }

  private getInstitutionIdsFromResearchers(researcherIds: number[]): number[] {
    return this.allResearchers
      .filter(res => researcherIds.includes(res.id))
      .map(res => res.institution?.id)
      .filter((id): id is number => id !== undefined);
  }


  private loadUserInfo(): void {
    this.appStateService
      .getAppState()
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => {
        this.userName =
          state.firstName && state.lastName
            ? `${state.firstName} ${state.lastName}`
            : state.username || 'User';
        
        if (state.roles) {
          this.userRoles = Array.isArray(state.roles) 
            ? state.roles.map(role => role.toString()) 
            : [state.roles.toString()];
        } else {
          this.userRoles = [];
        }
      });
  }

  private loadInitialData(): void {
    forkJoin({
      institutions: this.dataService
        .getAllInstitutionsWithRelatedData()
        .pipe(catchError(() => of([]))),
      analyses: this.dataService
        .getAllAnalysesWithRelatedData()
        .pipe(catchError(() => of([]))),
      microorganisms: this.dataService
        .getAllMicroorganismsWithRelatedData()
        .pipe(catchError(() => of([]))),
      instruments: this.dataService
        .getAllInstrumentsWithRelatedData()
        .pipe(catchError(() => of([]))),
      researchers: this.dataService
        .getAllEmployeesWithRelatedData()
        .pipe(catchError(() => of([]))),
      keywords: this.dataService
        .getAllKeywordsWithRelatedData()
        .pipe(catchError(() => of([]))),
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe((data) => {
        this.allInstitutions = data.institutions;
        this.allAnalyses = data.analyses;
        this.allMicroorganisms = data.microorganisms;
        this.allInstruments = data.instruments;
        this.allResearchers = data.researchers;
        this.allKeywords = data.keywords;

        this.filteredInstitutions = [...this.allInstitutions];
        this.filteredAnalyses = [...this.allAnalyses];
        this.filteredMicroorganisms = [...this.allMicroorganisms];
        this.filteredInstruments = [...this.allInstruments];
        this.filteredResearchers = [...this.allResearchers];
        this.filteredKeywords = [...this.allKeywords];
      });
  }

  setActiveTab(tab: 'analyses' | 'researchers' | 'database'): void {
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

    const getSelectedItem = (id: number | null, items: any[]): any => {
      return id ? items.find((item) => item.id === id) : null;
    };

    const selectedInstitution = getSelectedItem(
      this.selectedInstitution,
      this.allInstitutions
    );
    const selectedAnalysis = getSelectedItem(
      this.selectedAnalysis,
      this.allAnalyses
    );
    const selectedResearcher = getSelectedItem(
      this.selectedResearcher,
      this.allResearchers
    );
    const selectedKeyword = getSelectedItem(
      this.selectedKeyword,
      this.allKeywords
    );
    const selectedInstrument = getSelectedItem(
      this.selectedInstrument,
      this.allInstruments
    );
    const selectedMicroorganism = getSelectedItem(
      this.selectedMicroorganism,
      this.allMicroorganisms
    );

    // Helper function to filter items and include selected item if it matches
    const filterAndIncludeSelected = (
      items: any[],
      selectedItem: any,
      searchFn: (item: any) => boolean
    ): any[] => {
      const filtered = items.filter(searchFn);

      // If there's a selected item and it matches the search term, add it to the results
      if (selectedItem && searchFn(selectedItem)) {
        // Check if selected item is already in the filtered results
        const isAlreadyIncluded = filtered.some(
          (item) => item.id === selectedItem.id
        );
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
          (inst) =>
            inst.name.toLowerCase().includes(searchTerm) ||
            (inst.address && inst.address.toLowerCase().includes(searchTerm))
        ),
      },
      {
        type: 'analysis' as const,
        category: 'ANALYSES',
        items: filterAndIncludeSelected(
          this.allAnalyses,
          selectedAnalysis,
          (analysis) => analysis.name.toLowerCase().includes(searchTerm)
        ),
      },
      {
        type: 'researcher' as const,
        category: 'RESEARCHERS',
        items: filterAndIncludeSelected(
          this.allResearchers,
          selectedResearcher,
          (researcher) =>
            `${researcher.firstName} ${researcher.lastName}`
              .toLowerCase()
              .includes(searchTerm)
        ),
      },
      {
        type: 'keyword' as const,
        category: 'KEYWORDS',
        items: filterAndIncludeSelected(
          this.allKeywords,
          selectedKeyword,
          (keyword) => keyword.name.toLowerCase().includes(searchTerm)
        ),
      },
      {
        type: 'instrument' as const,
        category: 'INSTRUMENTS',
        items: filterAndIncludeSelected(
          this.allInstruments,
          selectedInstrument,
          (instrument) =>
            instrument.name.toLowerCase().includes(searchTerm) ||
            (instrument.description &&
              instrument.description.toLowerCase().includes(searchTerm)) ||
            (instrument.manufacturer &&
              instrument.manufacturer.toLowerCase().includes(searchTerm)) ||
            (instrument.model &&
              instrument.model.toLowerCase().includes(searchTerm))
        ),
      },
      {
        type: 'microorganism' as const,
        category: 'MICROORGANISMS',
        items: filterAndIncludeSelected(
          this.allMicroorganisms,
          selectedMicroorganism,
          (micro) => micro.name.toLowerCase().includes(searchTerm)
        ),
      },
    ].filter((group) => group.items.length > 0);
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
      microorganism: 'fas fa-bug',
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

  onInstitutionChange(): void {
    const institution = this.allInstitutions.find(
      (inst) => inst.id === this.selectedInstitution
    );
    this.selectedInstitutionDetails = institution || null;

    this.updateFilteredOptions();
    this.updateResults();
  }

  onAnalysisChange(): void {
    const analysis = this.allAnalyses.find(
      (anal) => anal.id === this.selectedAnalysis
    );
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
    const researcher = this.allResearchers.find(
      (res) => res.id === this.selectedResearcher
    );
    this.selectedResearcherDetails = researcher || null;
    this.updateFilteredOptions();
    this.updateResults();
  }

  onKeywordChange(): void {
    this.updateFilteredOptions();
    this.updateResults();
  }

  private updateFilteredOptions(): void {
    // This method can be extended if needed for filtering options
  }

  private updateResults(): void {
    this.filteredResults = {
      institutions: this.getFilteredInstitutions(),
      analyses: this.getFilteredAnalyses(),
      instruments: this.getFilteredInstruments(),
      keywords: this.getFilteredKeywords(),
      researchers: this.getFilteredResearchers(),
      microorganisms: this.getFilteredMicroorganisms(),
    };

    if (this.hasActiveFilters()) {
      this.addRelatedEntities();
    }
  }

  private addRelatedEntities(): void {
    // Add selected entities to results if they're not already included
    // This ensures the selected item is always visible even if filters are narrow
    
    if (this.selectedInstitution) {
      const institutionId = FilterUtils.parseId(this.selectedInstitution);
      const selectedInstitution = this.allInstitutions.find(
        (inst) => inst.id === institutionId
      );
      if (
        selectedInstitution &&
        !this.filteredResults.institutions.some(
          (inst) => inst.id === institutionId
        )
      ) {
        this.filteredResults.institutions = [
          selectedInstitution,
          ...this.filteredResults.institutions,
        ];
      }
    }

    if (this.selectedAnalysis) {
      const analysisId = FilterUtils.parseId(this.selectedAnalysis);
      const selectedAnalysis = this.allAnalyses.find(
        (anal) => anal.id === analysisId
      );
      if (
        selectedAnalysis &&
        !this.filteredResults.analyses.some((anal) => anal.id === analysisId)
      ) {
        this.filteredResults.analyses = [
          selectedAnalysis,
          ...this.filteredResults.analyses,
        ];
      }
    }

    if (this.selectedMicroorganism) {
      const microorganismId = FilterUtils.parseId(this.selectedMicroorganism);
      const selectedMicroorganism = this.allMicroorganisms.find(
        (micro) => micro.id === microorganismId
      );
      if (
        selectedMicroorganism &&
        !this.filteredResults.microorganisms.some(
          (micro) => micro.id === microorganismId
        )
      ) {
        this.filteredResults.microorganisms = [
          selectedMicroorganism,
          ...this.filteredResults.microorganisms,
        ];
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = FilterUtils.parseId(this.selectedInstrument);
      const selectedInstrument = this.allInstruments.find(
        (inst) => inst.id === instrumentId
      );
      if (
        selectedInstrument &&
        !this.filteredResults.instruments.some(
          (inst) => inst.id === instrumentId
        )
      ) {
        this.filteredResults.instruments = [
          selectedInstrument,
          ...this.filteredResults.instruments,
        ];
      }
    }

    if (this.selectedResearcher) {
      const researcherId = FilterUtils.parseId(this.selectedResearcher);
      const selectedResearcher = this.allResearchers.find(
        (res) => res.id === researcherId
      );
      if (
        selectedResearcher &&
        !this.filteredResults.researchers.some((res) => res.id === researcherId)
      ) {
        this.filteredResults.researchers = [
          selectedResearcher,
          ...this.filteredResults.researchers,
        ];
      }
    }

    if (this.selectedKeyword) {
      const keywordId = FilterUtils.parseId(this.selectedKeyword);
      const selectedKeyword = this.allKeywords.find(
        (kw) => kw.id === keywordId
      );
      if (
        selectedKeyword &&
        !this.filteredResults.keywords.some((kw) => kw.id === keywordId)
      ) {
        this.filteredResults.keywords = [
          selectedKeyword,
          ...this.filteredResults.keywords,
        ];
      }
    }
  }

  private getFilteredInstitutions(): Institution[] {
    let institutions = [...this.allInstitutions];

    if (!this.hasActiveFilters()) {
      return institutions;
    }

    // Apply filters
    if (this.selectedInstitution) {
      const institutionId = FilterUtils.parseId(this.selectedInstitution);
      institutions = institutions.filter(inst => inst.id === institutionId);
    }

    if (this.selectedAnalysis) {
      const analysisId = FilterUtils.parseId(this.selectedAnalysis);
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      const analysisInstitutionIds = this.getRelatedIds(selectedAnalysis?.institutions);
      institutions = institutions.filter(inst => analysisInstitutionIds.includes(inst.id));
    }

    if (this.selectedInstrument) {
      const instrumentId = FilterUtils.parseId(this.selectedInstrument);
      if (instrumentId !== null) {
        const validInstitutionIds = this.getInstitutionIdsByInstrument(instrumentId);
        institutions = institutions.filter(inst => validInstitutionIds.includes(inst.id));
      }
    }

    if (this.selectedMicroorganism) {
      const microorganismId = FilterUtils.parseId(this.selectedMicroorganism);
      if (microorganismId !== null) {
        const validInstitutionIds = this.getInstitutionIdsByMicroorganism(microorganismId);
        institutions = institutions.filter(inst => validInstitutionIds.includes(inst.id));
      }
    }

    if (this.selectedResearcher) {
      const researcherId = FilterUtils.parseId(this.selectedResearcher);
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      if (selectedResearcher?.institution) {
        institutions = institutions.filter(inst => inst.id === selectedResearcher.institution?.id);
      }
    }

    if (this.selectedKeyword) {
      const keywordId = FilterUtils.parseId(this.selectedKeyword);
      if (keywordId !== null) {
        const validInstitutionIds = this.getInstitutionIdsByKeyword(keywordId);
        institutions = institutions.filter(inst => validInstitutionIds.includes(inst.id));
      }
    }

    return institutions;
  }

  private getFilteredAnalyses(): Analysis[] {
    let analyses = [...this.allAnalyses];

    if (!this.hasActiveFilters()) {
      return analyses;
    }

    // Apply ALL filters with AND logic (intersection)
    if (this.selectedInstitution) {
      const institutionId = FilterUtils.parseId(this.selectedInstitution);
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      
      if (selectedInstitution?.analyses && selectedInstitution.analyses.length > 0) {
        const institutionAnalysisIds = this.getRelatedIds(selectedInstitution.analyses);
        analyses = analyses.filter(analysis => institutionAnalysisIds.includes(analysis.id));
      } else {
        // If institution has no analyses, return empty
        return [];
      }
    }

    if (this.selectedAnalysis) {
      const analysisId = FilterUtils.parseId(this.selectedAnalysis);
      analyses = analyses.filter(anal => anal.id === analysisId);
    }

    if (this.selectedMicroorganism) {
      const microorganismId = FilterUtils.parseId(this.selectedMicroorganism);
      if (microorganismId !== null) {
        analyses = analyses.filter(analysis =>
          analysis.microorganisms?.some(micro => micro.id === microorganismId)
        );
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = FilterUtils.parseId(this.selectedInstrument);
      if (instrumentId !== null) {
        const instrumentInstitutionIds = this.getInstitutionIdsByInstrument(instrumentId);
        analyses = analyses.filter(analysis =>
          analysis.institutions?.some(inst => instrumentInstitutionIds.includes(inst.id))
        );
      }
    }

    if (this.selectedResearcher) {
      const researcherId = FilterUtils.parseId(this.selectedResearcher);
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      if (selectedResearcher?.institution) {
        analyses = analyses.filter(analysis =>
          analysis.institutions?.some(inst => inst.id === selectedResearcher.institution?.id)
        );
      }
    }

    if (this.selectedKeyword) {
      const keywordId = FilterUtils.parseId(this.selectedKeyword);
      if (keywordId !== null) {
        const keywordInstitutionIds = this.getInstitutionIdsByKeyword(keywordId);
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

    // Apply ALL filters with AND logic (intersection)
    if (this.selectedInstitution) {
      const institutionId = FilterUtils.parseId(this.selectedInstitution);
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      
      if (selectedInstitution?.instruments && selectedInstitution.instruments.length > 0) {
        const institutionInstrumentIds = this.getRelatedIds(selectedInstitution.instruments);
        instruments = instruments.filter(inst => institutionInstrumentIds.includes(inst.id));
      } else {
        // If institution has no instruments, return empty
        return [];
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = FilterUtils.parseId(this.selectedInstrument);
      instruments = instruments.filter(inst => inst.id === instrumentId);
    }

    if (this.selectedAnalysis) {
      const analysisId = FilterUtils.parseId(this.selectedAnalysis);
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      const analysisInstitutionIds = this.getRelatedIds(selectedAnalysis?.institutions);
      
      const analysisInstitutionInstruments = this.allInstitutions
        .filter(inst => analysisInstitutionIds.includes(inst.id))
        .flatMap(inst => inst.instruments || []);
      const analysisInstrumentIds = this.getRelatedIds(analysisInstitutionInstruments);
      
      instruments = instruments.filter(inst => analysisInstrumentIds.includes(inst.id));
    }

    if (this.selectedMicroorganism) {
      const microorganismId = FilterUtils.parseId(this.selectedMicroorganism);
      if (microorganismId !== null) {
        const microorganismInstitutionIds = this.getInstitutionIdsByMicroorganism(microorganismId);
        
        const microorganismInstitutionInstruments = this.allInstitutions
          .filter(inst => microorganismInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.instruments || []);
        const microorganismInstrumentIds = this.getRelatedIds(microorganismInstitutionInstruments);
        
        instruments = instruments.filter(inst => microorganismInstrumentIds.includes(inst.id));
      }
    }

    if (this.selectedResearcher) {
      const researcherId = FilterUtils.parseId(this.selectedResearcher);
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      
      if (selectedResearcher?.institution) {
        const researcherInstitution = this.allInstitutions.find(
          inst => inst.id === selectedResearcher.institution?.id
        );
        const researcherInstrumentIds = this.getRelatedIds(researcherInstitution?.instruments);
        instruments = instruments.filter(inst => researcherInstrumentIds.includes(inst.id));
      }
    }

    if (this.selectedKeyword) {
      const keywordId = FilterUtils.parseId(this.selectedKeyword);
      if (keywordId !== null) {
        const keywordInstitutionIds = this.getInstitutionIdsByKeyword(keywordId);
        
        const keywordInstitutionInstruments = this.allInstitutions
          .filter(inst => keywordInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.instruments || []);
        const keywordInstrumentIds = this.getRelatedIds(keywordInstitutionInstruments);
        
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
      const keywordId = FilterUtils.parseId(this.selectedKeyword);
      keywords = keywords.filter(kw => kw.id === keywordId);
    }

    if (this.selectedResearcher) {
      const researcherId = FilterUtils.parseId(this.selectedResearcher);
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      
      if (selectedResearcher?.keywords && selectedResearcher.keywords.length > 0) {
        const researcherKeywordIds = this.getRelatedIds(selectedResearcher.keywords);
        keywords = keywords.filter(kw => researcherKeywordIds.includes(kw.id));
      }
    }

    if (this.selectedInstitution) {
      const institutionId = FilterUtils.parseId(this.selectedInstitution);
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      
      if (selectedInstitution && institutionId !== null) {
        if (selectedInstitution.keywords && selectedInstitution.keywords.length > 0) {
          // Use keywords directly associated with the institution
          const institutionKeywordIds = this.getRelatedIds(selectedInstitution.keywords);
          keywords = keywords.filter(kw => institutionKeywordIds.includes(kw.id));
        } else {
          // Fallback: find keywords through researchers at this institution
          const institutionResearcherIds = this.getResearcherIdsByInstitution(institutionId);
          const institutionKeywordIds = this.allKeywords
            .filter(kw => kw.researchers?.some(res => institutionResearcherIds.includes(res.id)))
            .map(kw => kw.id);
          keywords = keywords.filter(kw => institutionKeywordIds.includes(kw.id));
        }
      }
    }

    if (this.selectedAnalysis) {
      const analysisId = FilterUtils.parseId(this.selectedAnalysis);
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      const analysisInstitutionIds = this.getRelatedIds(selectedAnalysis?.institutions);
      
      const analysisResearcherIds = this.allResearchers
        .filter(res => analysisInstitutionIds.includes(res.institution?.id || 0))
        .map(res => res.id);
      
      const analysisKeywordIds = this.allKeywords
        .filter(kw => kw.researchers?.some(res => analysisResearcherIds.includes(res.id)))
        .map(kw => kw.id);
      
      keywords = keywords.filter(kw => analysisKeywordIds.includes(kw.id));
    }

    if (this.selectedMicroorganism) {
      const microorganismId = FilterUtils.parseId(this.selectedMicroorganism);
      if (microorganismId !== null) {
        const microorganismInstitutionIds = this.getInstitutionIdsByMicroorganism(microorganismId);
        
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
      const instrumentId = FilterUtils.parseId(this.selectedInstrument);
      if (instrumentId !== null) {
        const instrumentInstitutionIds = this.getInstitutionIdsByInstrument(instrumentId);
        
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
      const researcherId = FilterUtils.parseId(this.selectedResearcher);
      researchers = researchers.filter(res => res.id === researcherId);
    }

    if (this.selectedInstitution) {
      const institutionId = FilterUtils.parseId(this.selectedInstitution);
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      
      if (selectedInstitution) {
        if (selectedInstitution.employees && selectedInstitution.employees.length > 0) {
          // Use employees directly associated with the institution
          const institutionEmployeeIds = this.getRelatedIds(selectedInstitution.employees);
          researchers = researchers.filter(researcher => institutionEmployeeIds.includes(researcher.id));
        } else {
          // Fallback: filter by institution relationship
          researchers = researchers.filter(researcher => researcher.institution?.id === institutionId);
        }
      }
    }

    if (this.selectedKeyword) {
      const keywordId = FilterUtils.parseId(this.selectedKeyword);
      const selectedKeyword = this.allKeywords.find(kw => kw.id === keywordId);
      
      if (selectedKeyword) {
        researchers = researchers.filter(researcher =>
          researcher.keywords?.some(kw => kw.id === keywordId)
        );
      }
    }

    if (this.selectedAnalysis) {
      const analysisId = FilterUtils.parseId(this.selectedAnalysis);
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      const analysisInstitutionIds = this.getRelatedIds(selectedAnalysis?.institutions);
      
      researchers = researchers.filter(researcher =>
        analysisInstitutionIds.includes(researcher.institution?.id || 0)
      );
    }

    if (this.selectedMicroorganism) {
      const microorganismId = FilterUtils.parseId(this.selectedMicroorganism);
      if (microorganismId !== null) {
        const microorganismInstitutionIds = this.getInstitutionIdsByMicroorganism(microorganismId);
        researchers = researchers.filter(researcher =>
          microorganismInstitutionIds.includes(researcher.institution?.id || 0)
        );
      }
    }

    if (this.selectedInstrument) {
      const instrumentId = FilterUtils.parseId(this.selectedInstrument);
      if (instrumentId !== null) {
        const instrumentInstitutionIds = this.getInstitutionIdsByInstrument(instrumentId);
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

    // Apply ALL filters with AND logic (intersection)
    if (this.selectedInstitution) {
      const institutionId = FilterUtils.parseId(this.selectedInstitution);
      const selectedInstitution = this.allInstitutions.find(inst => inst.id === institutionId);
      
      if (selectedInstitution?.microorganisms && selectedInstitution.microorganisms.length > 0) {
        const institutionMicroorganismIds = this.getRelatedIds(selectedInstitution.microorganisms);
        microorganisms = microorganisms.filter(micro => institutionMicroorganismIds.includes(micro.id));
      } else {
        // If institution has no microorganisms, return empty
        return [];
      }
    }

    if (this.selectedMicroorganism) {
      const microorganismId = FilterUtils.parseId(this.selectedMicroorganism);
      microorganisms = microorganisms.filter(micro => micro.id === microorganismId);
    }

    if (this.selectedAnalysis) {
      const analysisId = FilterUtils.parseId(this.selectedAnalysis);
      const selectedAnalysis = this.allAnalyses.find(anal => anal.id === analysisId);
      const analysisMicroorganismIds = this.getRelatedIds(selectedAnalysis?.microorganisms);
      microorganisms = microorganisms.filter(micro => analysisMicroorganismIds.includes(micro.id));
    }

    if (this.selectedInstrument) {
      const instrumentId = FilterUtils.parseId(this.selectedInstrument);
      if (instrumentId !== null) {
        const instrumentInstitutionIds = this.getInstitutionIdsByInstrument(instrumentId);
        
        const instrumentInstitutionMicroorganismIds = this.allInstitutions
          .filter(inst => instrumentInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.microorganisms || [])
          .map(micro => micro.id);
        
        microorganisms = microorganisms.filter(micro =>
          instrumentInstitutionMicroorganismIds.includes(micro.id)
        );
      }
    }

    if (this.selectedResearcher) {
      const researcherId = FilterUtils.parseId(this.selectedResearcher);
      const selectedResearcher = this.allResearchers.find(res => res.id === researcherId);
      
      if (selectedResearcher?.institution) {
        const researcherInstitution = this.allInstitutions.find(
          inst => inst.id === selectedResearcher.institution?.id
        );
        const researcherMicroorganismIds = this.getRelatedIds(researcherInstitution?.microorganisms);
        microorganisms = microorganisms.filter(micro => researcherMicroorganismIds.includes(micro.id));
      }
    }

    if (this.selectedKeyword) {
      const keywordId = FilterUtils.parseId(this.selectedKeyword);
      if (keywordId !== null) {
        const keywordInstitutionIds = this.getInstitutionIdsByKeyword(keywordId);
        
        const keywordInstitutionMicroorganismIds = this.allInstitutions
          .filter(inst => keywordInstitutionIds.includes(inst.id))
          .flatMap(inst => inst.microorganisms || [])
          .map(micro => micro.id);
        
        microorganisms = microorganisms.filter(micro =>
          keywordInstitutionMicroorganismIds.includes(micro.id)
        );
      }
    }

    return microorganisms;
  }

  hasActiveFilters(): boolean {
    return !!(
      this.selectedInstitution ||
      this.selectedAnalysis ||
      this.selectedMicroorganism ||
      this.selectedInstrument ||
      this.selectedResearcher ||
      this.selectedKeyword
    );
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
    this.updateFilteredOptions();
    this.updateResults();
  }

  private showDetails(item: any, type: string): void {
    this.selectedItem = item;
    this.modalTitle = this.getDisplayName(item);
    this.modalType = type;
    this.showModal = true;
  }

  showInstitutionDetails = (institution: Institution) => this.showDetails(institution, 'institution');
  showResearcherDetails = (researcher: Employee) => this.showDetails(researcher, 'researcher');
  showAnalysisDetails = (analysis: Analysis) => this.showDetails(analysis, 'analysis');
  showKeywordDetails = (keyword: Keyword) => this.showDetails(keyword, 'keyword');
  showMicroorganismDetails = (microorganism: Microorganism) => this.showDetails(microorganism, 'microorganism');
  showInstrumentDetails = (instrument: Instrument) => this.showDetails(instrument, 'instrument');

  closeModal(): void {
    this.showModal = false;
    this.selectedItem = null;
    this.modalTitle = '';
    this.modalType = '';
  }

  getDisplayName(item: any): string {
    if (item.name) return item.name;
    if (item.firstName && item.lastName)
      return `${item.firstName} ${item.lastName}`;
    return 'Unknown';
  }

  getEntityNames(entities: any[] | undefined, nameMapper: (entity: any) => string = (e) => e.name): string {
    return entities?.map(nameMapper).join(', ') || '';
  }

  getInstitutionNames = (institutions: Institution[] | undefined) => this.getEntityNames(institutions);
  getKeywordNames = (keywords: Keyword[] | undefined) => this.getEntityNames(keywords);
  getResearcherNames = (researchers: Employee[] | undefined) => this.getEntityNames(researchers, (r) => `${r.firstName} ${r.lastName}`);
  getMicroorganismNames = (microorganisms: Microorganism[] | undefined) => this.getEntityNames(microorganisms);

  getMicroorganismInstitutions(microorganism: Microorganism): string {
    const institutions = this.allInstitutions.filter((inst) =>
      inst.microorganisms?.some((micro) => micro.id === microorganism.id)
    );
    return institutions.map((inst) => inst.name).join(', ') || '';
  }

  getMicroorganismAnalyses(microorganism: Microorganism): string {
    const analyses = this.allAnalyses.filter((analysis) =>
      analysis.microorganisms?.some((micro) => micro.id === microorganism.id)
    );
    return analyses.map((analysis) => analysis.name).join(', ') || '';
  }

  getInstrumentInstitutions(instrument: Instrument): string {
    const institutions = this.allInstitutions.filter((inst) =>
      inst.instruments?.some(
        (instInstrument) => instInstrument.id === instrument.id
      )
    );
    return institutions.map((inst) => inst.name).join(', ') || '';
  }

  getGreeting(): string {
    const hour = this.currentDate.getHours();
    if (hour < 12) return 'Good Morning';
    if (hour < 17) return 'Good Afternoon';
    return 'Good Evening';
  }

  onLogout(): void {
    this.authentificationFacadeService.logout();
    this.router.navigate(['/identity/login']);
  }

  public isPM(): boolean {
    return this.userRoles.some(role => role.toLowerCase() === 'pm');
  }

  public navigateToAdminManagement(): void {
    this.router.navigate(['/admin-management']);
  }

  public canManageDatabase(): boolean {
    return this.userRoles.some(role => role.toLowerCase() === 'administrator' || role.toLowerCase() === 'pm');
  }

  public canViewAnalytics(): boolean {
    return this.userRoles.some(role => 
      role.toLowerCase() === 'administrator' || 
      role.toLowerCase() === 'pm' || 
      role.toLowerCase() === 'guest'
    );
  }

  public toggleAnalytics(): void {
    this.showAnalytics = !this.showAnalytics;
    
    if (this.showAnalytics) {
      // Fetch fresh data every time the analytics window is opened
      this.loadAnalyticsData();
    } else {
      // Clear breakdown data when closing analytics panel
      this.clearAnalyticsBreakdown();
    }
  }

  /**
   * Clear institution breakdown selection
   * Called when analytics panel is closed or when breakdown needs to be reset
   */
  private clearAnalyticsBreakdown(): void {
    this.selectedInstitutionId = null;
    this.institutionBreakdownData = null;
  }

  private loadAnalyticsData(): void {
    if (this.canViewAnalytics()) {
      this.analyticsService.getAnalyticsSummary()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (data) => {
            this.analyticsData = data;
          },
          error: (error) => {
            console.error('Error loading analytics data:', error);
          }
        });
      
      this.loadTopInstitutions();
    }
  }

  private loadTopInstitutions(): void {
    this.analyticsService.getTopInstitutions(10)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.topInstitutionsData = data;
        },
        error: (error) => {
          console.error('Error loading top institutions:', error);
        }
      });
  }

 
  private refreshAnalyticsData(): void {
    if (this.canViewAnalytics() && this.showAnalytics) {
      // Reload summary analytics
      this.analyticsService.getAnalyticsSummary()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (data) => {
            this.analyticsData = data;
          },
          error: (error) => {
            console.error('Error refreshing analytics data:', error);
          }
        });
      
      // Reload top institutions
      this.loadTopInstitutions();
      
      // Reload institution breakdown if one is currently selected
      if (this.selectedInstitutionId !== null) {
        this.loadInstitutionBreakdown(this.selectedInstitutionId);
      }
    }
  }

  public loadInstitutionBreakdown(institutionId: number): void {
    this.selectedInstitutionId = institutionId;
    this.analyticsService.getInstitutionBreakdown(institutionId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.institutionBreakdownData = data;
        },
        error: (error) => {
          console.error('Error loading institution breakdown:', error);
          this.institutionBreakdownData = null;
        }
      });
  }

  public createInstitution(): void {
    if (!this.newInstitutionName || !this.newInstitutionCity || !this.newInstitutionStreet || !this.newInstitutionCountry || !this.newInstitutionStreetNumber) return;
    
    this.handleCrudOperation(
      this.dataService.createInstitution(this.newInstitutionName, this.newInstitutionDesc, this.newInstitutionCity, this.newInstitutionStreet, this.newInstitutionCountry, this.newInstitutionStreetNumber),
      () => {
        this.newInstitutionName = '';
        this.newInstitutionDesc = '';
        this.newInstitutionCity = '';
        this.newInstitutionStreet = '';
        this.newInstitutionCountry = '';
        this.newInstitutionStreetNumber = '';
      }
    );
  }

  public deleteInstitution(name: string): void {
    this.handleCrudOperation(this.dataService.deleteInstitution(name));
  }

  public createInstrument(): void {
    if (!this.newInstrumentName) return;
    
    this.handleCrudOperation(
      this.dataService.createInstrument(this.newInstrumentName, this.newInstrumentDesc),
      () => {
        this.newInstrumentName = '';
        this.newInstrumentDesc = '';
      }
    );
  }

  public deleteInstrument(name: string): void {
    this.handleCrudOperation(this.dataService.deleteInstrument(name));
  }

  public createKeyword(): void {
    if (!this.newKeywordName) return;
    
    this.handleCrudOperation(
      this.dataService.createKeyword(this.newKeywordName),
      () => { this.newKeywordName = ''; }
    );
  }

  public deleteKeyword(name: string): void {
    this.handleCrudOperation(this.dataService.deleteKeyword(name));
  }

  public createAnalysis(): void {
    if (!this.newAnalysisName) return;
    
    this.handleCrudOperation(
      this.dataService.createAnalysis(this.newAnalysisName, this.newAnalysisDesc),
      () => {
        this.newAnalysisName = '';
        this.newAnalysisDesc = '';
      }
    );
  }

  public deleteAnalysis(name: string): void {
    this.handleCrudOperation(this.dataService.deleteAnalysis(name));
  }

  public createEmployee(): void {
    if (!this.newEmployeeFirstName || !this.newEmployeeLastName || !this.newEmployeeEmail || !this.newEmployeeInstitutionId) return;
    
    this.handleCrudOperation(
      this.dataService.createEmployee(this.newEmployeeFirstName, this.newEmployeeLastName, this.newEmployeeEmail, this.newEmployeeInstitutionId),
      () => {
        this.newEmployeeFirstName = '';
        this.newEmployeeLastName = '';
        this.newEmployeeEmail = '';
        this.newEmployeeInstitutionId = null;
      }
    );
  }

  public deleteEmployee(id: number): void {
    this.handleCrudOperation(this.dataService.deleteEmployee(id));
  }

  public createMicroorganism(): void {
    if (!this.newMicroorganismName) return;
    
    this.handleCrudOperation(
      this.dataService.createMicroorganism(this.newMicroorganismName),
      () => { this.newMicroorganismName = ''; }
    );
  }

  public deleteMicroorganism(name: string): void {
    this.handleCrudOperation(this.dataService.deleteMicroorganism(name));
  }

  public connectInstitutionToAnalysis(): void {
    if (!this.selectedConnectionInstitution || !this.selectedConnectionAnalysis) return;
    this.connect(
      this.dataService.connectInstitutionToAnalysis(this.selectedConnectionInstitution, this.selectedConnectionAnalysis),
      () => { this.selectedConnectionInstitution = null; this.selectedConnectionAnalysis = null; }
    );
  }

  public connectInstitutionToInstrument(): void {
    if (!this.selectedConnectionInstitution || !this.selectedConnectionInstrument) return;
    this.connect(
      this.dataService.connectInstitutionToInstrument(this.selectedConnectionInstitution, this.selectedConnectionInstrument),
      () => { this.selectedConnectionInstitution = null; this.selectedConnectionInstrument = null; }
    );
  }

  public connectInstitutionToMicroorganism(): void {
    if (!this.selectedConnectionInstitution || !this.selectedConnectionMicroorganism) return;
    this.connect(
      this.dataService.connectInstitutionToMicroorganism(this.selectedConnectionInstitution, this.selectedConnectionMicroorganism),
      () => { this.selectedConnectionInstitution = null; this.selectedConnectionMicroorganism = null; }
    );
  }

  public connectResearcherToKeyword(): void {
    if (!this.selectedConnectionResearcher || !this.selectedConnectionKeyword) return;
    this.connect(
      this.dataService.connectResearcherToKeyword(this.selectedConnectionResearcher, this.selectedConnectionKeyword),
      () => { this.selectedConnectionResearcher = null; this.selectedConnectionKeyword = null; }
    );
  }

  public connectAnalysisToMicroorganism(): void {
    if (!this.selectedConnectionAnalysis || !this.selectedConnectionMicroorganism) return;
    this.connect(
      this.dataService.connectAnalysisToMicroorganism(this.selectedConnectionAnalysis, this.selectedConnectionMicroorganism),
      () => { this.selectedConnectionAnalysis = null; this.selectedConnectionMicroorganism = null; }
    );
  }

  public connectAnalysisToInstrument(): void {
    if (!this.selectedConnectionAnalysis || !this.selectedConnectionInstrument) return;
    this.connect(
      this.dataService.connectAnalysisToInstrument(this.selectedConnectionAnalysis, this.selectedConnectionInstrument),
      () => { this.selectedConnectionAnalysis = null; this.selectedConnectionInstrument = null; }
    );
  }
}

