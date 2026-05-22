using Application.RepositoryInterfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

/// <summary>EF Core implementation of the task repository.</summary>
public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Returns a paged slice of non-deleted tasks for a user, ordered by creation date descending.</summary>
    public async Task<List<TaskItem>> GetPagedAsync(
        int userId, int page, int pageSize, string? search, int? projectId = null, CancellationToken ct = default)
    {
        var query = _context.Tasks
            .Include(t => t.Project)
            .Where(t => t.UserId == userId && !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Task.Contains(search) || (t.Description != null && t.Description.Contains(search)));

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        return await query
            .OrderByDescending(t => t.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    /// <summary>Returns the total count of non-deleted tasks matching the optional search and project filters.</summary>
    public async Task<int> GetTotalCountAsync(int userId, string? search, int? projectId = null, CancellationToken ct = default)
    {
        var query = _context.Tasks
            .Where(t => t.UserId == userId && !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Task.Contains(search) || (t.Description != null && t.Description.Contains(search)));

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        return await query.CountAsync(ct);
    }

    /// <summary>Persists a new task and returns the saved entity with project navigation loaded.</summary>
    public async Task<TaskItem> CreateAsync(TaskItem task, CancellationToken ct = default)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(ct);

        if (task.ProjectId.HasValue)
            await _context.Entry(task).Reference(t => t.Project).LoadAsync(ct);

        return task;
    }

    /// <summary>Retrieves a specific non-deleted task scoped to the given user, including its project.</summary>
    public async Task<TaskItem?> GetByIdAsync(int id, int userId, CancellationToken ct = default)
        => await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && !t.IsDeleted, ct);

    /// <summary>Saves changes to an existing task record.</summary>
    public async Task UpdateAsync(TaskItem task, CancellationToken ct = default)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(ct);
    }

    /// <summary>Sets IsDeleted to true on the specified task. Returns false if the task was not found.</summary>
    public async Task<bool> SoftDeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var task = await GetByIdAsync(id, userId, ct);
        if (task == null) return false;

        task.IsDeleted = true;
        task.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>Returns aggregate task counts for the dashboard in a single pass.</summary>
    public async Task<(int Total, int Completed, int Remaining, int CompletedToday, int Overdue, int HighPriority)> GetDashboardStatsAsync(
        int userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var baseQuery = _context.Tasks.Where(t => t.UserId == userId && !t.IsDeleted);

        var total = await baseQuery.CountAsync(ct);
        var completed = await baseQuery.CountAsync(t => t.IsCompleted, ct);
        var completedToday = await baseQuery.CountAsync(
            t => t.IsCompleted && t.CompletedDate != null &&
                 t.CompletedDate.Value >= today && t.CompletedDate.Value < today.AddDays(1), ct);
        var overdue = await baseQuery.CountAsync(
            t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < now, ct);
        var highPriority = await baseQuery.CountAsync(
            t => !t.IsCompleted && t.Priority == TaskPriority.High, ct);

        return (total, completed, total - completed, completedToday, overdue, highPriority);
    }
}
