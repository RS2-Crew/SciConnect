import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, forkJoin, of } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';
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
    let token = '';
    this.appStateService.getAppState().subscribe(state => {
      token = state.accessToken || '';
    }).unsubscribe();
    
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getAllInstitutions(): Observable<Institution[]> {
    return this.http.get<InstitutionViewModel[]>(`${this.baseUrl}/institutions`, { headers: this.getHeaders() })
      .pipe(map(institutions => institutions as Institution[]));
  }

  getAllInstitutionsWithRelatedData(): Observable<Institution[]> {
    return this.getAllInstitutions().pipe(
      switchMap(institutions => {
        if (institutions.length === 0) {
          return of([]);
        }
        
        const institutionRequests = institutions.map(institution => 
          forkJoin({
            withAnalyses: this.getInstitutionWithAnalyses(institution.name).pipe(
              catchError(() => of(institution))
            ),
            withInstruments: this.getInstitutionWithInstruments(institution.name).pipe(
              catchError(() => of(institution))
            ),
            withEmployees: this.getInstitutionWithEmployees(institution.name).pipe(
              catchError(() => of(institution))
            ),
            withKeywords: this.getInstitutionWithKeywords(institution.name).pipe(
              catchError(() => of(institution))
            ),
            withMicroorganisms: this.getInstitutionWithMicroorganisms(institution.name).pipe(
              catchError(() => of(institution))
            )
          }).pipe(
            map(relatedData => {
              return {
                ...institution,
                analyses: relatedData.withAnalyses.analyses || [],
                instruments: relatedData.withInstruments.instruments || [],
                employees: relatedData.withEmployees.employees || [],
                keywords: relatedData.withKeywords.keywords || [],
                microorganisms: relatedData.withMicroorganisms.microorganisms || []
              };
            })
          )
        );
        
        return forkJoin(institutionRequests);
      })
    );
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

  getAllAnalyses(): Observable<Analysis[]> {
    return this.http.get<AnalysisViewModel[]>(`${this.baseUrl}/analyses`, { headers: this.getHeaders() })
      .pipe(map(analyses => analyses as Analysis[]));
  }

  getAllAnalysesWithRelatedData(): Observable<Analysis[]> {
    return this.getAllAnalyses().pipe(
      switchMap(analyses => {
        if (analyses.length === 0) {
          return of([]);
        }
        
        const analysisRequests = analyses.map(analysis => 
          forkJoin({
            institutions: this.getAnalysisWithInstitutions(analysis.name).pipe(
              catchError(() => of({ institutions: [] }))
            ),
            microorganisms: this.getAnalysisWithMicroorganisms(analysis.name).pipe(
              catchError(() => of({ microorganisms: [] }))
            )
          }).pipe(
            map(relatedData => ({
              ...analysis,
              institutions: relatedData.institutions.institutions || [],
              microorganisms: relatedData.microorganisms.microorganisms || []
            }))
          )
        );
        
        return forkJoin(analysisRequests);
      })
    );
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

  getAllInstruments(): Observable<Instrument[]> {
    return this.http.get<InstrumentViewModel[]>(`${this.baseUrl}/instruments`, { headers: this.getHeaders() })
      .pipe(map(instruments => instruments as Instrument[]));
  }

  getAllInstrumentsWithRelatedData(): Observable<Instrument[]> {
    return this.getAllInstruments().pipe(
      switchMap(instruments => {
        if (instruments.length === 0) {
          return of([]);
        }
        
        const instrumentRequests = instruments.map(instrument => 
          this.getInstitutionsByInstrument(instrument.name).pipe(
            catchError(() => of([])),
            map(institutions => ({
              ...instrument,
              institutions: institutions || []
            }))
          )
        );
        
        return forkJoin(instrumentRequests);
      })
    );
  }

  getInstrumentByName(name: string): Observable<Instrument[]> {
    return this.http.get<InstrumentViewModel[]>(`${this.baseUrl}/instruments/${name}`, { headers: this.getHeaders() })
      .pipe(map(instruments => instruments as Instrument[]));
  }

  getInstitutionsByInstrument(instrumentName: string): Observable<Institution[]> {
    return this.http.get<Institution[]>(`${this.baseUrl}/instrument/with-institutions/${instrumentName}`, { headers: this.getHeaders() });
  }

  getAllKeywords(): Observable<Keyword[]> {
    return this.http.get<KeywordViewModel[]>(`${this.baseUrl}/keywords`, { headers: this.getHeaders() })
      .pipe(map(keywords => keywords as Keyword[]));
  }

  getAllKeywordsWithRelatedData(): Observable<Keyword[]> {
    return this.getAllKeywords().pipe(
      switchMap(keywords => {
        if (keywords.length === 0) {
          return of([]);
        }
        
        const keywordRequests = keywords.map(keyword => 
          this.getEmployeesByKeyword(keyword.name).pipe(
            catchError(() => of([])),
            map(researchers => ({
              ...keyword,
              researchers: researchers || []
            }))
          )
        );
        
        return forkJoin(keywordRequests);
      })
    );
  }

  getKeywordByName(name: string): Observable<Keyword[]> {
    return this.http.get<KeywordViewModel[]>(`${this.baseUrl}/keywords/${name}`, { headers: this.getHeaders() })
      .pipe(map(keywords => keywords as Keyword[]));
  }

  getEmployeesByKeyword(keywordName: string): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.baseUrl}/keywords/${keywordName}/employees`, { headers: this.getHeaders() });
  }

  getAllEmployees(): Observable<Employee[]> {
    return this.http.get<EmployeeViewModel[]>(`${this.baseUrl}/employees`, { headers: this.getHeaders() })
      .pipe(map(employees => employees as Employee[]));
  }

  getAllEmployeesWithRelatedData(): Observable<Employee[]> {
    return this.getAllEmployees().pipe(
      switchMap(employees => {
        if (employees.length === 0) {
          return of([]);
        }
        
        const employeeRequests = employees.map(employee => 
          forkJoin({
            institution: this.getEmployeeWithInstitution(employee.firstName, employee.lastName).pipe(
              catchError(() => of({ institution: null }))
            ),
            keywords: this.getEmployeeWithKeywords(employee.firstName, employee.lastName).pipe(
              catchError(() => of({ keywords: [] }))
            )
          }).pipe(
            map(relatedData => ({
              ...employee,
              institution: relatedData.institution.institution || employee.institution,
              keywords: relatedData.keywords.keywords || employee.keywords || []
            }))
          )
        );
        
        return forkJoin(employeeRequests);
      })
    );
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

  getAllMicroorganisms(): Observable<Microorganism[]> {
    return this.http.get<MicroorganismViewModel[]>(`${this.baseUrl}/microorganisms`, { headers: this.getHeaders() })
      .pipe(map(microorganisms => microorganisms as Microorganism[]));
  }

  getAllMicroorganismsWithRelatedData(): Observable<Microorganism[]> {
    return this.getAllMicroorganisms().pipe(
      switchMap(microorganisms => {
        if (microorganisms.length === 0) {
          return of([]);
        }
        
        const microorganismRequests = microorganisms.map(microorganism => 
          forkJoin({
            withAnalysis: this.getMicroorganismWithAnalysis(microorganism.name).pipe(
              catchError(() => of(microorganism))
            ),
            withInstitution: this.getMicroorganismWithInstitution(microorganism.name).pipe(
              catchError(() => of(microorganism))
            )
          }).pipe(
            map(relatedData => ({
              ...microorganism,
              analysis: relatedData.withAnalysis.analysis || [],
              institution: relatedData.withInstitution.institution || undefined
            }))
          )
        );
        
        return forkJoin(microorganismRequests);
      })
    );
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

  updateFilterState(filterState: Partial<FilterState>): void {
    const currentState = this.filterStateSubject.value;
    const newState = { ...currentState, ...filterState };
    this.filterStateSubject.next(newState);
  }

  getFilterState(): FilterState {
    return this.filterStateSubject.value;
  }

  getAnalysesForInstitution(institutionName: string): Observable<Analysis[]> {
    return this.getInstitutionWithAnalyses(institutionName)
      .pipe(
        map(institution => {
          return institution['analyses'] || [];
        })
      );
  }

  createInstitution(name: string, description: string, city: string, street: string, country: string, streetNumber: string): Observable<number> {
    const command = { name, description, city, street, country, streetNumber };
    return this.http.post<number>(`${this.baseUrl}/institutions`, command, { headers: this.getHeaders() });
  }

  deleteInstitution(name: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/institutions/${name}`, { headers: this.getHeaders() });
  }

  createInstrument(name: string, description: string): Observable<number> {
    const command = { name, description };
    return this.http.post<number>(`${this.baseUrl}/instruments`, command, { headers: this.getHeaders() });
  }

  deleteInstrument(name: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/instruments/${name}`, { headers: this.getHeaders() });
  }

  createKeyword(name: string): Observable<number> {
    const command = { name };
    return this.http.post<number>(`${this.baseUrl}/keywords`, command, { headers: this.getHeaders() });
  }

  deleteKeyword(name: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/keywords/${name}`, { headers: this.getHeaders() });
  }

  createEmployee(firstName: string, lastName: string, email: string, institutionId: number): Observable<number> {
    const command = { firstName, lastName, email, institutionId };
    return this.http.post<number>(`${this.baseUrl}/employees`, command, { headers: this.getHeaders() });
  }

  deleteEmployee(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/employees/${id}`, { headers: this.getHeaders() });
  }

  createAnalysis(name: string, description: string): Observable<number> {
    const command = { name, description };
    return this.http.post<number>(`${this.baseUrl}/analyses`, command, { headers: this.getHeaders() });
  }

  deleteAnalysis(name: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/analyses/${name}`, { headers: this.getHeaders() });
  }

  createMicroorganism(name: string): Observable<number> {
    const command = { name };
    return this.http.post<number>(`${this.baseUrl}/microorganisms`, command, { headers: this.getHeaders() });
  }

  deleteMicroorganism(name: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/microorganisms/${name}`, { headers: this.getHeaders() });
  }

  connectInstitutionToAnalysis(institutionId: number, analysisId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/institutions/${institutionId}/analysis/${analysisId}`, {}, { headers: this.getHeaders() })
      .pipe(
        catchError(error => {
          throw error;
        })
      );
  }

  disconnectInstitutionFromAnalysis(institutionId: number, analysisId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/institutions/${institutionId}/analysis/${analysisId}`, { headers: this.getHeaders() });
  }

  connectInstitutionToInstrument(institutionId: number, instrumentId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/institutions/${institutionId}/instruments/${instrumentId}`, {}, { headers: this.getHeaders() });
  }

  disconnectInstitutionFromInstrument(institutionId: number, instrumentId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/institutions/${institutionId}/instruments/${instrumentId}`, { headers: this.getHeaders() });
  }

  connectInstitutionToMicroorganism(institutionId: number, microorganismId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/institutions/${institutionId}/microorganism/${microorganismId}`, {}, { headers: this.getHeaders() });
  }

  disconnectInstitutionFromMicroorganism(institutionId: number, microorganismId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/institutions/${institutionId}/microorganism/${microorganismId}`, { headers: this.getHeaders() });
  }

  connectResearcherToKeyword(researcherId: number, keywordId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/employee/${researcherId}/keyword/${keywordId}`, {}, { headers: this.getHeaders() });
  }

  disconnectResearcherFromKeyword(researcherId: number, keywordId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/employee/${researcherId}/keyword/${keywordId}`, { headers: this.getHeaders() });
  }

  connectAnalysisToMicroorganism(analysisId: number, microorganismId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/analysis/${analysisId}/microorganism/${microorganismId}`, {}, { headers: this.getHeaders() });
  }

  disconnectAnalysisFromMicroorganism(analysisId: number, microorganismId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/analysis/${analysisId}/microorganism/${microorganismId}`, { headers: this.getHeaders() });
  }

  connectAnalysisToInstrument(analysisId: number, instrumentId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/analysis/${analysisId}/instrument/${instrumentId}`, {}, { headers: this.getHeaders() });
  }

  disconnectAnalysisFromInstrument(analysisId: number, instrumentId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/analysis/${analysisId}/instrument/${instrumentId}`, { headers: this.getHeaders() });
  }
}
