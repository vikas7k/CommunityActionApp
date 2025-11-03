import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerService } from '../services/customer.service';


@Component({
  selector: 'app-customer-add',
  standalone: false,
  templateUrl: './customer-add.component.html',
  styleUrl: './customer-add.component.css'
})


export class CustomerAddComponent {
  customerForm: FormGroup;
  submitting = false;
  successMessage = '';
  errorMessage = '';

  constructor(private fb: FormBuilder, private customerService: CustomerService) {
    this.customerForm = this.fb.group({
      Email: ['', [Validators.required, Validators.email]],
      Name: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.customerForm.invalid) {
      return;
    }

    this.submitting = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.customerService.createCustomer(this.customerForm.value).subscribe({
      next: () => {
        this.submitting = false;
        this.successMessage = 'Customed registered successfully.';
        this.customerForm.reset();
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = err.message || 'Customer registration failed. Please try again later.';
      }
    });   
  }
}
