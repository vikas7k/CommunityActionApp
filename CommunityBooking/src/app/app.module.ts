import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { EventListComponent } from './event-list/event-list.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { EventDetailComponent } from './event-detail/event-detail.component';
import { EventBookingComponent } from './event-booking/event-booking.component';
import { CustomerAddComponent } from './customer-add/customer-add.component';
import { CustomerLoginComponent } from './customer-login/customer-login.component';
import { CustomerBookingsComponent } from './customer-bookings/customer-bookings.component';

@NgModule({
  declarations: [
    AppComponent,
    EventListComponent,
    EventDetailComponent,
    EventBookingComponent,
    CustomerAddComponent,
    CustomerLoginComponent,
    CustomerBookingsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule  
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
