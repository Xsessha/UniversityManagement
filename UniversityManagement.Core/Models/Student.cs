namespace UniversityManagement.Core.Models;

using UniversityManagement.Core.Enums;

public class Student
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public StudentStatus Status { get; set; } = StudentStatus.Active;

    public double Rating { get; set; }

    public int GroupId { get; set; }

    public Group? Group { get; set; }

    public List<Grade> Grades { get; set; } = new();

    public List<Attendance> Attendances { get; set; } = new();
}