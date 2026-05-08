import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Purchase } from '../Model/Purchase';

@Injectable({
  providedIn: 'root',
})
export class UserService {
    private Url = 'https://localhost:7036/api/User';
  constructor(private http: HttpClient) { }
  private getHeaders() {
    const token = localStorage.getItem('authToken');
    return { 'Authorization': `Bearer ${token}` };
  }
  AddToBasket(purchaseData: any) {
    return this.http.post(`${this.Url}/AddToBasket`, purchaseData, { headers: this.getHeaders() });
  }

  TicketPurchase(purchaseData: any) {
    return this.http.post(`${this.Url}/TicketPurchase`, purchaseData, { headers: this.getHeaders() });
  }

  ConfirmBasket() {
    return this.http.post(`${this.Url}/ConfirmBasket`, {}, { headers: this.getHeaders() });
  }

  RemoveFromBasket(purchaseId: number) {
    return this.http.delete(`${this.Url}/RemoveFromBasket/${purchaseId}`, { headers: this.getHeaders() });
  }

  GetGiftsWithWinners() {
    return this.http.get(`${this.Url}/GiftsWithWinners`, { headers: this.getHeaders() });
  }
}
