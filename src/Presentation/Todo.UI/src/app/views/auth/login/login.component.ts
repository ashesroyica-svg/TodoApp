import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

/** Login page — centered card with reactive email/password form. */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  errorMessage = '';
  isLoading = false;

  /** Email form control accessor. */
  get email() { return this.loginForm.get('email')!; }

  /** Password form control accessor. */
  get password() { return this.loginForm.get('password')!; }

  /** Validates the form and submits the login request. */
  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login({
      email: this.email.value!,
      password: this.password.value!
    }).subscribe({
      next: response => {
        this.isLoading = false;
        if (response.success) {
          this.router.navigate(['/todo']);
        } else {
          this.errorMessage = response.message;
        }
      },
      error: err => {
        this.isLoading = false;
        this.errorMessage = err.error?.message ?? 'Unable to connect to server. Please try again.';
      }
    });
  }
}
