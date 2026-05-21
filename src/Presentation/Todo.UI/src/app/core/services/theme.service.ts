import { Injectable } from '@angular/core';

/** Manages the light/dark theme, persisting the preference to localStorage. */
@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly THEME_KEY = 'ica-theme';

  constructor() {
    this.applyTheme(this.getSavedTheme());
  }

  /** Returns the currently active theme name. */
  getCurrentTheme(): string {
    return this.getSavedTheme();
  }

  /** Switches between light and dark themes and persists the choice. */
  toggle(): void {
    const next = this.isDark() ? 'light' : 'dark';
    this.applyTheme(next);
    localStorage.setItem(this.THEME_KEY, next);
  }

  /** Returns true when dark mode is active. */
  isDark(): boolean {
    return this.getSavedTheme() === 'dark';
  }

  private getSavedTheme(): string {
    return localStorage.getItem(this.THEME_KEY) ?? 'light';
  }

  private applyTheme(theme: string): void {
    document.documentElement.setAttribute('data-bs-theme', theme);
  }
}
