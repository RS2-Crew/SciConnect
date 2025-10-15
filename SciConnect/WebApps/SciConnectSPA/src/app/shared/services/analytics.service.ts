import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppStateService } from '../app-state/app-state.service';
import { ApiConfigService } from './api-config.service';

export interface SummaryAnalyticsResponse {
  totalInstitutions: number;
  totalAnalyses: number;
  totalResearchers: number;
  totalInstruments: number;
  totalKeywords: number;
  totalMicroorganisms: number;
}

export interface InstitutionBreakdownResponse {
  institutionId: number;
  institutionName: string;
  totalAnalyses: number;
  totalResearchers: number;
  totalInstruments: number;
}

export interface TopInstitutionResponse {
  institutionId: number;
  institutionName: string;
  analysisCount: number;
}

export interface DetailedAnalyticsResponse {
  entityType: string;
  entityName: string;
  count: number;
}

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private readonly baseUrl: string;

  constructor(
    private http: HttpClient,
    private appStateService: AppStateService,
    private apiConfigService: ApiConfigService
  ) {
    this.baseUrl = this.apiConfigService.getAnalyticsApiUrl();
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

  getAnalyticsSummary(): Observable<SummaryAnalyticsResponse> {
    return this.http.get<SummaryAnalyticsResponse>(`${this.baseUrl}/summary`, { headers: this.getHeaders() });
  }

  getInstitutionBreakdown(institutionId: number): Observable<InstitutionBreakdownResponse> {
    return this.http.get<InstitutionBreakdownResponse>(`${this.baseUrl}/institution/${institutionId}/breakdown`, { headers: this.getHeaders() });
  }

  getTopInstitutions(limit: number = 5): Observable<TopInstitutionResponse[]> {
    return this.http.get<TopInstitutionResponse[]>(`${this.baseUrl}/institutions/top?limit=${limit}`, { headers: this.getHeaders() });
  }

  getDetailedAnalytics(entityType: string): Observable<DetailedAnalyticsResponse[]> {
    return this.http.get<DetailedAnalyticsResponse[]>(`${this.baseUrl}/detailed/${entityType}`, { headers: this.getHeaders() });
  }
}
