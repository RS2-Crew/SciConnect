import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AdminManagementRoutingModule } from './admin-management-routing.module';
import { AdminManagementComponent } from './admin-management.component';

@NgModule({
  declarations: [
    AdminManagementComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AdminManagementRoutingModule
  ]
})
export class AdminManagementModule { }

