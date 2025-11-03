import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Event } from '../models/event.model';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EventService {
  private readonly baseUrl = `${environment.apiBaseUrl}`//'http://localhost:7131/api';

  constructor(private http: HttpClient) { }

  // Fetch all events
  getAllEvents(): Observable<Event[]> {
    let url = `${this.baseUrl}/events`;
    return this.http.get<Event[]>(url);
  }

  //Get event by Id
  getEventById(id: string): Observable<Event> {
    const url = `${this.baseUrl}/events/${id}`;
    return this.http.get<Event>(url);
  }

  bookEvent(eventId: string, bookingData: any): Observable<any> {
    const url = `${this.baseUrl}/events/${eventId}/book`;
    return this.http.post(url, bookingData).pipe(
      catchError(this.handleError)
    );
  }
  private handleError(error: HttpErrorResponse) {
    console.error('EventService error:', error);
    let msg = 'An unknown error occurred.';
    if (error.status === 0) msg = 'Network error — cannot reach API.';
    else if (error.status >= 400 && error.status < 500) msg = 'Invalid request or event not found.';
    else if (error.status >= 500) msg = 'Server error — please try again later.';
    return throwError(() => new Error(msg));
  }
}
