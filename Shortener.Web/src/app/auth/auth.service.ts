import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment.development';
import { BehaviorSubject, tap, throwError, catchError, Observable, map } from 'rxjs';
import { TokenResponce } from './auth.interface';
import { JwtPayload } from './jwt-payload.interface';
import { jwtDecode } from 'jwt-decode';
import { LoginFormData } from '../data/interfaces/login-form.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  http = inject(HttpClient);
  coockieService = inject(CookieService);
  router = inject(Router);
  baseUrl = `${environment.baseApiUrl}api/auth/`;
  private userEmailSubject = new BehaviorSubject<string | null>(
    this.getUserEmail(),
  );
  userEmail$ = this.userEmailSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.isAuth);
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  token: string | null = null;
  refreshToken: string | null = null;

  public get isAuth(): boolean {
    if (!this.token) {
      this.token = this.coockieService.get('token');
      this.refreshToken = this.coockieService.get('refreshToken');
    }
    return !!this.token;
  }

  login(credentials: LoginFormData): Observable<TokenResponce> {
    return this.http
      .post<Object>(`${this.baseUrl}login`, credentials)
      .pipe(
        map((response: any) => response as TokenResponce), // Приводимо відповідь до типу TokenResponce
        tap((response) => {
          this.setTokens(response);
          this.isAuthenticatedSubject.next(true);
        }),
        catchError((error) => {
          console.error('Login error:', error);
          if (error.status === 401) {
            return throwError(() => new Error('Invalid email or password'));
          }
          return throwError(() => new Error('An unexpected error occurred'));
        }),
      );
  }
  logout() {
    this.coockieService.deleteAll();
    this.token = null;
    this.refreshToken = null;
    this.userEmailSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  refreshAccessToken() {
    // Call the refresh token endpoint to get a new access token
    const refreshToken = this.coockieService.get('refreshToken');
    return this.http
      .post<TokenResponce>(`${this.baseUrl}refresh`, {
        refresh_token: refreshToken,
      })
      .pipe(
        tap((res) => {
          this.setTokens(res);
        }),
        catchError((err) => {
          this.logout();
          return throwError(() => err);
        }),
      );
  }

  private setTokens(res: TokenResponce) {
    console.log('Set tokens:', res);
    this.token = res.token;
    this.refreshToken = res.refreshToken;
    this.coockieService.set('token', res.token);
    this.coockieService.set('refreshToken', res.refreshToken);
    this.userEmailSubject.next(this.getUserEmail());
  }

  getDecodedToken(): JwtPayload | null {
    if (this.token) {
      try {
        return jwtDecode<JwtPayload>(this.token);
      } catch (e) {
        console.error('Invalid token:', e);
        return null;
      }
    }
    return null;
  }

  getUserEmail(): string | null {
    const decodedToken = this.getDecodedToken();
    return decodedToken?.email || null;
  }
}
