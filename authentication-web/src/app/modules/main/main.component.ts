import { AuthenticationService } from '@/services';
import { userData } from '@/shared';
import { Component, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'authen-main',
  imports: [TranslatePipe],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css',
})
export class MainComponent {
  userData = signal<userData>({
    username: '',
  });

  authenService = inject(AuthenticationService);

  ngOnInit() {
    this.authenService.getUserData().subscribe({
      next: (res) => {
        this.userData.set(res);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
