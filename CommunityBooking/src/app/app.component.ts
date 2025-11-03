import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'CommunityBooking';
  isLoggedIn = false;
  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
  }

  logout(): void {
    this.authService.logout();
    this.isLoggedIn = false;
  }
}
