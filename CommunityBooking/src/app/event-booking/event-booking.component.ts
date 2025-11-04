import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { EventService } from '../services/event.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { EventType } from '../models/eventType.model';

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
  eventTypes: EventType[] = [];
  termsAccepted = false; 
  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private route: ActivatedRoute,
    private eventService: EventService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id') || '';
    const customer = this.authService.getCurrentCustomer();

    this.bookingForm = this.fb.group({     
      name: [{ value: customer?.name || '', disabled: true }],
      email: [{ value: customer?.email || '', disabled: true }],
      entryType: ['', [Validators.required]],
      notes: [''],
      agreeToTerms: [false, [Validators.requiredTrue]]
    });

    this.loadEventTypes();
  }

  loadEventTypes(): void {
    this.eventService.getEventTypes().subscribe({
      next: (data) => (this.eventTypes = data),
      error: (err) => (this.errorMessage = 'Failed to load event types.')
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

    this.eventService.bookEvent(this.eventId, this.bookingForm.getRawValue()).subscribe({
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
