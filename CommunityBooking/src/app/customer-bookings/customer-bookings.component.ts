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

  downloadBookingsCSV() {
    this.bookingsService.getCustomerBookings().subscribe({
      next: (bookings) => {
        if (!bookings || bookings.length === 0) {
          alert('No bookings available to download.');
          return;
        }

        // Convert JSON to CSV string
        const csvData = this.convertToCSV(bookings);
        const blob = new Blob([csvData], { type: 'text/csv' });
        const url = window.URL.createObjectURL(blob);

        // Create a temporary link to trigger download
        const a = document.createElement('a');
        a.setAttribute('href', url);
        a.setAttribute('download', 'my_bookings.csv');
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
      },
      error: (err) => {
        console.error('Failed to download bookings', err);
        alert('Failed to download bookings.');
      }
    });
  }

  private convertToCSV(objArray: any[]): string {
    const header = Object.keys(objArray[0]).join(',');
    const rows = objArray.map(row =>
      Object.values(row)
        .map(val => `"${val?.toString().replace(/"/g, '""')}"`) // Escape quotes
        .join(',')
    );
    return [header, ...rows].join('\r\n');
  }
}
