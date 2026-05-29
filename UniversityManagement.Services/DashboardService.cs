using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Services;

public class DashboardService
{
    private readonly IRepository<Student>        _studentRepository;
    private readonly IRepository<Teacher>        _teacherRepository;
    private readonly IRepository<Course>         _courseRepository;
    private readonly IRepository<Group>          _groupRepository;
    private readonly IRepository<Attendance>     _attendanceRepository;
    private readonly IRepository<Lesson>         _lessonRepository;
    private readonly IRepository<Notification>   _notificationRepository;
    private readonly UniversityDbContext          _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(
        IRepository<Student>        studentRepository,
        IRepository<Teacher>        teacherRepository,
        IRepository<Course>         courseRepository,
        IRepository<Group>          groupRepository,
        IRepository<Attendance>     attendanceRepository,
        IRepository<Lesson>         lessonRepository,
        IRepository<Notification>   notificationRepository,
        UniversityDbContext          context,
        UserManager<ApplicationUser> userManager)
    {
        _studentRepository      = studentRepository;
        _teacherRepository      = teacherRepository;
        _courseRepository       = courseRepository;
        _groupRepository        = groupRepository;
        _attendanceRepository   = attendanceRepository;
        _lessonRepository       = lessonRepository;
        _notificationRepository = notificationRepository;
        _context                = context;
        _userManager            = userManager;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(string? userEmail = null)
    {
        var students      = await _studentRepository.GetAllAsync();
        var teachers      = await _teacherRepository.GetAllAsync();
        var courses       = await _courseRepository.GetAllAsync();
        var groups        = await _groupRepository.GetAllAsync();
        var attendance    = await _attendanceRepository.GetAllAsync();
        var lessons       = await _lessonRepository.GetAllAsync();
        var notifications = await _notificationRepository.GetAllAsync();

        var today        = DateTime.Today;
        var todayLessons = lessons.Where(l => l.Date.Date == today).ToList();

        var roleName     = "Admin";
        var roleSummary  = "University-wide analytics, schedules, notifications, and academic reporting.";
        var quickSummary = new List<string>
        {
            "Monitor university-wide activity",
            "Keep schedules and notifications synchronized",
            "Review academic analytics in real time"
        };

        if (string.IsNullOrWhiteSpace(userEmail))
            return BuildAdminViewModel(roleName, roleSummary, quickSummary,
                students, teachers, courses, groups, attendance, todayLessons, notifications);

        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return BuildAdminViewModel(roleName, roleSummary, quickSummary,
                students, teachers, courses, groups, attendance, todayLessons, notifications);

        roleName = user.Role.ToString();

        // ── STUDENT ──────────────────────────────────────────────────────────
        if (user.Role == UserRole.Student)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == user.Email);

            if (student != null)
            {
                var studentAttendance = await _context.Attendances
                    .Where(a => a.StudentId == student.Id)
                    .ToListAsync();

                var studentGrades = await _context.Grades
                    .Where(g => g.StudentId == student.Id)
                    .ToListAsync();

                var studentNotifications = await _context.Notifications
                    .Where(n => n.StudentId == student.Id)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                var studentGroupId = student.GroupId;

                var studentLessons = await _context.Lessons
                    .Include(l => l.Course)
                    .Where(l => _context.Courses
                        .Any(c => c.Id == l.CourseId &&
                                  c.Groups.Any(g => g.Id == studentGroupId)))
                    .OrderBy(l => l.Date)
                    .Take(10)
                    .ToListAsync();

                var activeCourseCount = await _context.Courses
                    .CountAsync(c => c.Groups.Any(g => g.Id == studentGroupId));

                return new DashboardViewModel
                {
                    RoleName            = roleName,
                    RoleSummary         = "Personal student workspace: grades, attendance, schedule, and notifications.",
                    QuickSummary        = new List<string>
                    {
                        "Track your latest grades and GPA",
                        "Review attendance status and upcoming lessons",
                        "Receive live updates from your teachers"
                    },
                    TotalStudents       = students.Count,
                    TotalTeachers       = teachers.Count,
                    TotalCourses        = courses.Count,
                    TotalGroups         = groups.Count,
                    AverageRating       = studentGrades.Any()
                                            ? studentGrades.Average(g => g.Value)
                                            : student.Rating,
                    AttendanceRate      = studentAttendance.Any()
                                            ? studentAttendance.Count(a => a.Status == AttendanceStatus.Present)
                                              * 100.0 / studentAttendance.Count
                                            : 0,
                    TodayLessons        = studentLessons.Where(l => l.Date.Date == today).ToList(),
                    RecentNotifications = studentNotifications.Take(5).ToList(),
                    CurrentStudent      = student,
                    ActiveCourseCount   = activeCourseCount,
                    NotificationCount   = studentNotifications.Count,
                    CurrentRating       = student.Rating
                };
            }
        }

