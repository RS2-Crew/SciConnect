import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/identity/login', pathMatch: 'full' },
  { path: 'identity', loadChildren: () => import('./identity/identity.module').then(m => m.IdentityModule) },
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
  { path: 'admin-management', loadChildren: () => import('./admin-management/admin-management.module').then(m => m.AdminManagementModule) }
];
