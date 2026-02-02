import { Component, inject } from '@angular/core';
import { ButtonComponent } from '../button/button.component';
import { Router, RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from '@/services';
import { CommonModule } from '@angular/common';

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
