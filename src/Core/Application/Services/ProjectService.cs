using Application.DTOs.Project;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using Application.Wrappers;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>Handles all project management business logic.</summary>
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IProjectRepository projectRepository, ILogger<ProjectService> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    /// <summary>Returns all projects for the user, including task counts.</summary>
    public async Task<ApiResponse<List<ProjectResponseDto>>> GetProjectsAsync(int userId, CancellationToken ct = default)
    {
        var projects = await _projectRepository.GetAllByUserAsync(userId, ct);
        var dtos = projects.Select(MapToDto).ToList();
        return ApiResponse<List<ProjectResponseDto>>.Ok(dtos, "Projects retrieved successfully.");
    }

    /// <summary>Creates a new project for the user.</summary>
    public async Task<ApiResponse<ProjectResponseDto>> CreateProjectAsync(
        int userId, CreateProjectDto dto, CancellationToken ct = default)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            Color = dto.Color,
            UserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _projectRepository.CreateAsync(project, ct);
        _logger.LogInformation("Project created for user {UserId}: {ProjectId}", userId, created.Id);
        return ApiResponse<ProjectResponseDto>.Ok(MapToDto(created), "Project created successfully.");
    }

    /// <summary>Updates an existing project owned by the user.</summary>
    public async Task<ApiResponse<ProjectResponseDto>> UpdateProjectAsync(
        int userId, int projectId, UpdateProjectDto dto, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, userId, ct);
        if (project == null)
            return ApiResponse<ProjectResponseDto>.Fail("Project not found.");

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.Color = dto.Color;
        project.UpdatedDate = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project, ct);
        return ApiResponse<ProjectResponseDto>.Ok(MapToDto(project), "Project updated successfully.");
    }

    /// <summary>Soft-deletes a project owned by the user.</summary>
    public async Task<ApiResponse<object>> DeleteProjectAsync(
        int userId, int projectId, CancellationToken ct = default)
    {
        var deleted = await _projectRepository.SoftDeleteAsync(projectId, userId, ct);
        if (!deleted)
            return ApiResponse<object>.Fail("Project not found.");

        _logger.LogInformation("Project {ProjectId} soft-deleted by user {UserId}", projectId, userId);
        return ApiResponse<object>.Ok(new object(), "Project deleted successfully.");
    }

    private static ProjectResponseDto MapToDto(Project project) => new()
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        Color = project.Color,
        TaskCount = project.Tasks.Count,
        CompletedTaskCount = project.Tasks.Count(t => t.IsCompleted),
        CreatedDate = project.CreatedDate
    };
}
