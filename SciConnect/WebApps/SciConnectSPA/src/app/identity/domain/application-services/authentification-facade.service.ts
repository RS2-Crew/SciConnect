import { Injectable } from "@angular/core";
import { AuthentificationService } from "../infrastructure/authentification.service";
import { map, Observable, switchMap } from "rxjs";
import { ILoginRequest } from "../models/login-request";
import { AppStateService } from "../../../shared/app-state/app-state.service";
import { ILoginResponse } from "../models/login-response";

@Injectable({
  providedIn: 'root'
})
export class AuthentificationFacadeService {
  constructor(private authentificationService:AuthentificationService, private appStateService: AppStateService
  ){}

  public login(username: string, password: string): Observable<boolean>{
    const request: ILoginRequest = {username: username, password: password};

    return this.authentificationService.login(request).pipe(
      map((loginResponse:ILoginResponse) => {
        return true;
      })
    );
  }
}
