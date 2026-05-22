import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';

/** Application top navbar with theme toggle and logout. */
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  private readonly authService = inject(AuthService);
  readonly themeService = inject(ThemeService);
  private readonly router = inject(Router);

  /** Display name of the currently authenticated user. */
  get username(): string {
    return this.authService.getCurrentUser()?.username ?? 'User';
  }

  /** Toggles the application theme between light and dark mode. */
  toggleTheme(): void {
    this.themeService.toggle();
  }

  /** Logs out the current user and navigates to the login page. */
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}
