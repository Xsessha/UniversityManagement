namespace UniversityManagement.Core.Models;

public class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Credits { get; set; }

    public string? Description { get; set; }

    public int TeacherId { get; set; }

    public Teacher? Teacher { get; set; }

    public List<Student> Students { get; set; } = new();

    public List<Lesson> Lessons { get; set; } = new();
}