import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { routes } from '../app/app.routes';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private Url = 'https://localhost:7036/api/Auth';
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  constructor(private http: HttpClient, private router: Router) { }


  login(data: { username: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.Url}/Login`, data).pipe(
      tap(res => {
        const token = res?.Token ?? res?.token ?? res?.TokenType ?? null;
        if (token) {
          localStorage.setItem('authToken', token);
          this.router.navigate(['/gifts']);
        }

        const userRaw = res?.User ?? res?.user ?? null;
        if (userRaw) {
          let roleVal: any = userRaw.Role ?? userRaw.role ?? 0;
          if (typeof roleVal === 'string') {
            const r = roleVal.trim().toLowerCase();
            if (r === 'admin') roleVal = 1;
            else if (!isNaN(Number(r))) roleVal = Number(r);
            else roleVal = 0;
          }
          if (typeof roleVal === 'object') {
            const numeric = Number(roleVal.value ?? roleVal.Value ?? roleVal);
            roleVal = isNaN(numeric) ? 0 : numeric;
          }

          const normalizedUser = {
            ...userRaw,
            role: Number(roleVal)
          };

          sessionStorage.setItem('user', JSON.stringify(normalizedUser));
          this.currentUserSubject.next(normalizedUser); // notify app
          console.log('AuthService: stored normalized user', normalizedUser);
        }
      }),
      catchError(err => {
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


  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout() {
    localStorage.removeItem('authToken');
  }


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