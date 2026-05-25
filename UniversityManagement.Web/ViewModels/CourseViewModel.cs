using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Web.ViewModels;

public class CourseViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(1, 20)]
    public int Credits { get; set; }

    public int TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public int StudentsCount { get; set; }

    public string? Description { get; set; }
}