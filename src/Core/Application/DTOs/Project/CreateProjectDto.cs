using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Project;

/// <summary>Data transfer object for creating a new project.</summary>
public class CreateProjectDto
{
    [Required(ErrorMessage = "Project name is required.")]
    [MinLength(1, ErrorMessage = "Project name cannot be empty.")]
    [MaxLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string Color { get; set; } = "#003087";
}
