import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface Customer {
  id?: string | number;
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  notes?: string;
  createdAt?: string;
  updatedAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly baseUrl = `${environment.apiBaseUrl}`;  
  constructor(private http: HttpClient) {}


  /**
 * Create a new customer.
 */
  createCustomer(customerData: any): Observable<any> {
    const url = `${this.baseUrl}/customers`;
    return this.http.post(url, customerData).pipe(
      catchError(this.handleError)
    );
  }
  
  /**
   * Centralized error handler for HTTP operations.
   */
  private handleError = (error: any) => {
    let message = 'An unknown error occurred';
    if (error) {
      if (error.error && error.error.message) {
        message = error.error.message;
      } else if (error.message) {
        message = error.message;
      } else if (typeof error === 'string') {
        message = error;
      }
    }
    // Keep side-effect minimal: log to console for developer diagnostics.
    // Production apps may forward this to a remote logging infrastructure.
    // Preserve original error object where possible.
    // eslint-disable-next-line no-console
    console.error('CustomerService error:', error);
    return throwError(() => new Error(message));
  };
}
