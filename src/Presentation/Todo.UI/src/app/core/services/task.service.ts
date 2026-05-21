import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Task, PaginatedResult } from '../models/task.model';

/** Provides typed HTTP methods for all task CRUD operations. */
@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/tasks`;

  /** Fetches a paginated list of tasks with optional keyword search. */
  getTasks(page: number, pageSize: number, search?: string): Observable<ApiResponse<PaginatedResult<Task>>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search?.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<ApiResponse<PaginatedResult<Task>>>(this.baseUrl, { params });
  }

  /** Creates a new task with the provided description. */
  createTask(task: string): Observable<ApiResponse<Task>> {
    return this.http.post<ApiResponse<Task>>(this.baseUrl, { task });
  }

  /** Updates the IsCompleted status of a task. */
  updateTaskStatus(id: number, isCompleted: boolean): Observable<ApiResponse<Task>> {
    return this.http.patch<ApiResponse<Task>>(`${this.baseUrl}/${id}/status`, { isCompleted });
  }

  /** Soft-deletes a task by ID. */
  deleteTask(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${id}`);
  }
}
