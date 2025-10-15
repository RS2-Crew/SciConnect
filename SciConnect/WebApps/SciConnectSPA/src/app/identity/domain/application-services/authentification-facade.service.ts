import { Injectable } from "@angular/core";
import { AuthentificationService } from "../infrastructure/authentification.service";
import { catchError, map, Observable, of, switchMap } from "rxjs";
import { ILoginRequest } from "../models/login-request";
import { IRegisterRequest } from "../models/register-request";
import { AppStateService } from "../../../shared/app-state/app-state.service";
import { ILoginResponse } from "../models/login-response";
import { JwtService } from "../../../shared/jwt/jwt.service";
import { JwtPayloadKeys } from "../../../shared/jwt/jwt-payload-keys";

@Injectable({
  providedIn: 'root'
})
export class AuthentificationFacadeService {

  constructor(private authentificationService: AuthentificationService, private appStateService: AppStateService, private jwtService: JwtService
  ){}

  public login(username: string, password: string): Observable<boolean>{
    const request: ILoginRequest = {username: username, password: password};

    return this.authentificationService.login(request).pipe(
      switchMap((loginResponse: ILoginResponse) => {
        this.appStateService.setAccessToken(loginResponse.accessToken);
        this.appStateService.setRefreshToken(loginResponse.refreshToken);

        const payload = this.jwtService.parsePayload(loginResponse.accessToken);

        this.appStateService.setRoles(payload[JwtPayloadKeys.Role]);
        this.appStateService.setUserName(payload[JwtPayloadKeys.Username] || username);

        // Return an Observable that emits the username, then maps to true
        return of(payload[JwtPayloadKeys.Username] || username);
      }),
      map(() => {return true;}),
      catchError((err) => {
        //cistimo stanje aplikacije u slucaju greske.
        this.appStateService.clearAppState();
        return of(false);
      })
    );
  }

  public registerGuest(request: IRegisterRequest): Observable<boolean> {
    return this.authentificationService.registerGuest(request).pipe(
      map(() => true),
      catchError((err) => {
        console.error('Guest registration error:', err);
        return of(false);
      })
    );
  }

  public registerAdministrator(request: IRegisterRequest): Observable<boolean> {
    return this.authentificationService.registerAdministrator(request).pipe(
      map(() => true),
      catchError((err) => {
        console.error('Administrator registration error:', err);
        return of(false);
      })
    );
  }

  public registerPM(request: IRegisterRequest): Observable<boolean> {
    return this.authentificationService.registerPM(request).pipe(
      map(() => true),
      catchError((err) => {
        console.error('PM registration error:', err);
        return of(false);
      })
    );
  }

  public requestAdminRegistration(email: string): Observable<boolean> {
    return this.authentificationService.requestAdminRegistration(email).pipe(
      map(() => true),
      catchError((err) => {
        console.error('Admin registration request error:', err);
        return of(false);
      })
    );
  }

  public generateVerificationCode(email: string): Observable<boolean> {
    return this.authentificationService.generateVerificationCode(email).pipe(
      map(() => true),
      catchError((err) => {
        console.error('Verification code generation error:', err);
        return of(false);
      })
    );
  }

  public logout(): void {
    this.appStateService.clearAppState();
  }
}
