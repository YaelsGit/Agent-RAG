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
  AddToBasket(PurchaseData: Purchase) {
    // Validation: Ensure required fields are present
    if (!PurchaseData || !PurchaseData.giftId || !PurchaseData.userId) {
      return {
        subscribe: (cb: any, errCb: any) => errCb({ userMessage: 'יש למלא את כל השדות הנדרשים בסל.' })
      };
    }
    console.log('Adding to basket:', PurchaseData);
    return this.http.post(`${this.Url}`, PurchaseData, { headers: this.getHeaders() });
  }
  TicketPurchase(TicketData: Purchase) {
    // Validation: Ensure required fields are present
    if (!TicketData || !TicketData.giftId || !TicketData.userId) {
      return {
        subscribe: (cb: any, errCb: any) => errCb({ userMessage: 'יש למלא את כל השדות הנדרשים לרכישת כרטיס.' })
      };
    }
    console.log('Purchasing ticket:', TicketData);
    return this.http.post(`${this.Url}/TicketPurchase`, TicketData, { headers: this.getHeaders() });
  }

}
