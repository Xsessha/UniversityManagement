using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Web.ViewModels;

public class TeacherViewModel
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    public int CoursesCount { get; set; }

    public string? ImageUrl { get; set; }

    public List<string> Courses { get; set; } = new();
}