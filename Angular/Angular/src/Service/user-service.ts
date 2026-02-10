import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

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
  AddToBasket(PurchaseData: any) {
    console.log('Adding to basket:', PurchaseData);
    return this.http.post(`${this.Url}`, PurchaseData, { headers: this.getHeaders() });
  }
  TicketPurchase(TicketData: any) {
    console.log('Purchasing ticket:', TicketData);
    return this.http.post(`${this.Url}/TicketPurchase`, TicketData, { headers: this.getHeaders() });
  }
  
}
