import { Injectable } from "@angular/core";
import { AuthentificationService } from "../infrastructure/authentification.service";
import { catchError, map, Observable, of, switchMap } from "rxjs";
import { ILoginRequest } from "../models/login-request";
import { AppStateService } from "../../../shared/app-state/app-state.service";
import { ILoginResponse } from "../models/login-response";
import { JwtService } from "../../../shared/jwt/jwt.service";
import { JwtPayloadKeys } from "../../../shared/jwt/jwt-payload-keys";
@Injectable({
  providedIn: 'root'
})
export class AuthentificationFacadeService {

  constructor(private authentificationService:AuthentificationService, private appStateService: AppStateService, private jwtService: JwtService
  ){}

  public login(username: string, password: string): Observable<boolean>{
    const request: ILoginRequest = {username: username, password: password};

    return this.authentificationService.login(request).pipe(
      switchMap((loginResponse: ILoginResponse) => {
        this.appStateService.setAccessToken(loginResponse.accessToken);
        this.appStateService.setRefreshToken(loginResponse.refreshToken);

        const payload = this.jwtService.parsePayload(loginResponse.accessToken);

        this.appStateService.setRoles(payload[JwtPayloadKeys.Role]);

        return payload[JwtPayloadKeys.Username];
      }),
      map(() => {return true;}),
      catchError((err) => {
        console.log(err);
        //cistimo stanje aplikacije u slucaju greske.
        this.appStateService.clearAppState();
        return of(false);
      })
    );
  }
}
