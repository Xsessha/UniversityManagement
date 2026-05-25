namespace UniversityManagement.Web.ViewModels;

public class DashboardViewModel
{
    public int TotalStudents { get; set; }

    public int TotalTeachers { get; set; }

    public int TotalCourses { get; set; }

    public int TotalGroups { get; set; }

    public int TotalFaculties { get; set; }

    public double AverageRating { get; set; }

    public List<StudentViewModel> TopStudents { get; set; } = new();

    public List<CourseViewModel> PopularCourses { get; set; } = new();
}