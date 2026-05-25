namespace UniversityManagement.Core.Models;

public class Report
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }

    public string Content { get; set; } = string.Empty;
}