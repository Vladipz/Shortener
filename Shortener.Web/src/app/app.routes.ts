import { Routes } from '@angular/router';
import { LayoutComponent } from './common-ui/layout/layout.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { UrlListComponent } from './pages/url-list/url-list.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: '',
        component: UrlListComponent,
      },
    ],
  },
  {
    path: 'login',
    component: LoginPageComponent,
  },
];
