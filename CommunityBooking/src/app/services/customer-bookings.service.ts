import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
//export interface Event {
//  id: string;
//  title: string;
//  imageUrl?: string;
//  start: string;
//  category: string;
//  description?: string;
//  moreInfoUrl?: string;
//  bookingEnabled: boolean;
//  capacity: number;
//  funRunDistanceKm?: number;
//}

//export interface Booking {
//  id: string;
//  name: string;
//  email: string;
//  option?: string;
//  notes?: string;
//  createdAt: string;
//  event: Event;
//}

export interface Booking {
  Id: string;
  Name: string;
  Email: string;
  Option?: string;
  Notes?: string;
  CreatedAt: string;
  Title: string;        // instead of nested event
  Start: string;     // rename fields to match API
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
  private apiUrl = `${environment.apiBaseUrl}/bookings/customer`// 'http://localhost:7131/api/bookings/customer'; // Azure Function endpoint

  constructor(private http: HttpClient) { }

  getCustomerBookings(): Observable<Booking[]> {
    const token = localStorage.getItem('customerToken');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    return this.http.get<Booking[]>(this.apiUrl, { headers });
  }
}
