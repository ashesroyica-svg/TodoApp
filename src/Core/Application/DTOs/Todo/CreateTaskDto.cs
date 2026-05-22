using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.DTOs.Todo;

/// <summary>Data transfer object for creating a new task.</summary>
public class CreateTaskDto
{
    [Required(ErrorMessage = "Task description is required.")]
    [MinLength(1, ErrorMessage = "Task cannot be empty.")]
    [MaxLength(500, ErrorMessage = "Task cannot exceed 500 characters.")]
    public string Task { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    public int? ProjectId { get; set; }
}
