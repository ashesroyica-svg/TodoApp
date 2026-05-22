import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Project, CreateProjectDto, UpdateProjectDto } from '../models/project.model';

/** Provides typed HTTP methods for all project CRUD operations. */
@Injectable({ providedIn: 'root' })
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/projects`;

  /** Fetches all projects for the authenticated user. */
  getProjects(): Observable<ApiResponse<Project[]>> {
    return this.http.get<ApiResponse<Project[]>>(this.baseUrl);
  }

  /** Creates a new project. */
  createProject(dto: CreateProjectDto): Observable<ApiResponse<Project>> {
    return this.http.post<ApiResponse<Project>>(this.baseUrl, dto);
  }

  /** Updates an existing project by ID. */
  updateProject(id: number, dto: UpdateProjectDto): Observable<ApiResponse<Project>> {
    return this.http.put<ApiResponse<Project>>(`${this.baseUrl}/${id}`, dto);
  }

  /** Soft-deletes a project by ID. */
  deleteProject(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${id}`);
  }
}
