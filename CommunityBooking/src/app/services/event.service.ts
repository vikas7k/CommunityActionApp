import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { EventType } from '../models/eventType.model';
import { Event } from '../models/event.model';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EventService {
  private readonly baseUrl = `${environment.apiBaseUrl}`;

  constructor(private http: HttpClient) { }

  // Fetch all events
  getAllEvents(search?: string, category?: string): Observable<Event[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (category) params = params.set('category', category);
    let url = `${this.baseUrl}/events`;
    return this.http.get<Event[]>(url, { params });
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

  getEventTypes(): Observable<EventType[]> {
    return this.http.get<EventType[]>(`${this.baseUrl}/event-types`);
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
