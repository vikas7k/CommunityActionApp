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
  filteredEvents: Event[] = [];  // Events to display
  eventTitles: string[] = [];
  eventCategories: string[] = [];
  selectedTitle: string = '';
  selectedCategory: string = '';
  isLoading = true;
  errorMessage = '';
   constructor(private eventService: EventService) { }

  ngOnInit(): void {  
    this.loadAllEvents();
  }

  private loadAllEvents(): void {
    this.eventService.getAllEvents().subscribe({
      next: (data: Event[]) => {     
        this.filteredEvents = data;
        this.isLoading = false;
        // Fill distinct dropdown values
        this.eventTitles = Array.from(new Set(data.map(e => e.Title))).sort();
        this.eventCategories = Array.from(new Set(data.map(e => e.Category))).sort();
      },
      error: (err) => {
        console.error('Failed to load events', err);
        this.errorMessage = 'Could not load events. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  // Call this when dropdown changes
  onFilterChange(): void {
    this.eventService.getAllEvents(this.selectedTitle, this.selectedCategory).subscribe({
      next: (data: Event[]) => {
        this.filteredEvents = data;
      },
      error: (err) => {
        console.error('Failed to fetch filtered events', err);
        this.isLoading = false;
      }
    });
  }
}
