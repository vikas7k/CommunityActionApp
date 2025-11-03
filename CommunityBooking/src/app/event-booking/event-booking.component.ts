import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { EventService } from '../services/event.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-event-booking',
  standalone: false,
  templateUrl: './event-booking.component.html',
  styleUrl: './event-booking.component.css'
})

export class EventBookingComponent implements OnInit {
  bookingForm!: FormGroup;
  eventId!: string;
  submitting = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private route: ActivatedRoute,
    private eventService: EventService
  ) { }

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id') || '';

    this.bookingForm = this.fb.group({
      name: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      entryType: ['', [Validators.required]], // e.g., "run", "walk", "cakes"
      notes: [''],
      agreeToTerms: [false, [Validators.requiredTrue]]
    });
  }

  onSubmit(): void {
    if (this.bookingForm.invalid) {
      this.bookingForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.eventService.bookEvent(this.eventId, this.bookingForm.value).subscribe({
      next: () => {
        this.submitting = false;
        this.successMessage = 'Booking successful! A confirmation email has been sent.';
        this.bookingForm.reset();
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = err.message || 'Booking failed. Please try again later.';
      }
    });   
  }
}
