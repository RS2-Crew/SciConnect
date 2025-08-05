import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
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
export class LoginFormComponent {
  public loginForm: FormGroup;

  constructor(private routerService: Router,
    private authentificationService: AuthentificationFacadeService
  ) {
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

    this.authentificationService.login(data.username, data.password).subscribe((success: boolean) => {
      window.alert(`Login ${success ? 'is': 'is not'} successful!`);
      this.loginForm.reset;
    })

  }
}
