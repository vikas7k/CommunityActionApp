import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EventDetailComponent } from './event-detail/event-detail.component';
import { EventListComponent } from './event-list/event-list.component';
import { EventBookingComponent } from './event-booking/event-booking.component';
import { CustomerAddComponent } from './customer-add/customer-add.component';
import { CustomerLoginComponent } from './customer-login/customer-login.component';
import { CustomerBookingsComponent } from './customer-bookings/customer-bookings.component';

//const routes: Routes = [];

const routes: Routes = [
  { path: '', component: EventListComponent, pathMatch: 'full' },
  { path: 'events/:id', component: EventDetailComponent },
  { path: 'events/:id/book', component: EventBookingComponent },
  { path: 'add-customer', component: CustomerAddComponent },
  { path: 'login', component: CustomerLoginComponent },
  { path: 'my-bookings', component: CustomerBookingsComponent },
  { path: '**', redirectTo: '' }
  // other routes...
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
