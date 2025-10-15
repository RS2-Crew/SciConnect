import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthentificationService } from '../identity/domain/infrastructure/authentification.service';
import { AppStateService } from '../shared/app-state/app-state.service';
import { FormUtils } from '../shared/utils/form-utils';

@Component({
  selector: 'app-admin-management',
  templateUrl: './admin-management.component.html',
  styleUrls: ['./admin-management.component.css']
})
export class AdminManagementComponent implements OnInit {
  public generateCodeForm: FormGroup;
  public isLoading = false;
  public successMessage = '';
  public errorMessage = '';
  public isPM = false;

  constructor(
    private fb: FormBuilder,
    private authentificationService: AuthentificationService,
    private appStateService: AppStateService
  ) {
    this.generateCodeForm = this.fb.group({
      adminEmail: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit(): void {
    this.checkUserRole();
  }

  private checkUserRole(): void {
    this.appStateService.getAppState().subscribe(appState => {
      if (appState.roles) {
        const roles = Array.isArray(appState.roles) ? appState.roles : [appState.roles];
        this.isPM = roles.some(role => role.toString() === 'PM');
      }
    });
  }

  public onGenerateVerificationCode(): void {
    if (this.generateCodeForm.invalid) {
      FormUtils.markFormGroupTouched(this.generateCodeForm);
      return;
    }

    const adminEmail = this.generateCodeForm.get('adminEmail')?.value;
    this.isLoading = true;
    this.clearMessages();

    this.authentificationService.generateVerificationCode(adminEmail).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.successMessage = `Verification code has been sent to ${adminEmail}`;
        this.generateCodeForm.reset();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = this.getHttpErrorMessage(error);
      }
    });
  }

  private clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }

  private getHttpErrorMessage(error: any): string {
    if (error?.error?.message) {
      return error.error.message;
    }
    if (error?.message) {
      return error.message;
    }
    if (typeof error?.error === 'string') {
      return error.error;
    }
    return 'An error occurred while generating verification code.';
  }

  public isFieldInvalid(fieldName: string): boolean {
    const field = this.generateCodeForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
}

