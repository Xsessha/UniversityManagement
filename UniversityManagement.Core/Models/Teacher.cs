namespace UniversityManagement.Core.Models;

public class Teacher
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public List<Course> Courses { get; set; } = new();
}