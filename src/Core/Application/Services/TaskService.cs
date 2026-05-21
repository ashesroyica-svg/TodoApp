using Application.DTOs.Todo;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using Application.Wrappers;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>Handles all task management business logic.</summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    /// <summary>Returns a paginated list of tasks for the user with optional keyword search.</summary>
    public async Task<ApiResponse<PaginatedResultDto<TaskResponseDto>>> GetTasksAsync(
        int userId, int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var totalCount = await _taskRepository.GetTotalCountAsync(userId, search, ct);
        var tasks = await _taskRepository.GetPagedAsync(userId, page, pageSize, search, ct);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);

        var result = new PaginatedResultDto<TaskResponseDto>
        {
            Items = tasks.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };

        return ApiResponse<PaginatedResultDto<TaskResponseDto>>.Ok(result, "Tasks retrieved successfully.");
    }

    /// <summary>Creates a new task for the user.</summary>
    public async Task<ApiResponse<TaskResponseDto>> CreateTaskAsync(
        int userId, CreateTaskDto dto, CancellationToken ct = default)
    {
        var task = new TaskItem
        {
            Task = dto.Task,
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            IsCompleted = false,
            IsDeleted = false
        };

        var created = await _taskRepository.CreateAsync(task, ct);
        _logger.LogInformation("Task created for user {UserId}: {TaskId}", userId, created.Id);
        return ApiResponse<TaskResponseDto>.Ok(MapToDto(created), "Task created successfully.");
    }

    /// <summary>Toggles the completion status of a task; sets or clears CompletedDate accordingly.</summary>
    public async Task<ApiResponse<TaskResponseDto>> UpdateTaskStatusAsync(
        int userId, int taskId, UpdateTaskStatusDto dto, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, userId, ct);
        if (task == null)
            return ApiResponse<TaskResponseDto>.Fail("Task not found.");

        task.IsCompleted = dto.IsCompleted;
        task.UpdatedDate = DateTime.UtcNow;
        task.CompletedDate = dto.IsCompleted ? DateTime.UtcNow : null;

        await _taskRepository.UpdateAsync(task, ct);
        return ApiResponse<TaskResponseDto>.Ok(MapToDto(task), "Task status updated successfully.");
    }

    /// <summary>Soft-deletes a task by setting IsDeleted to true.</summary>
    public async Task<ApiResponse<object>> DeleteTaskAsync(
        int userId, int taskId, CancellationToken ct = default)
    {
        var deleted = await _taskRepository.SoftDeleteAsync(taskId, userId, ct);
        if (!deleted)
            return ApiResponse<object>.Fail("Task not found.");

        _logger.LogInformation("Task {TaskId} soft-deleted by user {UserId}", taskId, userId);
        return ApiResponse<object>.Ok(new object(), "Task deleted successfully.");
    }

    private static TaskResponseDto MapToDto(TaskItem task) => new()
    {
        Id = task.Id,
        Task = task.Task,
        IsCompleted = task.IsCompleted,
        CreatedDate = task.CreatedDate,
        CompletedDate = task.CompletedDate
    };
}
