import { inject, Injectable, signal } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, catchError, map, Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import {
  AuthenticationRequestBody,
  AuthorizationResponse,
  LogoutResponseBodyDto,
  RefreshTokenRequestBody,
  userData,
} from '@/shared';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private readonly _isLogin = new BehaviorSubject<boolean>(false);

  ToastService = inject(ToastrService);
  private readonly baseUrl: string = environment.apiBaseUrl;
  private readonly endpoint = 'api/authentication';
  http = inject(HttpClient);

  private userData = signal<AuthorizationResponse | null>(null);
  userData$ = this.userData.asReadonly();

  constructor() {
    const local = localStorage.getItem('user');
    if (local) {
      this._isLogin.next(true);
      const employeeData: AuthorizationResponse = JSON.parse(local);
      this.userData.set(employeeData);
    }
  }

  public get userValue() {
    const local = localStorage.getItem('user');
    if (local) {
      const data: AuthorizationResponse = JSON.parse(local);
      return data;
    } else {
      return undefined;
    }
  }

  register(body: AuthenticationRequestBody): Observable<AuthorizationResponse> {
    return this.http
      .post<AuthorizationResponse>(`${this.baseUrl}/${this.endpoint}/register`, body)
      .pipe(
        map((res) => {
          this.ToastService.success('Register Success');
          return res;
        }),
        catchError((err) => {
          console.error('API error:', err);
          this.ToastService.error('Login failed');
          return throwError(() => new Error('Signin Failed'));
        }),
      );
  }

  login(body: AuthenticationRequestBody): Observable<AuthorizationResponse> {
    return this.http
      .post<AuthorizationResponse>(`${this.baseUrl}/${this.endpoint}/login`, body)
      .pipe(
        map((res) => {
          localStorage.setItem('user', JSON.stringify(res));
          this.userData.set(res);
          this.ToastService.success('Signin Success');
          return res;
        }),
        catchError((err) => {
          console.error('API error:', err);
          this.ToastService.error('Login failed');
          return throwError(() => new Error('Signin Failed'));
        }),
      );
  }

  logout(): Observable<LogoutResponseBodyDto> {
    return this.http
      .post<LogoutResponseBodyDto>(`${this.baseUrl}/${this.endpoint}/logout`, null)
      .pipe(
        map((res) => {
          localStorage.removeItem('user');
          this.userData.set(null);
          this.ToastService.success('Signout Success');
          return res;
        }),
        catchError((err) => {
          console.error('API error:', err);
          this.ToastService.success('Signout Failed');
          return throwError(() => new Error('Logout failed'));
        }),
      );
  }

  getUserData(): Observable<userData> {
    return this.http.get<userData>(`${this.baseUrl}/${this.endpoint}/user`).pipe(
      map((res) => {
        return res;
      }),
      catchError((err) => {
        return throwError(() => Error('get Userdata faileds'));
      }),
    );
  }

  refreshToken() {
    const local = localStorage.getItem('user');
    if (local) {
      const data: AuthorizationResponse = JSON.parse(local);
      let body: RefreshTokenRequestBody = {
        username: data.username,
      };
      return this.http
        .post<AuthorizationResponse>(`${this.baseUrl}/${this.endpoint}/refresh-token`, body)
        .pipe(
          map((res) => {
            return res;
          }),
          catchError((err) => {
            console.error('API error:', err);
            return throwError(() => new Error('Refreshtoken failed'));
          }),
        );
    }
    return undefined;
  }
}
