namespace UniversityManagement.Core.Models;

public class Report
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string GeneratedBy { get; set; } = string.Empty;

    public string ReportType { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;
}