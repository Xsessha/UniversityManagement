using UniversityManagement.Core.Enums;

namespace UniversityManagement.Core.Models;

public class Lesson
{
    public int Id { get; set; }

    public string Topic { get; set; } = string.Empty;

    public LessonType LessonType { get; set; }

    public DateTime Date { get; set; }

    public int CourseId { get; set; }

    public Course? Course { get; set; }

    public int TeacherId { get; set; }

    public Teacher? Teacher { get; set; }
}