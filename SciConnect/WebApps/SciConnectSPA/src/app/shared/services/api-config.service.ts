import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  private readonly dbApiUrl: string;
  private readonly identityApiUrl: string;
  private readonly analyticsApiUrl: string;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    if (isPlatformBrowser(this.platformId)) {
      // Read from window object (injected at runtime)
      const config = (window as any).__env || {};
      this.dbApiUrl = config.API_DB_URL || 'http://localhost:4001/api/v1/db';
      this.identityApiUrl = config.API_IDENTITY_URL || 'http://localhost:4000/api/v1/identity';
      this.analyticsApiUrl = config.API_ANALYTICS_URL || 'http://localhost:4002/api/v1/analytics';
    } else {
      // SSR fallback
      this.dbApiUrl = 'http://localhost:4001/api/v1/db';
      this.identityApiUrl = 'http://localhost:4000/api/v1/identity';
      this.analyticsApiUrl = 'http://localhost:4002/api/v1/analytics';
    }
  }

  getBaseUrl(): string {
    return this.dbApiUrl.replace('/db', '');
  }

  getDbApiUrl(): string {
    return this.dbApiUrl;
  }

  getIdentityApiUrl(): string {
    return this.identityApiUrl;
  }

  getAnalyticsApiUrl(): string {
    return this.analyticsApiUrl;
  }
}
