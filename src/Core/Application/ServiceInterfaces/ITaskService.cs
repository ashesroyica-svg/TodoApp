using Application.DTOs.Todo;
using Application.Wrappers;

namespace Application.ServiceInterfaces;

/// <summary>Service interface for task management operations.</summary>
public interface ITaskService
{
    /// <summary>Retrieves a paginated list of tasks for the authenticated user.</summary>
    Task<ApiResponse<PaginatedResultDto<TaskResponseDto>>> GetTasksAsync(int userId, int page, int pageSize, string? search, CancellationToken ct = default);

    /// <summary>Creates a new task for the authenticated user.</summary>
    Task<ApiResponse<TaskResponseDto>> CreateTaskAsync(int userId, CreateTaskDto dto, CancellationToken ct = default);

    /// <summary>Updates the completion status of a task owned by the user.</summary>
    Task<ApiResponse<TaskResponseDto>> UpdateTaskStatusAsync(int userId, int taskId, UpdateTaskStatusDto dto, CancellationToken ct = default);

    /// <summary>Soft-deletes a task owned by the user.</summary>
    Task<ApiResponse<object>> DeleteTaskAsync(int userId, int taskId, CancellationToken ct = default);
}
