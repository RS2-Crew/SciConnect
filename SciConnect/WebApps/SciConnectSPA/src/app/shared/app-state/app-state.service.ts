import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppState, IAppState } from './app-state';
import { Role } from './role';
import { LocalStorageService } from '../local-storage/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class AppStateService {
  private readonly APP_STATE_KEY = 'sciConnectAppState';
  private appState: IAppState;
  private appStateSubject: BehaviorSubject<IAppState>;
  private appStateObservable: Observable<IAppState>;

  constructor(private localStorageService: LocalStorageService){
    // Restore state from localStorage on initialization
    const savedState = this.localStorageService.get<IAppState>(this.APP_STATE_KEY);
    if (savedState) {
      this.appState = new AppState(
        savedState.accessToken,
        savedState.refreshToken,
        savedState.username,
        savedState.email,
        savedState.roles,
        savedState.firstName,
        savedState.lastName,
        savedState.userId
      );
    } else {
      this.appState = new AppState();
    }
    this.appStateSubject = new BehaviorSubject<IAppState>(this.appState);
    this.appStateObservable = this.appStateSubject.asObservable();
  }
  public getAppState(): Observable<IAppState>{
    return this.appStateObservable;
  }

  public setAccessToken(accessToken: string): void {
    this.appState = this.appState.clone();
    this.appState.accessToken = accessToken;
    this.appStateSubject.next(this.appState);
    this.localStorageService.set(this.APP_STATE_KEY, this.appState);
  }

  public setRefreshToken(refreshToken: string): void {
    this.appState = this.appState.clone();
    this.appState.refreshToken = refreshToken;
    this.appStateSubject.next(this.appState);
    this.localStorageService.set(this.APP_STATE_KEY, this.appState);
  }

  public setUserName(username: string): void {
    this.appState = this.appState.clone();
    this.appState.username = username;
    this.appStateSubject.next(this.appState);
    this.localStorageService.set(this.APP_STATE_KEY, this.appState);
  }

  public clearAppState(): void {
    this.localStorageService.clear(this.APP_STATE_KEY);
    this.appState = new AppState();
    this.appStateSubject.next(this.appState);

  }

  public setRoles(roles: Role | Role[]): void {
    this.appState = this.appState.clone();
    this.appState.roles = roles;
    this.appStateSubject.next(this.appState);
    this.localStorageService.set(this.APP_STATE_KEY, this.appState);

  }

  public setFirstName(firstName: string): void {
    this.appState = this.appState.clone();
    this.appState.firstName = firstName;
    this.appStateSubject.next(this.appState);
    this.localStorageService.set(this.APP_STATE_KEY, this.appState);

  }

  public setLastName(lastName: string): void {
    this.appState = this.appState.clone();
    this.appState.lastName = lastName;
    this.appStateSubject.next(this.appState);
    this.localStorageService.set(this.APP_STATE_KEY, this.appState);

  }

  public setUserId(userId: string): void {
    this.appState = this.appState.clone();
    this.appState.userId = userId;
    this.appStateSubject.next(this.appState);
    this.localStorageService.set(this.APP_STATE_KEY, this.appState);
  }
}
