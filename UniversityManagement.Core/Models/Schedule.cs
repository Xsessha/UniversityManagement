namespace UniversityManagement.Core.Models;

public class Schedule
{
    public int Id { get; set; }

    public string DayOfWeek { get; set; } = string.Empty;

    public string StartTime { get; set; } = string.Empty;

    public string EndTime { get; set; } = string.Empty;

    public int GroupId { get; set; }

    public Group? Group { get; set; }

    public int LessonId { get; set; }

    public Lesson? Lesson { get; set; }
}