namespace UniversityManagement.Web.ViewModels;
using UniversityManagement.Core.Models;

public class DashboardViewModel
{
    public string RoleName { get; set; } = "";
    public string RoleSummary { get; set; } = "";
    public List<string> QuickSummary { get; set; } = new();

    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalCourses { get; set; }
    public int TotalGroups { get; set; }

    public double AverageRating { get; set; }
    public double AttendanceRate { get; set; }

    public List<Lesson> TodayLessons { get; set; } = new();
    public List<Notification> RecentNotifications { get; set; } = new();

    public Student? CurrentStudent { get; set; }
    public Teacher? CurrentTeacher { get; set; }

    public int ActiveCourseCount { get; set; }
    public int NotificationCount { get; set; }
    public double CurrentRating { get; set; }
}