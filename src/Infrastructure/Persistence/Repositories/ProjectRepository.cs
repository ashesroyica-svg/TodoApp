using Application.RepositoryInterfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

/// <summary>EF Core implementation of the project repository.</summary>
public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Returns all non-deleted projects for the user, ordered by creation date descending.</summary>
    public async Task<List<Project>> GetAllByUserAsync(int userId, CancellationToken ct = default)
        => await _context.Projects
            .Include(p => p.Tasks.Where(t => !t.IsDeleted))
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync(ct);

    /// <summary>Retrieves a specific non-deleted project scoped to the given user.</summary>
    public async Task<Project?> GetByIdAsync(int id, int userId, CancellationToken ct = default)
        => await _context.Projects
            .Include(p => p.Tasks.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted, ct);

    /// <summary>Persists a new project and returns the saved entity.</summary>
    public async Task<Project> CreateAsync(Project project, CancellationToken ct = default)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(ct);
        return project;
    }

    /// <summary>Saves changes to an existing project record.</summary>
    public async Task UpdateAsync(Project project, CancellationToken ct = default)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(ct);
    }

    /// <summary>Sets IsDeleted to true on the specified project. Returns false if not found.</summary>
    public async Task<bool> SoftDeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted, ct);

        if (project == null) return false;

        project.IsDeleted = true;
        project.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>Returns the total number of active projects for the user.</summary>
    public async Task<int> GetCountAsync(int userId, CancellationToken ct = default)
        => await _context.Projects.CountAsync(p => p.UserId == userId && !p.IsDeleted, ct);
}
