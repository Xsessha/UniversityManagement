namespace UniversityManagement.Core.Models;

public class Faculty
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Group> Groups { get; set; } = new();
}