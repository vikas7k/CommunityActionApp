import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-customer-login',
  standalone: false,
  templateUrl: './customer-login.component.html',
  styleUrl: './customer-login.component.css'
})

export class CustomerLoginComponent {
  loginForm: FormGroup;
  loading = false;
  errorMessage?: string;
  successMessage?: string;

  constructor(private fb: FormBuilder, private authService: AuthService, private location: Location) {
    this.loginForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    const { name, email } = this.loginForm.value;
    this.loading = true;
    this.errorMessage = undefined;
    this.successMessage = undefined;

    this.authService.login(name, email).subscribe({
      next: (token) => {
        this.loading = false;
        this.successMessage = 'Login successful!';
        this.location.back();
        setTimeout(() => window.location.reload(), 100); 
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Login failed';
      }
    });
  }
}
