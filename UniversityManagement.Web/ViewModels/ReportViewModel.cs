namespace UniversityManagement.Web.ViewModels;

public class ReportViewModel
{
    public string Title { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }

    public int TotalStudents { get; set; }

    public int TotalTeachers { get; set; }

    public int TotalCourses { get; set; }

    public double AverageRating { get; set; }

    public List<FacultyViewModel> Faculties { get; set; } = new();

    public List<StudentViewModel> TopStudents { get; set; } = new();

    public List<CourseViewModel> PopularCourses { get; set; } = new();
}