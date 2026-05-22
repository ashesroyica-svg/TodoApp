import { Component, OnInit, inject, signal } from '@angular/core';
import { NgIf, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { Dashboard } from '../../core/models/task.model';
import { NavbarComponent } from '../../layouts/navbar/navbar.component';

/** Dashboard page showing task completion statistics and today's progress bar. */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgIf, DecimalPipe, RouterLink, NavbarComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly taskService = inject(TaskService);

  stats = signal<Dashboard | null>(null);
  errorMessage = '';

  ngOnInit(): void {
    this.taskService.getDashboard().subscribe({
      next: response => {
        if (response.success && response.data) {
          this.stats.set(response.data);
        }
      },
      error: () => {
        this.errorMessage = 'Unable to load dashboard. Please try again.';
      }
    });
  }
}
