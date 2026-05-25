namespace UniversityManagement.Web.ViewModels;

public class DashboardViewModel
{
    public int StudentsCount { get; set; }
    public int TeachersCount { get; set; }
    public int CoursesCount { get; set; }
    public int GroupsCount { get; set; }

    public double AverageRating { get; set; }
}