import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ILoginRequest } from "../models/login-request";
import { Observable } from "rxjs";
import { ILoginResponse } from "../models/login-response";
import { ILogoutRequest } from "../models/logout-request";

@Injectable({
  providedIn: 'root'
  }
)
export class AuthentificationService {
  private readonly url: string = 'http://localhost:4000/api/v1/authentication';

  constructor(private httpClient: HttpClient){}

  public login(request: ILoginRequest): Observable<ILoginResponse> {
    return this.httpClient.post<ILoginResponse>(`${this.url}/login`, request);
  }

  public logout(request: ILogoutRequest): Observable<any>{
    return this.httpClient.post(`${this.url}/logout`, request);
  }
}




