using UniversityManagement.Core.Enums;

namespace UniversityManagement.Core.Models;

public class Schedule
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public LessonType LessonType { get; set; }

    public int CourseId { get; set; }

    public Course? Course { get; set; }

    public int TeacherId { get; set; }

    public Teacher? Teacher { get; set; }

    public int GroupId { get; set; }

    public Group? Group { get; set; }
}
