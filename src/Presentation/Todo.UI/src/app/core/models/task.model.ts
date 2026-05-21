/** Represents a single task item returned by the API. */
export interface Task {
  id: number;
  task: string;
  isCompleted: boolean;
  createdDate: string;
  completedDate?: string;
}

/** Generic paginated result wrapper matching the backend PaginatedResultDto<T>. */
export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
