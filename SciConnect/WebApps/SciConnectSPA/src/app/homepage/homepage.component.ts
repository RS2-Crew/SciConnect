import { Component, OnInit, Inject, PLATFORM_ID, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { AppStateService } from '../shared/app-state/app-state.service';
import { JwtService } from '../shared/jwt/jwt.service';
import { JwtPayloadKeys } from '../shared/jwt/jwt-payload-keys';
import { Subscription } from 'rxjs';

interface UserInfo {
  username: string;
  email: string;
  roles: string[];
  firstName?: string;
  lastName?: string;
}

@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css']
})
export class HomepageComponent implements OnInit, OnDestroy {
  public userInfo: UserInfo | null = null;
  public isDarkTheme: boolean = false;
  public currentTime: Date = new Date();
  public isLoading: boolean = true;
  private subscription: Subscription = new Subscription();

  constructor(
    private router: Router,
    private appStateService: AppStateService,
    private jwtService: JwtService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadThemePreference();
    this.applyTheme();
    this.updateTime();
    
    // Update time every minute
    setInterval(() => {
      this.updateTime();
    }, 60000);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private loadUserInfo(): void {
    try {
      // Subscribe to app state changes
      this.subscription.add(
        this.appStateService.getAppState().subscribe(appState => {
          if (appState.accessToken) {
            try {
              const payload = this.jwtService.parsePayload(appState.accessToken);
              const roles = appState.roles ? (Array.isArray(appState.roles) ? appState.roles : [appState.roles]) : [];
              
              this.userInfo = {
                username: payload[JwtPayloadKeys.Username] || appState.username || 'Unknown User',
                email: payload[JwtPayloadKeys.Email] || appState.email || 'No email available',
                roles: roles.map(role => role.toString()),
                firstName: appState.firstName || '',
                lastName: appState.lastName || ''
              };
            } catch (error) {
              // Fallback to app state data
              this.userInfo = {
                username: appState.username || 'Unknown User',
                email: appState.email || 'No email available',
                roles: appState.roles ? (Array.isArray(appState.roles) ? appState.roles.map(r => r.toString()) : [appState.roles.toString()]) : [],
                firstName: appState.firstName || '',
                lastName: appState.lastName || ''
              };
            }
          } else {
            // No token, redirect to login
            this.router.navigate(['/identity/login']);
            return;
          }
          this.isLoading = false;
        })
      );
    } catch (error) {
      this.router.navigate(['/identity/login']);
      return;
    }
  }

  public onLogout(): void {
    this.appStateService.clearAppState();
    this.router.navigate(['/identity/login']);
  }

  public toggleTheme(): void {
    this.isDarkTheme = !this.isDarkTheme;
    this.applyTheme();
    if (isPlatformBrowser(this.platformId)) {
      this.saveThemePreference();
    }
  }

  private loadThemePreference(): void {
    try {
      const savedTheme = localStorage.getItem('sciConnectTheme');
      this.isDarkTheme = savedTheme === 'dark';
    } catch (error) {
      console.warn('Could not load theme preference:', error);
    }
  }

  private saveThemePreference(): void {
    try {
      localStorage.setItem('sciConnectTheme', this.isDarkTheme ? 'dark' : 'light');
    } catch (error) {
      console.warn('Could not save theme preference:', error);
    }
  }

  private applyTheme(): void {
    if (this.isDarkTheme) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
  }

  private updateTime(): void {
    this.currentTime = new Date();
  }

  public getGreeting(): string {
    const hour = this.currentTime.getHours();
    if (hour < 12) return 'Good Morning';
    if (hour < 18) return 'Good Afternoon';
    return 'Good Evening';
  }

  public getRoleDisplayName(role: string): string {
    switch (role.toLowerCase()) {
      case 'administrator': return 'Administrator';
      case 'pm': return 'Project Manager';
      case 'guest': return 'Guest User';
      default: return role;
    }
  }

  public navigateToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  public navigateToAdminManagement(): void {
    this.router.navigate(['/admin-management']);
  }

  public navigateToProfile(): void {
    console.log('Navigate to profile');
  }

  public navigateToSettings(): void {
    console.log('Navigate to settings');
  }

  public isPM(): boolean {
    console.log('Checking PM role. User roles:', this.userInfo?.roles);
    if (!this.userInfo?.roles) return false;
    const isPMResult = this.userInfo.roles.some(role => role.toLowerCase() === 'pm');
    console.log('isPM result:', isPMResult);
    return isPMResult;
  }

  public getLogoPath(): string {
    return this.isDarkTheme 
      ? 'assets/images/dark_theme_logo.png' 
      : 'assets/images/white_theme_logo.png';
  }
}
