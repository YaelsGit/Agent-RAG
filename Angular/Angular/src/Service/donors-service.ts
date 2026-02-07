import { Injectable } from '@angular/core';
import { Gift } from '../Model/Gift';
import { HttpClient } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';
import { Donor } from '../Model/Donor';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({ providedIn: 'root' })
export class DonorService {
  private Url = 'https://localhost:7036/api/Donor';
  constructor(private http: HttpClient) { }

  private getHeaders() {
    const token = localStorage.getItem('authToken');
    return { 'Authorization': `Bearer ${token}` };
  }

  getallDonors(): Observable<Donor[]> {
    return this.http.get<Donor[]>(this.Url, { headers: this.getHeaders() });
  }

  createDonor(donorData: any) {
    return this.http.post(this.Url, donorData, { headers: this.getHeaders() });
  }

  updateDonor(id: number, donorData: any) {
return this.http.put(`${this.Url}/${id}`, donorData, { 
    headers: this.getHeaders() 
  });
}

 deleteDonor(id: number) {
  return this.http.delete(`${this.Url}/${id}`, { 
    headers: this.getHeaders() 
  });
}

  getDonorByName(firstName: string, lastName: string): Observable<Donor[]> {
    const params = new HttpParams().set('firstName', firstName).set('lastName', lastName);
    return this.http.get<Donor[]>(`${this.Url}/Name`, { headers: this.getHeaders(), params });
  }

  getDonorByEmail(email: string): Observable<Donor[]> {
    const params = new HttpParams().set('email', email);
    return this.http.get<Donor[]>(`${this.Url}/Email`, { headers: this.getHeaders(), params });
  }

  getDonorByGift(gift: string): Observable<Donor[]> {
    const params = new HttpParams().set('gift', gift);
    return this.http.get<Donor[]>(`${this.Url}/Gift`, { headers: this.getHeaders(), params });
  }
}