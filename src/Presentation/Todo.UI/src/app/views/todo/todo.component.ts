import { Component, OnInit, DestroyRef, inject, signal, computed } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgFor, NgIf, DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TaskService } from '../../core/services/task.service';
import { ProjectService } from '../../core/services/project.service';
import { Task, TaskPriority, PRIORITY_LABELS, PRIORITY_COLORS } from '../../core/models/task.model';
import { Project } from '../../core/models/project.model';
import { NavbarComponent } from '../../layouts/navbar/navbar.component';

/** Main todo page — displays task list with priority, due date, description, project filter, search, and pagination. */
@Component({
  selector: 'app-todo',
  standalone: true,
  imports: [ReactiveFormsModule, NgFor, NgIf, DatePipe, NavbarComponent],
  templateUrl: './todo.component.html',
  styleUrl: './todo.component.scss'
})
export class TodoComponent implements OnInit {
  private readonly taskService = inject(TaskService);
  private readonly projectService = inject(ProjectService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  tasks = signal<Task[]>([]);
  projects = signal<Project[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  currentPage = signal(1);
  readonly pageSize = 8;

  activeProjectId: number | undefined = undefined;

  readonly priorityLabels = PRIORITY_LABELS;
  readonly priorityColors = PRIORITY_COLORS;

  taskForm: FormGroup = this.fb.group({
    task: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(500)]],
    description: ['', Validators.maxLength(2000)],
    priority: [1],
    dueDate: [''],
    projectId: [null]
  });

  searchControl = new FormControl('');
  searchQuery = '';

  showTaskForm = false;
  errorMessage = '';
  taskError = '';

  /** Array of page numbers for the pagination control. */
  pageNumbers = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

  /** The currently active project (for display in the header). */
  activeProject = computed(() =>
    this.activeProjectId !== undefined
      ? this.projects().find(p => p.id === this.activeProjectId)
      : undefined
  );

  ngOnInit(): void {
    this.projectService.getProjects().subscribe({
      next: res => {
        if (res.success && res.data) this.projects.set(res.data);
      }
    });

    this.route.queryParams.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      const pid = params['projectId'] ? Number(params['projectId']) : undefined;
      this.activeProjectId = pid;
      if (pid !== undefined) this.taskForm.patchValue({ projectId: pid });
      this.loadTasks(1);
    });

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
    this.taskService.getTasks(page, this.pageSize, this.searchQuery, this.activeProjectId).subscribe({
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

  /** Submits the new task form. */
  addTask(): void {
    if (this.taskForm.invalid) {
      this.taskError = 'Please fill in the required fields.';
      return;
    }

    this.taskError = '';
    const { task, description, priority, dueDate, projectId } = this.taskForm.value;

    this.taskService.createTask({
      task,
      description: description?.trim() || undefined,
      priority: +priority as TaskPriority,
      dueDate: dueDate || undefined,
      projectId: projectId != null && projectId !== '' ? +projectId : undefined
    }).subscribe({
      next: response => {
        if (response.success) {
          this.taskForm.reset({ task: '', description: '', priority: 1, dueDate: '', projectId: this.activeProjectId ?? null });
          this.showTaskForm = false;
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

  /** Clears the project filter and shows all tasks. */
  clearProjectFilter(): void {
    this.activeProjectId = undefined;
    this.loadTasks(1);
  }
}
