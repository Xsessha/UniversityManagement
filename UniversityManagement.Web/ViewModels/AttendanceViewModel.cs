using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Web.ViewModels;

public class AttendanceViewModel
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    public bool IsPresent { get; set; }

    public string Status => IsPresent ? "Present" : "Absent";
}