import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppState, IAppState } from './app-state';
import { Role } from './role';

@Injectable({
  providedIn: 'root'
})
export class AppStateService {
  //Ovde ako bi vratili appState u geteru, znaci da njegovu referencu nekom dajemo
  // I on moze da promeni appState i da se to propagira kroz celu aplikaciju
  // Zato kad hocemo da ga promenimo mi svaki put pravimo novi Objekat AppState sa tom promenom
  private appState: IAppState = new AppState();
  private appStateSubject: BehaviorSubject<IAppState> =  new BehaviorSubject<IAppState>(this.appState);
  private appStateObservable: Observable<IAppState> =  this.appStateSubject.asObservable();

  public getAppState(): Observable<IAppState>{
    return this.appStateObservable;
  }

}
