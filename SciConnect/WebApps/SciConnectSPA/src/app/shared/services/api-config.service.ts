import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  private readonly baseUrl = '/api/v1';

  getBaseUrl(): string {
    return this.baseUrl;
  }

  getDbApiUrl(): string {
    return this.baseUrl;
  }

  getIdentityApiUrl(): string {
    return '/identity/v1';
  }

  getAnalyticsApiUrl(): string {
    return '/analytics/v1';
  }
}
