import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable, Inject, PLATFORM_ID } from "@angular/core";
import { isPlatformBrowser } from "@angular/common";
import { ILoginRequest } from "../models/login-request";
import { Observable, switchMap, take } from "rxjs";
import { ILoginResponse } from "../models/login-response";
import { IRegisterRequest } from "../models/register-request";
import { AppStateService } from "../../../shared/app-state/app-state.service";

@Injectable({
  providedIn: 'root'
  }
)
export class AuthentificationService {
  private readonly url: string;

  constructor(
    private httpClient: HttpClient, 
    private appStateService: AppStateService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      const config = (window as any).__env || {};
      const baseUrl = config.API_IDENTITY_URL || 'http://localhost:4000/api/v1/identity';
      // Replace /identity with /authentication for auth endpoints
      this.url = baseUrl.replace('/identity', '/authentication');
    } else {
      this.url = 'http://localhost:4000/api/v1/authentication';
    }
  }

  public login(request: ILoginRequest): Observable<ILoginResponse> {
    return this.httpClient.post<ILoginResponse>(`${this.url}/login`, request);
  }

  public registerGuest(request: IRegisterRequest): Observable<any> {
    return this.httpClient.post(`${this.url}/RegisterGuest`, request);
  }

  public registerAdministrator(request: IRegisterRequest): Observable<any> {
    return this.httpClient.post(`${this.url}/RegisterAdministrator`, request);
  }

  public registerPM(request: IRegisterRequest): Observable<any> {
    return this.httpClient.post(`${this.url}/RegisterPM`, request);
  }

  public requestAdminRegistration(email: string): Observable<any> {
    return this.httpClient.post(`${this.url}/RequestAdminRegistration`, JSON.stringify(email), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  public generateVerificationCode(email: string): Observable<any> {
    return this.appStateService.getAppState().pipe(
      take(1),
      switchMap(appState => {
        const headers = new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${appState.accessToken}`
        });
        
        return this.httpClient.post(`${this.url}/GenerateVerificationCode`, JSON.stringify(email), {
          headers: headers
        });
      })
    );
  }

  public logout(userName: string, refreshToken: string): Observable<any> {
    return this.appStateService.getAppState().pipe(
      take(1),
      switchMap(appState => {
        const headers = new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${appState.accessToken}`
        });
        
        return this.httpClient.post(`${this.url}/Logout`, {
          userName: userName,
          refreshToken: refreshToken
        }, { headers: headers });
      })
    );
  }
}




