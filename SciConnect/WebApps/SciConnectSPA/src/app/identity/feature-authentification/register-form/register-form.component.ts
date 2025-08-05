import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';


interface IRegisterFormData {
  email: string;
  username: string;
  password: string;
}

@Component({
  selector: 'app-register-form',
  standalone: true,
  imports: [],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.css'
})
export class RegisterFormComponent {
  public registerForm: FormGroup;

  constructor(){
    this.registerForm = new FormGroup(
      {
        email: new FormControl("", [Validators.required, Validators.email]),
        username: new FormControl("", [Validators.required, Validators.minLength(3)]),
        password: new FormControl("", [Validators.required, Validators.minLength(8)])
      }
    );
  }


  public onRegisterFormSubmit(): void {
    if (this.registerForm.invalid){
      window.alert('Form has errors!');
      return;
    }

    const data: IRegisterFormData = this.registerForm.value as IRegisterFormData;
    
  }

}
