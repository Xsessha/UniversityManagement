using UniversityManagement.Core.Enums;

namespace UniversityManagement.Core.Models;

public class Attendance
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public AttendanceStatus Status { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }
}