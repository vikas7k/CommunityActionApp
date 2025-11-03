import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Booking {
  Id: string;
  Name: string;
  Email: string;
  Option?: string;
  Notes?: string;
  CreatedAt: string;
  Title: string;        
  Start: string;    
  Category: string;
  ImageUrl: string;
  Description?: string;
  MoreInfoUrl: string;
  BookingEnabled: string;
  Capacity?: string;
  FunRunDistanceKm?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerBookingsService {
  private apiUrl = `${environment.apiBaseUrl}/bookings/customer`; 

  constructor(private http: HttpClient) { }

  getCustomerBookings(): Observable<Booking[]> {
     return this.http.get<Booking[]>(this.apiUrl);
  }
}
