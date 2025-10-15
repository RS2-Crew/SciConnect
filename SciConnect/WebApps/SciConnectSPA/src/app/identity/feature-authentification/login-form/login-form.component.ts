import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthentificationFacadeService } from '../../domain/application-services/authentification-facade.service';
import { ThemeService } from '../../../shared/services/theme.service';

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

  constructor(
    private routerService: Router,
    private authentificationService: AuthentificationFacadeService,
    public themeService: ThemeService
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

  public onLoginFormSubmit(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const loginData: ILoginFormData = this.loginForm.value as ILoginFormData;

    this.authentificationService.login(loginData.username, loginData.password).subscribe({
      next: (success) => {
        this.isLoading = false;
        if (success) {
          this.loginSuccess = 'Login successful! Redirecting to dashboard...';
          setTimeout(() => {
            this.routerService.navigate(['/dashboard']);
          }, 1500);
        } else {
          this.loginError = 'Invalid username or password. Please try again.';
        }
      },
      error: (error) => {
        this.isLoading = false;
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
}
