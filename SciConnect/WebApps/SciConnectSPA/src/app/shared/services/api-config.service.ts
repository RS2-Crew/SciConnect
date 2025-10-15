import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  private readonly baseUrl = 'http://localhost:4001/api/v1'; // DB API runs on port 4001

  getBaseUrl(): string {
    return this.baseUrl;
  }

  getDbApiUrl(): string {
    return `${this.baseUrl}/db`;
  }

  getIdentityApiUrl(): string {
    return `http://localhost:4000/api/v1/identity`; // Identity service runs on port 4000
  }

  getAnalyticsApiUrl(): string {
    return `http://localhost:4002/api/v1/analytics`; // Analytics service runs on port 4002
  }
}
