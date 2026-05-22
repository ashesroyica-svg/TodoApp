using Domain.Entities;

namespace Application.RepositoryInterfaces;

/// <summary>Repository interface for project data access operations.</summary>
public interface IProjectRepository
{
    /// <summary>Returns all non-deleted projects for the given user, including task counts.</summary>
    Task<List<Project>> GetAllByUserAsync(int userId, CancellationToken ct = default);

    /// <summary>Retrieves a specific non-deleted project scoped to the given user.</summary>
    Task<Project?> GetByIdAsync(int id, int userId, CancellationToken ct = default);

    /// <summary>Persists a new project and returns the saved entity.</summary>
    Task<Project> CreateAsync(Project project, CancellationToken ct = default);

    /// <summary>Saves changes to an existing project record.</summary>
    Task UpdateAsync(Project project, CancellationToken ct = default);

    /// <summary>Soft-deletes a project. Returns false if the project was not found.</summary>
    Task<bool> SoftDeleteAsync(int id, int userId, CancellationToken ct = default);

    /// <summary>Returns the total number of active (non-deleted) projects for the user.</summary>
    Task<int> GetCountAsync(int userId, CancellationToken ct = default);
}
