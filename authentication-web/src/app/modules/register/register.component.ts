import { AuthenticationService } from '@/services';
import { FormFieldConfig, FormsBuilderComponent, RegisterRequestBody } from '@/shared';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'authen-register',
  imports: [FormsBuilderComponent, TranslatePipe],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  registerFormData = signal<RegisterRequestBody>({ username: '', password: '' });
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
      validation: (value, fields) => {
        const confirmPassword = fields.find((f) => f.key === 'confirm-password')?.value;

        if (!value) return 'กรุณากรอก password';
        if (confirmPassword && value !== confirmPassword) {
          return 'รหัสผ่านไม่ตรงกัน';
        }
        return null;
      },
    },
    {
      key: 'confirm-password',
      label: 'register.forms.confirm-password',
      type: 'password',
      placeholder: 'กรอกรหัสผ่านอีกครั้ง',
      value: '',
      validation: (value, fields) => {
        const password = fields.find((f) => f.key === 'password')?.value;

        if (!value) return 'กรุณากรอก password';
        if (password !== value) return 'รหัสผ่านไม่ตรงกัน';
        return null;
      },
    },
  ];

  onFormValueChange(formData: RegisterRequestBody): void {
    this.registerFormData.set(formData);
  }

  onLogin() {
    this.authService.register(this.registerFormData()).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
