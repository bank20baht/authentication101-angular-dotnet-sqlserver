import { Component, inject } from '@angular/core';
import { ButtonComponent } from '../button/button.component';
import { ActivatedRoute, NavigationEnd, Router, RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from '@/services';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs';

@Component({
  selector: 'authen-navbar',
  imports: [TranslatePipe, RouterModule, ButtonComponent, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  currentLang: 'th' | 'en' = 'en';
  translateService = inject(TranslateService);
  authenticationService = inject(AuthenticationService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  title: string = '';

  constructor() {
    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      let currentRoute = this.route.root;

      while (currentRoute.firstChild) {
        currentRoute = currentRoute.firstChild;
      }

      this.title = currentRoute.snapshot.data['title'] ?? '';
    });
  }

  switchLang(lang: 'th' | 'en') {
    this.translateService.use(lang);
    this.currentLang = lang;
  }

  handleClickLogout() {
    this.authenticationService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