        // ── TEACHER ──────────────────────────────────────────────────────────
        if (user.Role == UserRole.Teacher)
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == user.Email);

            if (teacher != null)
            {
                var teacherCourses = await _context.Courses
                    .Where(c => c.TeacherId == teacher.Id)
                    .ToListAsync();

                var teacherCourseIds = teacherCourses.Select(c => c.Id).ToList();

                var teacherGroups = await _context.Groups
                    .Where(g => g.Courses.Any(c => teacherCourseIds.Contains(c.Id)))
                    .ToListAsync();

                var teacherLessons = await _context.Lessons
                    .Where(l => teacherCourseIds.Contains(l.CourseId))
                    .ToListAsync();

                var teacherNotifications = await _context.Notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(10)
                    .ToListAsync();

                return new DashboardViewModel
                {
                    RoleName            = roleName,
                    RoleSummary         = "Teacher workspace: manage classes, attendance, ratings, and group performance.",
                    QuickSummary        = new List<string>
                    {
                        "Review today's lessons and assigned groups",
                        "Update attendance and grades for your classes",
                        "Send academic updates to students instantly"
                    },
                    TotalStudents       = students.Count,
                    TotalTeachers       = teachers.Count,
                    TotalCourses        = teacherCourses.Count,
                    TotalGroups         = teacherGroups.Count,
                    AverageRating       = teacherCourses.Any()
                                            ? teacherCourses.Average(c => c.Credits)
                                            : 0,
                    AttendanceRate      = teacherLessons.Any() ? 100 : 0,
                    TodayLessons        = teacherLessons.Where(l => l.Date.Date == today).ToList(),
                    RecentNotifications = teacherNotifications,
                    CurrentTeacher      = teacher,
                    ActiveCourseCount   = teacherCourses.Count,
                    NotificationCount   = teacherNotifications.Count
                };
            }
        }

        // ── ADMIN fallback ───────────────────────────────────────────────────
        return BuildAdminViewModel(roleName, roleSummary, quickSummary,
            students, teachers, courses, groups, attendance, todayLessons, notifications);
    }

    private static DashboardViewModel BuildAdminViewModel(
        string           roleName,
        string           roleSummary,
        List<string>     quickSummary,
        List<Student>    students,
        List<Teacher>    teachers,
        List<Course>     courses,
        List<Group>      groups,
        List<Attendance> attendance,
        List<Lesson>     todayLessons,
        List<Notification> notifications)
    {
        return new DashboardViewModel
        {
            RoleName            = roleName,
            RoleSummary         = roleSummary,
            QuickSummary        = quickSummary,
            TotalStudents       = students.Count,
            TotalTeachers       = teachers.Count,
            TotalCourses        = courses.Count,
            TotalGroups         = groups.Count,
            AverageRating       = students.Any() ? students.Average(s => s.Rating) : 0,
            AttendanceRate      = attendance.Any()
                                    ? attendance.Count(a => a.Status == AttendanceStatus.Present)
                                      * 100.0 / attendance.Count
                                    : 0,
            TodayLessons        = todayLessons,
            RecentNotifications = notifications.OrderByDescending(n => n.CreatedAt).Take(5).ToList(),
            ActiveCourseCount   = courses.Count,
            NotificationCount   = notifications.Count,
            CurrentRating       = students.Any() ? students.Average(s => s.Rating) : 0
        };
    }
}

// ViewModel живе тут — в Services, щоб уникнути circular dependency
public class DashboardViewModel
{
    public string       RoleName     { get; set; } = "Admin";
    public string       RoleSummary  { get; set; } = "";
    public List<string> QuickSummary { get; set; } = new();

    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalCourses  { get; set; }
    public int TotalGroups   { get; set; }

    public double AverageRating  { get; set; }
    public double AttendanceRate { get; set; }

    public List<Lesson>       TodayLessons        { get; set; } = new();
    public List<Notification> RecentNotifications { get; set; } = new();

    public Student? CurrentStudent { get; set; }
    public Teacher? CurrentTeacher { get; set; }

    public int    ActiveCourseCount { get; set; }
    public int    NotificationCount { get; set; }
    public double CurrentRating     { get; set; }
}