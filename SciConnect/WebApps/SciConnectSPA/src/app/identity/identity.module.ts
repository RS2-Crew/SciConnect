import { NgModule } from "@angular/core";
import { IdentityComponent } from "./identity.component";
import { CommonModule } from "@angular/common";
import { IdentityRoutingModule } from "./identity-routing.module";
import { LoginFormComponent } from "./feature-authentification/login-form/login-form.component";
import { ReactiveFormsModule } from "@angular/forms";

@NgModule({
  declarations: [
    IdentityComponent,
    LoginFormComponent
  ],
  imports: [
    CommonModule,
    IdentityRoutingModule,
    ReactiveFormsModule
  ],
})
export class IdentityModule {}

