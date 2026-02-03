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
        data: { title: 'IT 02-3' },
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./modules/register/register.component').then((m) => m.RegisterComponent),
        data: { title: 'IT 02-2' },
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./modules/login/login.component').then((m) => m.LoginComponent),
        data: { title: 'IT 02-1' },
      },
    ],
  },
];
