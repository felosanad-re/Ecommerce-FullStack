import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  constructor() {
    // Initialize theme on app start
    this.initTheme();
  }

  initTheme() {
    const savedTheme = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const theme = savedTheme || (prefersDark ? 'dark' : 'light');

    document.documentElement.setAttribute('data-theme', theme);

    // Update the topbar component's isDark state
    this.broadcastThemeChange(theme === 'dark');
  }

  toggleTheme() {
    const currentTheme = document.documentElement.getAttribute('data-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

    document.documentElement.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);

    // Broadcast the change to all components
    this.broadcastThemeChange(newTheme === 'dark');
  }

  private broadcastThemeChange(isDark: boolean) {
    // Dispatch a custom event that components can listen to
    window.dispatchEvent(new CustomEvent('themeChange', { detail: { isDark } }));
  }

  getCurrentTheme(): string {
    return document.documentElement.getAttribute('data-theme') || 'light';
  }
}