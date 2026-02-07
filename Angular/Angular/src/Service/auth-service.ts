import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private Url = 'https://localhost:7036/api/Auth';

  constructor(private http: HttpClient) { }

  login(data: { username: string; password: string }) {
    return this.http.post<any>(`${this.Url}/Login`, data).pipe(
      tap(res => {
        if (res && res.token) {
          localStorage.setItem('authToken', res.token);
        }
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
  }) {
    return this.http.post(`${this.Url}`, data);
  }
}