import { Component, OnInit, inject, signal } from '@angular/core';
import { NgIf, NgFor, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { Dashboard } from '../../core/models/task.model';
import { NavbarComponent } from '../../layouts/navbar/navbar.component';

/** Dashboard page showing task completion statistics, priority breakdown, and project summaries. */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgIf, NgFor, DecimalPipe, RouterLink, NavbarComponent],
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

  /** Returns Bootstrap progress bar width percentage (capped at 100). */
  projectProgress(taskCount: number, completedCount: number): number {
    if (taskCount === 0) return 0;
    return Math.round((completedCount / taskCount) * 100);
  }
}
