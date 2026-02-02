import { AuthenticationService } from '@/services';
import { AuthenticationRequestBody, FormFieldConfig, FormsBuilderComponent } from '@/shared';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'authen-login',
  imports: [FormsBuilderComponent, TranslatePipe],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  authenData = signal<AuthenticationRequestBody>({ username: '', password: '' });
  isAuthenFormsValid = signal<boolean>(false);
  authService = inject(AuthenticationService);
  router = inject(Router);

  formFields: FormFieldConfig[] = [
    {
      key: 'username',
      label: 'login.forms.username',
      type: 'text',
      placeholder: 'กรอกชื่อผู้ใช้',
      value: '',
      validation: (value) => {
        if (!value) return 'กรุณากรอก username';
        return null;
      },
    },
    {
      key: 'password',
      label: 'login.forms.password',
      type: 'password',
      placeholder: 'กรอกรหัสผ่าน',
      value: '',
      validation: (value) => {
        if (!value) return 'กรุณากรอก password';
        return null;
      },
    },
  ];

  onFormValueChange(formData: AuthenticationRequestBody): void {
    this.authenData.set(formData);
  }

  onLogin() {
    this.authService.login(this.authenData()).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
