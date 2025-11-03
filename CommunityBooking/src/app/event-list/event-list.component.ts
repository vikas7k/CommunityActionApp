import { Component, OnInit } from '@angular/core';
import { EventService } from '../services/event.service';
import { Event } from '../models/event.model';

@Component({
  selector: 'app-event-list',
  standalone: false,
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css']
})
export class EventListComponent implements OnInit {
  events: Event[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(private eventService: EventService) { }

  ngOnInit(): void {
    this.loadEvents();
  }

  private loadEvents(): void {
    this.eventService.getAllEvents().subscribe({
      next: (data: Event[]) => {
        this.events = data;        
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Failed to load events', err);
        this.errorMessage = 'Could not load events. Please try again later.';
        this.isLoading = false;
      }
    });
  }
}
