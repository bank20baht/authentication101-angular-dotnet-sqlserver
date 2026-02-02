import { Routes } from '@angular/router';
import { NavbarComponent } from './shared';

export const routes: Routes = [
  {
    path: '',
    component: NavbarComponent,
    children: [
      {
        path: '',
        loadComponent: () => import('./modules/main/main.component').then((m) => m.MainComponent),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./modules/register/register.component').then((m) => m.RegisterComponent),
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./modules/login/login.component').then((m) => m.LoginComponent),
      },
    ],
  },
];
