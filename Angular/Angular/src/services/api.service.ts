import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ApiService {
  // Use the same base URL as UserService to avoid CORS / wrong-port issues
  private baseUrl = 'https://localhost:7036/api/User';

  private getAuthHeaders(): HeadersInit {
    // Align token key name with Angular UserService (authToken)
    const token = localStorage.getItem('authToken') || '';
    const headers: HeadersInit = { 'Content-Type': 'application/json' };
    if (token) headers['Authorization'] = `Bearer ${token}`;
    return headers;
  }
async confirmBasket(basketData: any): Promise<void> {
  console.log('ConfirmBasket payload:', basketData);

  const res = await fetch(`${this.baseUrl}/ConfirmBasket`, {
    method: 'POST',
    headers: {
      ...this.getAuthHeaders(),
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(basketData),
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || 'Confirm basket failed');
  }
}


  async getWinners<T = any[]>(): Promise<T> {
    const res = await fetch(`${this.baseUrl}/Winners`, {
      method: 'GET',
      headers: { 'Accept': 'application/json' },
    });
    if (!res.ok) throw new Error('Failed to fetch winners');
    return res.json();
  }
}
