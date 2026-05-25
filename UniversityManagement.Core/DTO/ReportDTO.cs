namespace UniversityManagement.Core.DTO;

public class ReportDTO
{
    public string Title { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }

    public string Content { get; set; } = string.Empty;
}