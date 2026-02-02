import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, catchError, finalize, from, map, switchMap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { LoaderService } from '@/services/loader/loader.service';
import { AuthenticationService, DialogService } from '@/services';
import { ProblemObjectResponse } from '@/shared';

@Injectable()
export class CustomInterceptor implements HttpInterceptor {
  constructor(
    private readonly router: Router,
    private readonly dialogService: DialogService,
    private readonly loader: LoaderService,
  ) {}

  authenticationSerivce = inject(AuthenticationService);
  userData$ = this.authenticationSerivce.userData$;

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.loader.show();

    request = request.clone({
      withCredentials: true,
    });

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error?.error instanceof Blob) {
          return from(error.error.text()).pipe(
            map((txt) => txt ?? ''),
            map((txt) => {
              try {
                return JSON.parse(txt) as ProblemObjectResponse;
              } catch {
                return { raw: txt } as any;
              }
            }),
            switchMap((problem) => this.handleErrorAndMaybeRefresh(request, next, error, problem)),
          );
        }

        const problem = this.normalizeError(error.error);
        return this.handleErrorAndMaybeRefresh(request, next, error, problem);
      }),
      finalize(() => {
        this.loader.hide();
      }),
    );
  }

  private handleErrorAndMaybeRefresh(
    request: HttpRequest<any>,
    next: HttpHandler,
    httpError: HttpErrorResponse,
    problem?: ProblemObjectResponse | any,
  ): Observable<HttpEvent<any>> {
    const errCode = problem?.status?.code ?? '';
    const errDesc = problem?.status?.desc ?? 'ERROR';
    const errDescBadReq: string[] = problem?.errors ?? [''];
    const messageBadReq = errDescBadReq.length > 0 ? errDescBadReq.join(', ') : '';

    if (httpError.status === 401 && errCode === 'FAIL_AUTHEN_OA') {
      this.dialogService.showError(`[${errCode}] ${errDesc}`);
      return throwError(() => {});
    }

    if (httpError.status === 400) {
      this.dialogService.showError(`${messageBadReq}`);
      return throwError(() => {});
    }

    if (httpError.status === 401) {
      return this.handleTokenExpired(request, next);
    }

    if (httpError.status !== 401) {
      this.dialogService.showError(`[${errCode}] ${errDesc}`);
      return throwError(() => {});
    }
    return throwError(() => {});
  }

  private normalizeError(err: any): ProblemObjectResponse | any {
    if (!err) return undefined;
    if (typeof err === 'string') {
      try {
        return JSON.parse(err);
      } catch {
        return { raw: err };
      }
    }
    return err;
  }

  private handleTokenExpired(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    const refreshToken$ = this.authenticationSerivce.refreshToken();
    if (!refreshToken$) {
      this.redirectToLogin();
      return throwError(() => new Error('No refresh token available'));
    }

    return refreshToken$.pipe(
      switchMap(() => {
        const newRequest = request.clone({ withCredentials: true });
        return next.handle(newRequest).pipe(
          catchError((error: HttpErrorResponse) => {
            if (error.status === 400 || error.status === 401) {
              this.redirectToLogin();
            }
            return throwError(() => error);
          }),
        );
      }),
      catchError((error) => {
        // this.redirectToLogin();
        return throwError(() => error);
      }),
    );
  }

  private redirectToLogin() {
    this.authenticationSerivce.logout();
    this.router.navigate(['login']);
  }
}
