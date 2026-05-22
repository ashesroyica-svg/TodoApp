using System.Security.Claims;
using Application.DTOs.Project;
using Application.ServiceInterfaces;
using Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Todo.API.Controllers;

/// <summary>Handles all project management operations for authenticated users.</summary>
[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController> _logger;

    public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>Extracts the authenticated user's ID from the JWT NameIdentifier claim.</summary>
    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User ID claim not found in token.");
        return int.Parse(userIdClaim.Value);
    }

    /// <summary>Returns all projects for the authenticated user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProjectResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var result = await _projectService.GetProjectsAsync(userId, ct);
        return Ok(result);
    }

    /// <summary>Creates a new project for the authenticated user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<ProjectResponseDto>.Fail("Validation failed.", errors));
        }

        var userId = GetUserIdFromToken();
        var result = await _projectService.CreateProjectAsync(userId, dto, ct);
        return CreatedAtAction(nameof(GetProjects), result);
    }

    /// <summary>Updates an existing project owned by the authenticated user.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<ProjectResponseDto>.Fail("Validation failed.", errors));
        }

        var userId = GetUserIdFromToken();
        var result = await _projectService.UpdateProjectAsync(userId, id, dto, ct);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>Soft-deletes a project owned by the authenticated user.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(int id, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var result = await _projectService.DeleteProjectAsync(userId, id, ct);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
