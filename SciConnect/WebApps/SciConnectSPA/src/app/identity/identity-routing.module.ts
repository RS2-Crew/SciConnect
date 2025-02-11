import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IdentityComponent } from './identity.component';
import { LoginFormComponent } from './feature-authentification/login-form/login-form.component';
import { ReactiveFormsModule } from '@angular/forms';

const routes: Routes = [
  { path: '', component: IdentityComponent, children: [{path: 'login', component: LoginFormComponent}] }
];

@NgModule({
  imports: [RouterModule.forChild(routes), ReactiveFormsModule],
  exports: [RouterModule],
  bootstrap: [LoginFormComponent]
})
export class IdentityRoutingModule { }
