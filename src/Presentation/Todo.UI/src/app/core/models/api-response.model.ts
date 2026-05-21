/** Standard API response wrapper matching the backend ApiResponse<T> shape. */
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors?: string[];
}
