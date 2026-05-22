/** Represents a project that groups related tasks. */
export interface Project {
  id: number;
  name: string;
  description?: string;
  color: string;
  taskCount: number;
  completedTaskCount: number;
  createdDate: string;
}

/** Summary of a project used in the dashboard. */
export interface ProjectSummary {
  id: number;
  name: string;
  color: string;
  taskCount: number;
  completedCount: number;
  completionPercentage: number;
}

/** DTO for creating a new project. */
export interface CreateProjectDto {
  name: string;
  description?: string;
  color: string;
}

/** DTO for updating an existing project. */
export interface UpdateProjectDto {
  name: string;
  description?: string;
  color: string;
}
