import {
    Routes
} from '@angular/router';

import {
    LoginComponent
} from './pages/login/login.component';

import {
    PurchaseBillComponent
} from './pages/purchase-bill/purchase-bill.component';

import {
    authGuard
} from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    component: LoginComponent
  },

  {
    path: 'purchase-bill',
    component: PurchaseBillComponent,
    canActivate: [authGuard]
  },

  {
    path: '**',
    redirectTo: 'login'
  }
];