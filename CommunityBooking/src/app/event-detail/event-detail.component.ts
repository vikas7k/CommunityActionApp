import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Event } from '../models/event.model';
import { EventService } from '../services/event.service';
import { AuthService } from '../services/auth.service';
@Component({
  selector: 'app-event-detail',
  standalone: false,
  templateUrl: './event-detail.component.html',
  styleUrl: './event-detail.component.css'
})
export class EventDetailComponent implements OnInit {

  event?: Event;
  loading = true;
  error?: string;
  customerLoggedIn = false;
  constructor(private eventService: EventService, private authService: AuthService,  private route: ActivatedRoute) { }

  ngOnInit(): void {
    const eventId = this.route.snapshot.paramMap.get('id');   
    this.customerLoggedIn = this.authService.isLoggedIn();
    if (eventId) {
      this.loadEvent(eventId);
    }  
  }

  private loadEvent(eventId: string): void {
    this.eventService.getEventById(eventId).subscribe({
      next: (data: Event) => {
        this.event = data;
        this.loading = false;
      },
      error: (err: any) => {
        console.error('Failed to load events', err);
        this.error = 'Could not load events. Please try again later.';
        this.loading = false;
      }
    });
  }
}
