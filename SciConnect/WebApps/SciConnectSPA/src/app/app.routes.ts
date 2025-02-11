import { Routes } from '@angular/router';

export const routes: Routes = [
  {path: 'identity', loadChildren: () => import('./identity/identity.module').then(m => m.IdentityModule)}
];
