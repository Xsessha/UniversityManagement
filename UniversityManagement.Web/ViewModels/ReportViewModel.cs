namespace UniversityManagement.Web.ViewModels;

public class ReportViewModel
{
    public string Title { get; set; } = "";

    public int StudentsCount { get; set; }
    public int TeachersCount { get; set; }
    public int CoursesCount { get; set; }

    public double AverageRating { get; set; }
}