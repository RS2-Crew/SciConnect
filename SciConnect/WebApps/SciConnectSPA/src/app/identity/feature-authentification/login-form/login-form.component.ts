import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

interface ILoginFormData {
  username: string;
  password: string;
}

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.css'
})
export class LoginFormComponent {
  public loginForm: FormGroup;

  constructor(private routerService: Router) {
    this.loginForm = new FormGroup(
      {
        username: new FormControl("", [Validators.required, Validators.minLength(3)]),
        password: new FormControl("", [Validators.required, Validators.minLength(8)])
      }
      );
  }

  public onLoginFormSubmit(): void {
    if (this.loginForm.invalid){
      window.alert('Form has errors!');
      return;
    }

    const data: ILoginFormData = this.loginForm.value as ILoginFormData;

    this.loginForm.reset;
    this.routerService.navigate(['/identity', 'profile']);

  }
}
