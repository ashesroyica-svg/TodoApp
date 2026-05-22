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
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _logger = logger;
    }

    /// <summary>Returns a paginated list of tasks for the user with optional keyword search and project filter.</summary>
    public async Task<ApiResponse<PaginatedResultDto<TaskResponseDto>>> GetTasksAsync(
        int userId, int page, int pageSize, string? search, int? projectId = null, CancellationToken ct = default)
    {
        var totalCount = await _taskRepository.GetTotalCountAsync(userId, search, projectId, ct);
        var tasks = await _taskRepository.GetPagedAsync(userId, page, pageSize, search, projectId, ct);
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
        if (dto.ProjectId.HasValue)
        {
            var project = await _projectRepository.GetByIdAsync(dto.ProjectId.Value, userId, ct);
            if (project == null)
                return ApiResponse<TaskResponseDto>.Fail("The selected project was not found.");
        }

        var task = new TaskItem
        {
            Task = dto.Task,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId,
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

    /// <summary>Aggregates task counts and computes today's completion percentage plus project summaries.</summary>
    public async Task<ApiResponse<DashboardDto>> GetDashboardAsync(int userId, CancellationToken ct = default)
    {
        var (total, completed, remaining, completedToday, overdue, highPriority) =
            await _taskRepository.GetDashboardStatsAsync(userId, ct);

        var projects = await _projectRepository.GetAllByUserAsync(userId, ct);

        var projectSummaries = projects.Select(p =>
        {
            var taskCount = p.Tasks.Count;
            var completedCount = p.Tasks.Count(t => t.IsCompleted);
            return new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Color = p.Color,
                TaskCount = taskCount,
                CompletedCount = completedCount,
                CompletionPercentage = taskCount == 0 ? 0 : Math.Round((double)completedCount / taskCount * 100, 1)
            };
        }).ToList();

        var dto = new DashboardDto
        {
            TotalTasks = total,
            CompletedTasks = completed,
            RemainingTasks = remaining,
            CompletedToday = completedToday,
            CompletedTodayPercentage = total == 0 ? 0 : Math.Round((double)completedToday / total * 100, 1),
            OverdueTasks = overdue,
            HighPriorityTasks = highPriority,
            TotalProjects = projects.Count,
            ProjectSummaries = projectSummaries
        };

        return ApiResponse<DashboardDto>.Ok(dto, "Dashboard loaded successfully.");
    }

    private static TaskResponseDto MapToDto(TaskItem task)
    {
        var now = DateTime.UtcNow;
        return new TaskResponseDto
        {
            Id = task.Id,
            Task = task.Task,
            Description = task.Description,
            Priority = task.Priority,
            PriorityLabel = task.Priority.ToString(),
            DueDate = task.DueDate,
            IsOverdue = !task.IsCompleted && task.DueDate.HasValue && task.DueDate.Value < now,
            IsCompleted = task.IsCompleted,
            CreatedDate = task.CreatedDate,
            CompletedDate = task.CompletedDate,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name,
            ProjectColor = task.Project?.Color
        };
    }
}
