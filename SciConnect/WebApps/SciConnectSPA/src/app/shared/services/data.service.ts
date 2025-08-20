import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { 
  Institution, 
  Analysis, 
  Instrument, 
  Keyword, 
  Employee, 
  Microorganism,
  InstitutionViewModel,
  AnalysisViewModel,
  InstrumentViewModel,
  KeywordViewModel,
  EmployeeViewModel,
  MicroorganismViewModel,
  FilterState
} from '../models/data-models';
import { AppStateService } from '../app-state/app-state.service';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private readonly baseUrl: string;
  private filterStateSubject = new BehaviorSubject<FilterState>({
    selectedInstruments: [],
    selectedAnalyses: [],
    selectedKeywords: [],
    selectedEmployees: [],
    selectedMicroorganisms: []
  });

  public filterState$ = this.filterStateSubject.asObservable();

  constructor(
    private http: HttpClient,
    private appStateService: AppStateService,
    private apiConfigService: ApiConfigService
  ) {
    this.baseUrl = this.apiConfigService.getDbApiUrl();
  }

  private getHeaders(): HttpHeaders {
    const appState = this.appStateService.getAppState();
    let token = '';
    appState.subscribe(state => {
      token = state.accessToken || '';
    });
    
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  // Institutions
  getAllInstitutions(): Observable<Institution[]> {
    return this.http.get<InstitutionViewModel[]>(`${this.baseUrl}/institutions`, { headers: this.getHeaders() })
      .pipe(map(institutions => institutions as Institution[]));
  }

  getInstitutionByName(name: string): Observable<Institution[]> {
    return this.http.get<InstitutionViewModel[]>(`${this.baseUrl}/institutions/${name}`, { headers: this.getHeaders() })
      .pipe(map(institutions => institutions as Institution[]));
  }

  getInstitutionWithAnalyses(institutionName: string): Observable<Institution> {
    return this.http.get<Institution>(`${this.baseUrl}/institution/with-analyses/${institutionName}`, { headers: this.getHeaders() });
  }

  getInstitutionWithInstruments(institutionName: string): Observable<Institution> {
    return this.http.get<Institution>(`${this.baseUrl}/institution/with-instruments/${institutionName}`, { headers: this.getHeaders() });
  }

  getInstitutionWithEmployees(institutionName: string): Observable<Institution> {
    return this.http.get<Institution>(`${this.baseUrl}/institution/with-employees/${institutionName}`, { headers: this.getHeaders() });
  }

  getInstitutionWithKeywords(institutionName: string): Observable<Institution> {
    return this.http.get<Institution>(`${this.baseUrl}/institution/with-keywords/${institutionName}`, { headers: this.getHeaders() });
  }

  getInstitutionWithMicroorganisms(institutionName: string): Observable<Institution> {
    return this.http.get<Institution>(`${this.baseUrl}/institution/with-microorganisms/${institutionName}`, { headers: this.getHeaders() });
  }

  // Analyses
  getAllAnalyses(): Observable<Analysis[]> {
    return this.http.get<AnalysisViewModel[]>(`${this.baseUrl}/analyses`, { headers: this.getHeaders() })
      .pipe(map(analyses => analyses as Analysis[]));
  }

  getAnalysisByName(name: string): Observable<Analysis[]> {
    return this.http.get<AnalysisViewModel[]>(`${this.baseUrl}/analyses/${name}`, { headers: this.getHeaders() })
      .pipe(map(analyses => analyses as Analysis[]));
  }

  getAnalysisWithInstitutions(analysisName: string): Observable<Analysis> {
    return this.http.get<Analysis>(`${this.baseUrl}/analysis/with-institutions/${analysisName}`, { headers: this.getHeaders() });
  }

  getAnalysisWithMicroorganisms(analysisName: string): Observable<Analysis> {
    return this.http.get<Analysis>(`${this.baseUrl}/analysis/with-microorganisms/${analysisName}`, { headers: this.getHeaders() });
  }

  // Instruments
  getAllInstruments(): Observable<Instrument[]> {
    return this.http.get<InstrumentViewModel[]>(`${this.baseUrl}/instruments`, { headers: this.getHeaders() })
      .pipe(map(instruments => instruments as Instrument[]));
  }

  getInstrumentByName(name: string): Observable<Instrument[]> {
    return this.http.get<InstrumentViewModel[]>(`${this.baseUrl}/instruments/${name}`, { headers: this.getHeaders() })
      .pipe(map(instruments => instruments as Instrument[]));
  }

  getInstitutionsByInstrument(instrumentName: string): Observable<Institution[]> {
    return this.http.get<Institution[]>(`${this.baseUrl}/instrument/with-institutions/${instrumentName}`, { headers: this.getHeaders() });
  }

  // Keywords
  getAllKeywords(): Observable<Keyword[]> {
    return this.http.get<KeywordViewModel[]>(`${this.baseUrl}/keywords`, { headers: this.getHeaders() })
      .pipe(map(keywords => keywords as Keyword[]));
  }

  getKeywordByName(name: string): Observable<Keyword[]> {
    return this.http.get<KeywordViewModel[]>(`${this.baseUrl}/keywords/${name}`, { headers: this.getHeaders() })
      .pipe(map(keywords => keywords as Keyword[]));
  }

  getEmployeesByKeyword(keywordName: string): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.baseUrl}/keywords/${keywordName}/employees`, { headers: this.getHeaders() });
  }

  // Employees
  getAllEmployees(): Observable<Employee[]> {
    return this.http.get<EmployeeViewModel[]>(`${this.baseUrl}/employees`, { headers: this.getHeaders() })
      .pipe(map(employees => employees as Employee[]));
  }

  getEmployeeByName(firstName: string, lastName: string): Observable<Employee[]> {
    return this.http.get<EmployeeViewModel[]>(`${this.baseUrl}/employees/${firstName}/${lastName}`, { headers: this.getHeaders() })
      .pipe(map(employees => employees as Employee[]));
  }

  getEmployeeWithInstitution(firstName: string, lastName: string): Observable<Employee> {
    return this.http.get<Employee>(`${this.baseUrl}/employee/with-institution/${firstName}/${lastName}`, { headers: this.getHeaders() });
  }

  getEmployeeWithKeywords(firstName: string, lastName: string): Observable<Employee> {
    return this.http.get<Employee>(`${this.baseUrl}/employee/with-keywords/${firstName}/${lastName}`, { headers: this.getHeaders() });
  }

  // Microorganisms
  getAllMicroorganisms(): Observable<Microorganism[]> {
    return this.http.get<MicroorganismViewModel[]>(`${this.baseUrl}/microorganisms`, { headers: this.getHeaders() })
      .pipe(map(microorganisms => microorganisms as Microorganism[]));
  }

  getMicroorganismByName(name: string): Observable<Microorganism[]> {
    return this.http.get<MicroorganismViewModel[]>(`${this.baseUrl}/microorganisms/${name}`, { headers: this.getHeaders() })
      .pipe(map(microorganisms => microorganisms as Microorganism[]));
  }

  getMicroorganismWithAnalysis(microorganismName: string): Observable<Microorganism> {
    return this.http.get<Microorganism>(`${this.baseUrl}/microorganism/with-analysis/${microorganismName}`, { headers: this.getHeaders() });
  }

  getMicroorganismWithInstitution(microorganismName: string): Observable<Microorganism> {
    return this.http.get<Microorganism>(`${this.baseUrl}/microorganism/with-institution/${microorganismName}`, { headers: this.getHeaders() });
  }

  // Filter state management
  updateFilterState(filterState: Partial<FilterState>): void {
    const currentState = this.filterStateSubject.value;
    const newState = { ...currentState, ...filterState };
    this.filterStateSubject.next(newState);
  }

  getFilterState(): FilterState {
    return this.filterStateSubject.value;
  }

  // Helper method to get analyses for a selected institution
  getAnalysesForInstitution(institutionName: string): Observable<Analysis[]> {
    return this.getInstitutionWithAnalyses(institutionName)
      .pipe(
        map(institution => {
          // This would need to be adjusted based on the actual response structure
          return institution['analyses'] || [];
        })
      );
  }
}
