import { NgModule } from "@angular/core";
import { IdentityComponent } from "./identity.component";
import { CommonModule } from "@angular/common";
import { IdentityRoutingModule } from "./identity-routing.module";
import { LoginFormComponent } from "./feature-authentification/login-form/login-form.component";
import { RegisterFormComponent } from "./feature-authentification/register-form/register-form.component";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";

@NgModule({
  declarations: [
    IdentityComponent,
    LoginFormComponent,
    RegisterFormComponent
  ],
  imports: [
    CommonModule,
    IdentityRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ],
})
export class IdentityModule {}

