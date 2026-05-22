import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../../core/services/project.service';
import { Project } from '../../core/models/project.model';
import { NavbarComponent } from '../../layouts/navbar/navbar.component';

/** Projects page — list, create, edit, and delete projects. */
@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [ReactiveFormsModule, NgFor, NgIf, RouterLink, NavbarComponent],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent implements OnInit {
  private readonly projectService = inject(ProjectService);
  private readonly fb = inject(FormBuilder);

  projects = signal<Project[]>([]);
  errorMessage = '';
  successMessage = '';

  showCreateForm = false;
  editingProjectId: number | null = null;

  readonly colorOptions = [
    { label: 'Navy', value: '#003087' },
    { label: 'Blue', value: '#0d6efd' },
    { label: 'Teal', value: '#0dcaf0' },
    { label: 'Green', value: '#198754' },
    { label: 'Orange', value: '#fd7e14' },
    { label: 'Red', value: '#dc3545' },
    { label: 'Purple', value: '#6f42c1' },
    { label: 'Pink', value: '#d63384' },
  ];

  createForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
    description: ['', Validators.maxLength(500)],
    color: ['#003087', Validators.required]
  });

  editForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
    description: ['', Validators.maxLength(500)],
    color: ['#003087', Validators.required]
  });

  ngOnInit(): void {
    this.loadProjects();
  }

  /** Loads all projects from the API. */
  loadProjects(): void {
    this.projectService.getProjects().subscribe({
      next: response => {
        if (response.success && response.data) {
          this.projects.set(response.data);
          this.errorMessage = '';
        }
      },
      error: () => {
        this.errorMessage = 'Unable to load projects. Please try again.';
      }
    });
  }

  /** Submits the create project form. */
  submitCreate(): void {
    if (this.createForm.invalid) return;

    const { name, description, color } = this.createForm.value;
    this.projectService.createProject({ name, description: description || undefined, color }).subscribe({
      next: response => {
        if (response.success && response.data) {
          this.projects.update(list => [response.data!, ...list]);
          this.createForm.reset({ name: '', description: '', color: '#003087' });
          this.showCreateForm = false;
          this.successMessage = 'Project created successfully.';
          setTimeout(() => (this.successMessage = ''), 3000);
        }
      },
      error: () => {
        this.errorMessage = 'Failed to create project. Please try again.';
      }
    });
  }

  /** Populates the edit form and opens the inline edit panel for the given project. */
  startEdit(project: Project): void {
    this.editingProjectId = project.id;
    this.editForm.patchValue({
      name: project.name,
      description: project.description ?? '',
      color: project.color
    });
  }

  /** Cancels the inline edit. */
  cancelEdit(): void {
    this.editingProjectId = null;
    this.editForm.reset();
  }

  /** Submits the edit form for the currently edited project. */
  submitEdit(projectId: number): void {
    if (this.editForm.invalid) return;

    const { name, description, color } = this.editForm.value;
    this.projectService.updateProject(projectId, { name, description: description || undefined, color }).subscribe({
      next: response => {
        if (response.success && response.data) {
          this.projects.update(list =>
            list.map(p => p.id === projectId ? response.data! : p)
          );
          this.editingProjectId = null;
          this.successMessage = 'Project updated successfully.';
          setTimeout(() => (this.successMessage = ''), 3000);
        }
      },
      error: () => {
        this.errorMessage = 'Failed to update project. Please try again.';
      }
    });
  }

  /** Soft-deletes the given project after confirmation. */
  deleteProject(project: Project): void {
    if (!confirm(`Delete project "${project.name}"? Tasks in this project will not be deleted.`)) return;

    this.projectService.deleteProject(project.id).subscribe({
      next: response => {
        if (response.success) {
          this.projects.update(list => list.filter(p => p.id !== project.id));
          this.successMessage = 'Project deleted.';
          setTimeout(() => (this.successMessage = ''), 3000);
        }
      },
      error: () => {
        this.errorMessage = 'Failed to delete project.';
      }
    });
  }

  /** Returns Bootstrap progress bar width percentage (capped at 100). */
  progressWidth(project: Project): number {
    if (project.taskCount === 0) return 0;
    return Math.round((project.completedTaskCount / project.taskCount) * 100);
  }
}
