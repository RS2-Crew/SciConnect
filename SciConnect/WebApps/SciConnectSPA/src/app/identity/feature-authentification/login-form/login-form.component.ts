import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { AuthentificationFacadeService } from '../../domain/application-services/authentification-facade.service';

interface ILoginFormData {
  username: string;
  password: string;
}

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.css'
})
export class LoginFormComponent implements OnInit {
  public loginForm: FormGroup;
  public isLoading: boolean = false;
  public loginError: string = '';
  public loginSuccess: string = '';
  public showPassword: boolean = false;
  public isDarkTheme: boolean = false;

  constructor(
    private routerService: Router,
    private authentificationService: AuthentificationFacadeService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.loginForm = new FormGroup({
      username: new FormControl("", [
        Validators.required, 
        Validators.minLength(3)
      ]),
      password: new FormControl("", [
        Validators.required, 
        Validators.minLength(8)
      ])
    });
  }

  ngOnInit(): void {
    // Clear any existing error/success messages
    this.clearMessages();
    // Load theme preference from localStorage only in browser
    if (isPlatformBrowser(this.platformId)) {
      this.loadThemePreference();
      this.applyTheme();
    }
  }

  public isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return field ? field.invalid && field.touched : false;
  }

  public isFieldTouched(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return field ? field.touched : false;
  }

  public togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  public toggleTheme(): void {
    this.isDarkTheme = !this.isDarkTheme;
    this.applyTheme();
    if (isPlatformBrowser(this.platformId)) {
      this.saveThemePreference();
    }
  }

  public onLoginFormSubmit(): void {
    this.clearMessages();
    
    if (this.loginForm.invalid) {
      this.markFormGroupTouched();
      this.loginError = 'Please correct the errors above before submitting.';
      return;
    }

    this.isLoading = true;
    const data: ILoginFormData = this.loginForm.value as ILoginFormData;

    this.authentificationService.login(data.username, data.password).subscribe({
      next: (success: boolean) => {
        this.isLoading = false;
        if (success) {
          this.loginSuccess = 'Login successful! Redirecting...';
          this.loginForm.reset();
          // Redirect to dashboard or main page after successful login
          setTimeout(() => {
            this.routerService.navigate(['/dashboard']);
          }, 1500);
        } else {
          this.loginError = 'Invalid username or password. Please try again.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Login error:', error);
        this.loginError = this.getErrorMessage(error);
      }
    });
  }

  public onForgotPassword(event: Event): void {
    event.preventDefault();
    this.loginError = 'Password reset functionality will be available soon. Please contact your administrator.';
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  private clearMessages(): void {
    this.loginError = '';
    this.loginSuccess = '';
  }

  private getErrorMessage(error: any): string {
    if (error.status === 401) {
      return 'Invalid username or password. Please check your credentials.';
    } else if (error.status === 0) {
      return 'Unable to connect to the server. Please check your internet connection.';
    } else if (error.status >= 500) {
      return 'Server error. Please try again later.';
    } else {
      return 'An unexpected error occurred. Please try again.';
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
}
