import { ProjectSummary } from './project.model';

/** Task priority levels matching the backend enum. */
export type TaskPriority = 0 | 1 | 2;

/** Human-readable priority labels. */
export const PRIORITY_LABELS: Record<TaskPriority, string> = {
  0: 'Low',
  1: 'Medium',
  2: 'High'
};

/** Bootstrap badge colours for each priority. */
export const PRIORITY_COLORS: Record<TaskPriority, string> = {
  0: 'success',
  1: 'warning',
  2: 'danger'
};

/** Represents a single task item returned by the API. */
export interface Task {
  id: number;
  task: string;
  description?: string;
  priority: TaskPriority;
  priorityLabel: string;
  dueDate?: string;
  isOverdue: boolean;
  isCompleted: boolean;
  createdDate: string;
  completedDate?: string;
  projectId?: number;
  projectName?: string;
  projectColor?: string;
}

/** Dashboard statistics returned by GET /api/tasks/dashboard. */
export interface Dashboard {
  totalTasks: number;
  completedTasks: number;
  remainingTasks: number;
  completedToday: number;
  completedTodayPercentage: number;
  overdueTasks: number;
  highPriorityTasks: number;
  totalProjects: number;
  projectSummaries: ProjectSummary[];
}

/** Generic paginated result wrapper matching the backend PaginatedResultDto<T>. */
export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
