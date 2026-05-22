using System.Security.Claims;
using Application.DTOs.Todo;
using Application.ServiceInterfaces;
using Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Todo.API.Controllers;

/// <summary>Handles all task management operations for authenticated users.</summary>
[ApiController]
[Route("api/tasks")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>Extracts the authenticated user's ID from the JWT NameIdentifier claim.</summary>
    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User ID claim not found in token.");
        return int.Parse(userIdClaim.Value);
    }

    /// <summary>Returns a paginated list of tasks, with optional keyword search and project filter.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResultDto<TaskResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTasks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 8,
        [FromQuery] string? search = null,
        [FromQuery] int? projectId = null,
        CancellationToken ct = default)
    {
        var userId = GetUserIdFromToken();
        var result = await _taskService.GetTasksAsync(userId, page, pageSize, search, projectId, ct);
        return Ok(result);
    }

    /// <summary>Creates a new task for the authenticated user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<TaskResponseDto>.Fail("Validation failed.", errors));
        }

        var userId = GetUserIdFromToken();
        var result = await _taskService.CreateTaskAsync(userId, dto, ct);
        return CreatedAtAction(nameof(GetTasks), result);
    }

    /// <summary>Updates the IsCompleted status of a specific task.</summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTaskStatus(
        int id, [FromBody] UpdateTaskStatusDto dto, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var result = await _taskService.UpdateTaskStatusAsync(userId, id, dto, ct);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>Returns task statistics for the authenticated user's dashboard.</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<DashboardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var result = await _taskService.GetDashboardAsync(userId, ct);
        return Ok(result);
    }

    /// <summary>Soft-deletes a task by setting IsDeleted to true.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var result = await _taskService.DeleteTaskAsync(userId, id, ct);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
