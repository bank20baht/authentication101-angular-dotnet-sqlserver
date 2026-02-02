import { Component, inject, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'authen-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('authentication-web');

  router = inject(Router);
  private translate = inject(TranslateService);

  constructor() {
    this.translate.addLangs(['th', 'en']);
    this.translate.setFallbackLang('en');
    this.translate.use('en');
  }
}
