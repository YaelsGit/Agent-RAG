import { Injectable } from '@angular/core';
import { Gift } from '../Model/Gift';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class GiftService {
  private Url = 'https://localhost:7036/api/Gift';
  constructor(private http: HttpClient) { }

  private getHeaders() {
    const token = localStorage.getItem('authToken');
    console.log("Auth Token:", token);
    return { 'Authorization': `Bearer ${token}` };
  }
  getallGifts(): Observable<Gift[]> {

    return this.http.get<Gift[]>(this.Url, { headers: this.getHeaders() });
  }
  getGiftByGiftName(name: string): Observable<any[]> {
    const params = new HttpParams().set('name', name);
    return this.http.get<any[]>(`${this.Url}/Name`, { headers: this.getHeaders(), params });
  }
  getGiftByDonorName(firstName: string, lastName: string): Observable<any[]> {
    const params = new HttpParams().set('firstName', firstName).set('lastName', lastName);
    return this.http.get<any[]>(`${this.Url}/Donor`, { headers: this.getHeaders(), params });
  }
  GetGiftByNumPurchase(num: number): Observable<any[]> {
    const params = new HttpParams().set('num', num);
    return this.http.get<any[]>(`${this.Url}/Count`, { headers: this.getHeaders(), params });
  }
  createCategory(categoryData: any) {
    return this.http.post(`${this.Url}/Category`, categoryData, { headers: this.getHeaders() });
  }
  createGift(giftData: any) {
    return this.http.post(this.Url, giftData, { headers: this.getHeaders() });
  }
  updateGift(id: number, giftData: any) {
    return this.http.put(`${this.Url}/${id}`, giftData, {
      headers: this.getHeaders()
    });
  }
  deleteGift(id: number): Observable<void> {
    return this.http.delete<void>(`${this.Url}/${id}`, {
      headers: this.getHeaders()
    });
  }
  GetGiftPurchases(giftId: number): Observable<any> {
    const params = new HttpParams().set('giftId', giftId.toString());
    return this.http.get<any>(`${this.Url}/Purchase&Gift`, { headers: this.getHeaders(), params });
  }
  GetGiftsBySorted(sortBy: string): Observable<any[]> {
    if (sortBy == 'price') {
      return this.http.get<any[]>(`${this.Url}/sort-by-price`, { headers: this.getHeaders() });
    } else {
      return this.http.get<any[]>(`${this.Url}/sort-by-most-purchased`, { headers: this.getHeaders() });
    }
  }

  GetsPurchaseWithUser(giftId: number): Observable<any[]> {
    const params = new HttpParams().set('giftId', giftId.toString());
    return this.http.get<any[]>(`${this.Url}/purchases-with-users`, { headers: this.getHeaders(), params })
      .pipe(
        map(purchases => purchases.map((p: any) => ({
          id: p.id ?? 0,
          date: p.date ?? '',
          giftId: p.giftId ?? 0,
          userId: p.userId ?? 0,
          firstName: p.firstName ?? '',
          lastName: p.lastName ?? ''
        })))
      );
  }


  GiftRandom(): Observable<any[]> {
    return this.http.post<any[]>(
      `${this.Url}/drawAllWinners`,
      {},
      { headers: this.getHeaders() }
    );
  }

  GetTotalSum(): Observable<any> {
    return this.http.get<any>(`${this.Url}/TotalSum`, { headers: this.getHeaders() });
  }
  GetGiftsBySortedCategoryOrPrice(sortedBy: string): Observable<any[]> {
    const params = new HttpParams().set('sortedBy', sortedBy);
    return this.http.get<any[]>(`${this.Url}/GetBySorted`, { headers: this.getHeaders(), params });
  }
  DownloadGiftRandomFile() {
    return this.http.get(`${this.Url}/generateWinnersFile`, {
      headers: this.getHeaders(),
      responseType: 'blob'
    });
  }
  AddToBasket(PurchaseData: any) {
    console.log('Adding to basket:', PurchaseData);
    return this.http.post(`${this.Url}`, PurchaseData, { headers: this.getHeaders() });
  }
}