import { FormGroup } from '@angular/forms';

export class FormUtils {
  
  public static markFormGroupTouched(form: FormGroup): void {
    Object.keys(form.controls).forEach(key => {
      const control = form.get(key);
      control?.markAsTouched();
    });
  }

  public static getHttpErrorMessage(error: any, context: 'login' | 'register'): string {
    if (context === 'login') {
      if (error.status === 401) {
        return 'Invalid username or password. Please check your credentials.';
      }
    } else if (context === 'register') {
      if (error.status === 400) {
        return error.error || 'Invalid registration data. Please check your information.';
      }
    }
    
    // Common errors
    if (error.status === 0) {
      return 'Unable to connect to the server. Please check your internet connection.';
    } else if (error.status >= 500) {
      return 'Server error. Please try again later.';
    } else {
      return 'An unexpected error occurred. Please try again.';
    }
  }
}
