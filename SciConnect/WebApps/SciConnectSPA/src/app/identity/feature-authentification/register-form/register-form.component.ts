import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { AuthentificationFacadeService } from '../../domain/application-services/authentification-facade.service';

interface IRegisterFormData {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
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
  public selectedRole: string = 'guest';
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
    this.showVerificationCode = this.selectedRole === 'administrator';
    if (this.showVerificationCode) {
      this.registerForm.get('verificationCode')?.setValidators([Validators.required]);
    } else {
      this.registerForm.get('verificationCode')?.clearValidators();
    }
    this.registerForm.get('verificationCode')?.updateValueAndValidity();
  }

  public onRegisterFormSubmit(): void {
    this.clearMessages();
    
    if (this.registerForm.invalid) {
      this.markFormGroupTouched();
      this.registerError = 'Please correct the errors above before submitting.';
      return;
    }

    this.isLoading = true;
    const data: IRegisterFormData = this.registerForm.value as IRegisterFormData;

    // Remove confirmPassword from the data sent to backend
    const { confirmPassword, ...registrationData } = data;

    // For now, we'll simulate registration since the service doesn't have a register method
    // In a real implementation, you would call the actual registration service
    setTimeout(() => {
      this.isLoading = false;
      this.registerSuccess = 'Registration successful! Redirecting to login...';
      this.registerForm.reset();
      setTimeout(() => {
        this.routerService.navigate(['/identity/login']);
      }, 2000);
    }, 1500);
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
}
