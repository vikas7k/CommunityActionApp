import { Component, OnInit } from '@angular/core';
import { CustomerBookingsService, Booking } from '../services/customer-bookings.service';
import { AuthService } from '../services/auth.service';
@Component({
  selector: 'app-customer-bookings',
  standalone: false,
  templateUrl: './customer-bookings.component.html',
  styleUrl: './customer-bookings.component.css'
})
export class CustomerBookingsComponent implements OnInit {
  bookings: Booking[] = [];
  loading = true;
  error?: string;
  constructor(private bookingsService: CustomerBookingsService, private authService: AuthService) { }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.loadBookings();
    }
    else {
      this.error = 'Please log in to view your booking.';
      this.loading = false;
    };
  }

  loadBookings(): void {
    this.bookingsService.getCustomerBookings().subscribe({
      next: (data: Booking[]) => {
        this.bookings = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load bookings', err);
        this.error = 'Could not load bookings. Please try again later.';
        this.loading = false;
      }
    });
  }
}
