namespace UniversityManagement.Core.Models;

using UniversityManagement.Core.Enums;

public class Attendance
{
    public int Id { get; set; }

    public AttendanceStatus Status { get; set; }

    public DateTime Date { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public int LessonId { get; set; }

    public Lesson? Lesson { get; set; }
}