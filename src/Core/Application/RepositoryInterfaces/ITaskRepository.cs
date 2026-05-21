using Domain.Entities;

namespace Application.RepositoryInterfaces;

/// <summary>Repository interface for task data access operations.</summary>
public interface ITaskRepository
{
    /// <summary>Retrieves a paginated page of non-deleted tasks for a user, with optional search.</summary>
    Task<List<TaskItem>> GetPagedAsync(int userId, int page, int pageSize, string? search, CancellationToken ct = default);

    /// <summary>Returns the total count of non-deleted tasks for a user, with optional search filter.</summary>
    Task<int> GetTotalCountAsync(int userId, string? search, CancellationToken ct = default);

    /// <summary>Persists a new task to the database.</summary>
    Task<TaskItem> CreateAsync(TaskItem task, CancellationToken ct = default);

    /// <summary>Retrieves a specific task by ID, scoped to the given user and excluding soft-deleted records.</summary>
    Task<TaskItem?> GetByIdAsync(int id, int userId, CancellationToken ct = default);

    /// <summary>Saves changes to an existing task record.</summary>
    Task UpdateAsync(TaskItem task, CancellationToken ct = default);

    /// <summary>Soft-deletes a task by setting IsDeleted to true. Returns false if the task was not found.</summary>
    Task<bool> SoftDeleteAsync(int id, int userId, CancellationToken ct = default);
}
