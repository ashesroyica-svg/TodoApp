import { Component, OnInit, DestroyRef, inject, signal, computed } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { NgFor, NgIf, DatePipe } from '@angular/common';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TaskService } from '../../core/services/task.service';
import { Task } from '../../core/models/task.model';
import { NavbarComponent } from '../../layouts/navbar/navbar.component';

/** Main todo page — displays task list with add, search, toggle, delete, and pagination. */
@Component({
  selector: 'app-todo',
  standalone: true,
  imports: [ReactiveFormsModule, NgFor, NgIf, DatePipe, NavbarComponent],
  templateUrl: './todo.component.html',
  styleUrl: './todo.component.scss'
})
export class TodoComponent implements OnInit {
  private readonly taskService = inject(TaskService);
  private readonly destroyRef = inject(DestroyRef);

  tasks = signal<Task[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  currentPage = signal(1);
  readonly pageSize = 8;

  taskControl = new FormControl('');
  searchControl = new FormControl('');
  searchQuery = '';

  errorMessage = '';
  taskError = '';

  /** Array of page numbers for the pagination control. */
  pageNumbers = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

  ngOnInit(): void {
    this.loadTasks(1);

    this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(query => {
      this.searchQuery = query ?? '';
      this.currentPage.set(1);
      this.loadTasks(1);
    });
  }

  /** Fetches a page of tasks from the API and updates local state. */
  loadTasks(page: number): void {
    this.taskService.getTasks(page, this.pageSize, this.searchQuery).subscribe({
      next: response => {
        if (response.success && response.data) {
          this.tasks.set(response.data.items);
          this.totalCount.set(response.data.totalCount);
          this.totalPages.set(response.data.totalPages);
          this.currentPage.set(response.data.page);
          this.errorMessage = '';
        }
      },
      error: () => {
        this.errorMessage = 'Unable to load tasks. Please try again.';
      }
    });
  }

  /** Validates the new-task input and calls the create API. */
  addTask(): void {
    const taskText = this.taskControl.value?.trim();
    if (!taskText) {
      this.taskError = 'Task description cannot be empty.';
      return;
    }

    this.taskError = '';

    this.taskService.createTask(taskText).subscribe({
      next: response => {
        if (response.success) {
          this.taskControl.setValue('');
          this.searchQuery = '';
          this.searchControl.setValue('', { emitEvent: false });
          this.loadTasks(1);
        }
      },
      error: () => {
        this.taskError = 'Failed to create task. Please try again.';
      }
    });
  }

  /** Toggles the completion status of a task and updates it in the local list. */
  toggleStatus(task: Task): void {
    this.taskService.updateTaskStatus(task.id, !task.isCompleted).subscribe({
      next: response => {
        if (response.success && response.data) {
          this.tasks.update(tasks =>
            tasks.map(t => t.id === task.id ? response.data! : t)
          );
        }
      },
      error: () => {
        this.errorMessage = 'Failed to update task status.';
      }
    });
  }

  /** Soft-deletes a task and removes it from the local list. */
  deleteTask(id: number): void {
    this.taskService.deleteTask(id).subscribe({
      next: response => {
        if (response.success) {
          this.tasks.update(tasks => tasks.filter(t => t.id !== id));
          this.totalCount.update(c => c - 1);
        }
      },
      error: () => {
        this.errorMessage = 'Failed to delete task.';
      }
    });
  }

  /** Navigates to a page if the target page is within valid bounds. */
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.loadTasks(page);
  }
}
