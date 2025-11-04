import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { DecodedToken } from '../models/decoded-token.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiBaseUrl}/auth/customer-token`; 

  constructor(private http: HttpClient, private router: Router) { }

  login(name: string, email: string): Observable<string> {
    return this.http.post<{ token: string }>(this.apiUrl, { name, email }).pipe(
      map(res => {
        if (res && res.token) {
          localStorage.setItem('customerToken', res.token);
          return res.token;
        } else {
          throw new Error('Token not received');
        }
      }),
      catchError(this.handleError)
    );
  }

  logout(): void {
    localStorage.removeItem('customerToken');
    this.router.navigate(['/']); 
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('customerToken');
  }

  getCurrentCustomer(): { name: string; email: string; role: string } | null {
    const token = localStorage.getItem('customerToken');
    if (!token) return null;
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      return {
        name: decoded.name,
        email: decoded.sub,
        role: decoded.role
      };
    } catch {
      return null;  
    }
  }

  private handleError(error: HttpErrorResponse) {
    let message = 'An unknown error occurred';
    if (error.error?.error) message = error.error.error;
    return throwError(() => new Error(message));
  }
}
