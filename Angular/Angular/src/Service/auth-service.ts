import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private Url = 'https://localhost:7036/api/Auth';

  constructor(private http: HttpClient) { }

  /**
   * Logs in the user and stores the auth token on success.
   * @param data Login credentials
   */
  login(data: { username: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.Url}/Login`, data).pipe(
      tap(res => {
        if (res && res.token) {
          localStorage.setItem('authToken', res.token);
        }
      }),
      catchError(err => {
        // Return a user-friendly error object
        let message = 'אירעה שגיאה בעת ההתחברות. נסה שוב.';
        if (err.status === 401) {
          message = 'שם משתמש או סיסמה שגויים.';
        } else if (err.status === 0) {
          message = 'אין חיבור לשרת.';
        }
        return throwError(() => ({ ...err, userMessage: message }));
      })
    );
  }


  getToken() {
    return localStorage.getItem('authToken');
  }

  /**
   * Returns true if the user is logged in.
   */
  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  /**
   * Logs out the user by removing the auth token.
   */
  logout() {
    localStorage.removeItem('authToken');
  }

  /**
   * Registers a new user.
   * @param data User registration data
   */
  register(data: {
    id: number;
    firstName: string;
    lastName: string;
    userName: string;
    email: string;
    password: string;
    city: string;
    street: string;
    buildingNumber: number;
    phone: string;
  }): Observable<any> {
    return this.http.post(`${this.Url}`, data).pipe(
      catchError(err => {
        let message = 'אירעה שגיאה בעת ההרשמה. נסה שוב.';
        if (err.status === 409) {
          message = 'שם משתמש או אימייל כבר קיימים.';
        } else if (err.status === 0) {
          message = 'אין חיבור לשרת.';
        }
        return throwError(() => ({ ...err, userMessage: message }));
      })
    );
  }
}