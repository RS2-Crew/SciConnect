import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { AuthentificationFacadeService } from '../../domain/application-services/authentification-facade.service';
import { Observable } from 'rxjs';
import { IRegisterRequest } from '../../domain/models/register-request';

interface IRegisterFormData {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  role: string;
  password: string;
  confirmPassword: string;
  verificationCode?: string;
}

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.css']
})
export class RegisterFormComponent implements OnInit {
  public registerForm: FormGroup;
  public isLoading: boolean = false;
  public registerError: string = '';
  public registerSuccess: string = '';
  public showPassword: boolean = false;
  public showConfirmPassword: boolean = false;
  public isDarkTheme: boolean = false;
  public showVerificationCode: boolean = false;

  constructor(
    private routerService: Router,
    private authentificationService: AuthentificationFacadeService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.registerForm = new FormGroup({
      firstName: new FormControl("", [
        Validators.required,
        Validators.minLength(2),
        Validators.pattern(/^[a-zA-Z\s]*$/)
      ]),
      lastName: new FormControl("", [
        Validators.required,
        Validators.minLength(2),
        Validators.pattern(/^[a-zA-Z\s]*$/)
      ]),
      userName: new FormControl("", [
        Validators.required,
        Validators.minLength(3),
        Validators.pattern(/^[a-zA-Z0-9_]*$/)
      ]),
      email: new FormControl("", [
        Validators.required,
        Validators.email
      ]),
      role: new FormControl("guest", [
        Validators.required
      ]),
      password: new FormControl("", [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/)
      ]),
      confirmPassword: new FormControl("", [
        Validators.required
      ]),
      verificationCode: new FormControl("")
    });

    // Add custom validator after form creation
    this.registerForm.setValidators(this.passwordMatchValidator);

    // Load theme preference from localStorage only in browser
    if (isPlatformBrowser(this.platformId)) {
      this.loadThemePreference();
    }
  }

  ngOnInit(): void {
    this.clearMessages();
    if (isPlatformBrowser(this.platformId)) {
      this.applyTheme();
    }
  }

  public isFieldInvalid(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return field ? field.invalid && field.touched : false;
  }

  public isFieldTouched(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return field ? field.touched : false;
  }

  public togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  public toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  public toggleTheme(): void {
    this.isDarkTheme = !this.isDarkTheme;
    this.applyTheme();
    if (isPlatformBrowser(this.platformId)) {
      this.saveThemePreference();
    }
  }

  public onRoleChange(): void {
    const selectedRole = this.registerForm.get('role')?.value;
    this.showVerificationCode = selectedRole === 'administrator';
    
    // Clear verification code when role changes
    if (selectedRole !== 'administrator') {
      this.registerForm.patchValue({ verificationCode: '' });
    }
  }

  public onRequestAdminRegistration(): void {
    const email = this.registerForm.get('email')?.value;
    if (!email) {
      this.registerError = 'Please enter your email address first.';
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    this.authentificationService.requestAdminRegistration(email).subscribe({
      next: (success) => {
        this.isLoading = false;
        if (success) {
          this.registerSuccess = 'Admin registration request sent successfully! Please check your email for further instructions.';
        } else {
          this.registerError = 'Failed to send admin registration request. Please try again.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.registerError = this.getErrorMessage(error);
        console.error('Admin registration request error:', error);
      }
    });
  }

  public onRegisterFormSubmit(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const registrationData: IRegisterRequest = {
      firstName: this.registerForm.get('firstName')?.value,
      lastName: this.registerForm.get('lastName')?.value,
      userName: this.registerForm.get('userName')?.value,
      password: this.registerForm.get('password')?.value,
      email: this.registerForm.get('email')?.value,
      verificationCode: this.registerForm.get('verificationCode')?.value
    };

    const selectedRole = this.registerForm.get('role')?.value;
    let registrationObservable: Observable<boolean>;

    switch (selectedRole) {
      case 'guest':
        registrationObservable = this.authentificationService.registerGuest(registrationData);
        break;
      case 'administrator':
        if (!registrationData.verificationCode) {
          this.registerError = 'Verification code is required for administrator registration.';
          this.isLoading = false;
          return;
        }
        registrationObservable = this.authentificationService.registerAdministrator(registrationData);
        break;
      case 'pm':
        registrationObservable = this.authentificationService.registerPM(registrationData);
        break;
      default:
        this.registerError = 'Invalid role selected.';
        this.isLoading = false;
        return;
    }

    registrationObservable.subscribe({
      next: (success) => {
        this.isLoading = false;
        if (success) {
          this.registerSuccess = 'Registration successful! Redirecting to login...';
          this.registerForm.reset();
          // Reset role to guest after successful registration
          this.registerForm.patchValue({ role: 'guest' });
          setTimeout(() => {
            this.routerService.navigate(['/identity/login']);
          }, 2000);
        } else {
          this.registerError = 'Registration failed. Please try again.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.registerError = this.getErrorMessage(error);
        console.error('Registration error:', error);
      }
    });
  }

  public onBackToLogin(): void {
    this.routerService.navigate(['/identity/login']);
  }

  private passwordMatchValidator(control: AbstractControl): { [key: string]: any } | null {
    const form = control as FormGroup;
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    
    return null;
  }

  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      const control = this.registerForm.get(key);
      control?.markAsTouched();
    });
  }

  private clearMessages(): void {
    this.registerError = '';
    this.registerSuccess = '';
  }

  private getErrorMessage(error: any): string {
    if (error.status === 400) {
      return error.error || 'Invalid registration data. Please check your information.';
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

  public getPasswordStrength(): string {
    const password = this.registerForm.get('password')?.value;
    if (!password) return '';
    
    let strength = 0;
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[@$!%*?&]/.test(password)) strength++;
    
    if (strength <= 2) return 'weak';
    if (strength <= 3) return 'medium';
    return 'strong';
  }

  public getPasswordStrengthColor(): string {
    const strength = this.getPasswordStrength();
    switch (strength) {
      case 'weak': return '#dc3545';
      case 'medium': return '#ffc107';
      case 'strong': return '#28a745';
      default: return '#6c757d';
    }
  }

  public getLogoPath(): string {
    return this.isDarkTheme 
      ? 'assets/images/dark_theme_logo.png' 
      : 'assets/images/white_theme_logo.png';
  }
}
