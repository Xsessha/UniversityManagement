namespace UniversityManagement.Core.Models;

public class Grade
{
    public int Id { get; set; }

    public int Value { get; set; }

    public DateTime Date { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public int CourseId { get; set; }

    public Course? Course { get; set; }
}