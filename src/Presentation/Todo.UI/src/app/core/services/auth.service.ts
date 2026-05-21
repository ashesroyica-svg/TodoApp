import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
// import { environment } from '../../../../environments/environment';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { User } from '../models/user.model';

interface RegisterDto {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

interface LoginDto {
  email: string;
  password: string;
}

/** Handles authentication, JWT storage, and user session management. */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly TOKEN_KEY = 'ica_jwt_token';
  private readonly USER_KEY = 'ica_user';
  private readonly baseUrl = `${environment.apiUrl}/api/auth`;

  /** Registers a new user account. */
  register(dto: RegisterDto): Observable<ApiResponse<object>> {
    return this.http.post<ApiResponse<object>>(`${this.baseUrl}/register`, dto);
  }

  /** Authenticates the user and persists the token and user info to localStorage. */
  login(dto: LoginDto): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(`${this.baseUrl}/login`, dto).pipe(
      tap(response => {
        if (response.success && response.data) {
          localStorage.setItem(this.TOKEN_KEY, response.data.token);
          localStorage.setItem(this.USER_KEY, JSON.stringify(response.data));
        }
      })
    );
  }

  /** Removes all authentication data from localStorage. */
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }

  /** Returns true if a JWT token is present in localStorage. */
  isLoggedIn(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }

  /** Returns the stored JWT token string, or null if absent. */
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /** Returns the deserialized user object from localStorage, or null. */
  getCurrentUser(): User | null {
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? (JSON.parse(raw) as User) : null;
  }
}
