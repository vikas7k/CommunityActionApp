import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
@Component({
  selector: 'app-customer-logout',
  standalone: false,
  templateUrl: './customer-logout.component.html',
  styleUrl: './customer-logout.component.css'
})
export class CustomerLogoutComponent implements OnInit {
  constructor(   
    private router: Router, private authService: AuthService
  ) { }

  ngOnInit(): void {
   this.authService.logout();
   window.location.href = '/';   
  }


}
