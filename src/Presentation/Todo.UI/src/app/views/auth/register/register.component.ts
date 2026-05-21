import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

/** Cross-field validator that ensures password and confirmPassword match. */
const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirm = control.get('confirmPassword');
  if (!password || !confirm) return null;
  return password.value !== confirm.value ? { passwordMismatch: true } : null;
};

/** Registration page — centered card with username, email, password, and confirm-password fields. */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  registerForm = this.fb.group(
    {
      username: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.pattern(/^(?=.*[A-Z])(?=.*\d).+$/)
        ]
      ],
      confirmPassword: ['', [Validators.required]]
    },
    { validators: passwordMatchValidator }
  );

  errorMessage = '';
  successMessage = '';
  isLoading = false;

  /** Username form control accessor. */
  get username() { return this.registerForm.get('username')!; }

  /** Email form control accessor. */
  get email() { return this.registerForm.get('email')!; }

  /** Password form control accessor. */
  get password() { return this.registerForm.get('password')!; }

  /** Confirm password form control accessor. */
  get confirmPassword() { return this.registerForm.get('confirmPassword')!; }

  /** Validates the form and submits the registration request. */
  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.register({
      username: this.username.value!,
      email: this.email.value!,
      password: this.password.value!,
      confirmPassword: this.confirmPassword.value!
    }).subscribe({
      next: response => {
        this.isLoading = false;
        if (response.success) {
          this.successMessage = 'Account created successfully! Redirecting to login...';
          setTimeout(() => this.router.navigate(['/auth/login']), 1500);
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
