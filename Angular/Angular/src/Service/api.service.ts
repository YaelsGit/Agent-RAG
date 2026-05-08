import { inject, Inject, Injectable } from '@angular/core';
import { firstValueFrom, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http'; // וודאי שיש אימפורט
@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = 'https://localhost:7036/api/User';
private http = inject(HttpClient);  private getAuthHeaders(): HeadersInit {
    const token = localStorage.getItem('authToken') || '';
    const headers: HeadersInit = { 'Content-Type': 'application/json' };
    if (token) headers['Authorization'] = `Bearer ${token}`;
    return headers;
  }
   private getHeaders() {
    const token = localStorage.getItem('authToken');
    return { 'Authorization': `Bearer ${token}` };
  }

  async confirmBasket(): Promise<void> {
    const res = await fetch(`${this.baseUrl}/ConfirmBasket`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
    });

    if (!res.ok) {
      const text = await res.text();
      console.error('ConfirmBasket error response:', text);
      throw new Error(text || 'Confirm basket failed');
    }
  }

  async getWinners<T = any[]>(): Promise<T> {
    const res = await fetch(`${this.baseUrl}/GiftsWithWinners`, {
      method: 'GET',
      headers: { 
        ...this.getAuthHeaders(), 
        'Accept': 'application/json' 
      },
    });
    
    if (!res.ok) {
      const text = await res.text();
      throw new Error(text || 'Failed to fetch winners');
    }
    return res.json();
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('authToken');
  }

  private getUser(): any | null {
    const ud = sessionStorage.getItem('user') || localStorage.getItem('user');
    if (!ud) return null;
    try {
      return JSON.parse(ud);
    } catch (e) {
      console.error('Failed to parse stored user', e);
      return null;
    }
  }

  getRole(): number | null {
    const user = this.getUser();
    if (!user) return null;
    const rawRole: any = user.role ?? user.Role ?? null;
    if (rawRole === null || rawRole === undefined) return null;
    if (typeof rawRole === 'number') return rawRole;
    if (typeof rawRole === 'string') {
      const r = rawRole.trim().toLowerCase();
      if (r === 'admin') return 1;
      if (!isNaN(Number(r))) return Number(r);
      return 0;
    }
    const numeric = Number(rawRole.value ?? rawRole.Value ?? rawRole);
    return isNaN(numeric) ? null : numeric;
  }

  ensureAdmin(): void {
    if (!this.isAdmin()) {
      throw new Error('Access denied: admin only');
    }
  }

  private async adminFetch(path: string, options: RequestInit = {}): Promise<Response> {
    this.ensureAdmin();
    const res = await fetch(`${this.baseUrl}${path}`, {
      ...options,
      headers: { ...this.getAuthHeaders(), ...(options.headers || {}) }
    });
    return res;
  }

  async addToBasket(giftId: number, quantity: number): Promise<void> {
    const payload = { 
        GiftId: giftId, 
        Quentity: quantity
    };

    const res = await fetch(`${this.baseUrl}/AddToBasket`, {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(payload)
    });

    if (!res.ok) {
        const errorText = await res.text();
        console.error('AddToBasket failed with status:', res.status, 'Response:', errorText);
        throw new Error(errorText || 'Failed to add to basket');
    }
    
  }
public isAdmin(): boolean {
  const userStr = sessionStorage.getItem('user');
  if (!userStr) return false;

  const user = JSON.parse(userStr);
  
  if (user.role === 'Admin' || user.role === 1) {
    return true;
  }

  const token = sessionStorage.getItem('token');
  if (token) {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    return role === 'Admin';
  }
  
  return false;
}
getBasket(): Observable<any[]> {
  return this.http.get<any[]>(`${this.baseUrl}/GetBasket`, { headers: this.getHeaders() });
}

}