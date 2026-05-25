namespace UniversityManagement.Core.Models;

public class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int FacultyId { get; set; }

    public Faculty? Faculty { get; set; }

    public List<Student> Students { get; set; } = new();
}