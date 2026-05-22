using Application.DTOs.Project;
using Application.Wrappers;

namespace Application.ServiceInterfaces;

/// <summary>Service interface for project management operations.</summary>
public interface IProjectService
{
    /// <summary>Returns all projects for the authenticated user.</summary>
    Task<ApiResponse<List<ProjectResponseDto>>> GetProjectsAsync(int userId, CancellationToken ct = default);

    /// <summary>Creates a new project for the authenticated user.</summary>
    Task<ApiResponse<ProjectResponseDto>> CreateProjectAsync(int userId, CreateProjectDto dto, CancellationToken ct = default);

    /// <summary>Updates an existing project owned by the user.</summary>
    Task<ApiResponse<ProjectResponseDto>> UpdateProjectAsync(int userId, int projectId, UpdateProjectDto dto, CancellationToken ct = default);

    /// <summary>Soft-deletes a project owned by the user.</summary>
    Task<ApiResponse<object>> DeleteProjectAsync(int userId, int projectId, CancellationToken ct = default);
}
