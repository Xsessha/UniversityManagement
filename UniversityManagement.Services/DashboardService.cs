using UniversityManagement.Core.Models;
using UniversityManagement.Core.Interfaces;

namespace UniversityManagement.Services;

public class DashboardService
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<Teacher> _teacherRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Lesson> _lessonRepository;
    private readonly IRepository<Notification> _notificationRepository;

    public DashboardService(
        IRepository<Student> studentRepository,
        IRepository<Teacher> teacherRepository,
        IRepository<Course> courseRepository,
        IRepository<Lesson> lessonRepository,
        IRepository<Notification> notificationRepository)
    {
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        var teachers = await _teacherRepository.GetAllAsync();
        var courses = await _courseRepository.GetAllAsync();
        var lessons = await _lessonRepository.GetAllAsync();
        var notifications = await _notificationRepository.GetAllAsync();

        var today = DateTime.Today;
        var todayLessons = lessons.Where(l => l.Date.Date == today).ToList();

        return new DashboardViewModel
        {
            TotalStudents = students.Count,
            TotalTeachers = teachers.Count,
            TotalCourses = courses.Count,
            TodayLessons = todayLessons,
            RecentNotifications = notifications.OrderByDescending(n => n.CreatedAt).Take(5).ToList()
        };
    }
}

public class DashboardViewModel
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalCourses { get; set; }
    public List<Lesson> TodayLessons { get; set; } = new();
    public List<Notification> RecentNotifications { get; set; } = new();
}