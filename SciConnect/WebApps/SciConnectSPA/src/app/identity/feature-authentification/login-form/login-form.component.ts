import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthentificationFacadeService } from '../../domain/application-services/authentification-facade.service';
import { ThemeService } from '../../../shared/services/theme.service';
import { FormUtils } from '../../../shared/utils/form-utils';

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
      FormUtils.markFormGroupTouched(this.loginForm);
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
        this.loginError = FormUtils.getHttpErrorMessage(error, 'login');
      }
    });
  }

  public onForgotPassword(event: Event): void {
    event.preventDefault();
    this.loginError = 'Password reset functionality will be available soon. Please contact your administrator.';
  }

  private clearMessages(): void {
    this.loginError = '';
    this.loginSuccess = '';
  }
}
