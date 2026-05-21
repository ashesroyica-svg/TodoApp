using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Todo;

/// <summary>Data transfer object for creating a new task.</summary>
public class CreateTaskDto
{
    [Required(ErrorMessage = "Task description is required.")]
    [MinLength(1, ErrorMessage = "Task cannot be empty.")]
    [MaxLength(500, ErrorMessage = "Task cannot exceed 500 characters.")]
    public string Task { get; set; } = string.Empty;
}
