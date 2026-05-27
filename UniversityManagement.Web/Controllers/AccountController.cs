using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Core.Enums;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UniversityDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        UniversityDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Dashboard");
        }

        ModelState.AddModelError("", "Invalid login credentials");
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string firstName, string lastName, string email, string password, string role)
    {
        if (!Enum.TryParse<UserRole>(role, out var parsedRole))
        {
            ModelState.AddModelError("", "Please select a valid role");
            return View();
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = parsedRole
        };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            var roleName = parsedRole.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);

            if (parsedRole == UserRole.Student)
            {
                await CreateStudentProfileAsync(user, parsedRole);
            }
            else if (parsedRole == UserRole.Teacher)
            {
                await CreateTeacherProfileAsync(user, parsedRole);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Dashboard");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View();
    }

    private async Task CreateStudentProfileAsync(ApplicationUser user, UserRole role)
    {
        var group = await _context.Groups.OrderBy(g => g.Id).FirstOrDefaultAsync();

        var student = new Student
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            GroupId = group?.Id ?? 0,
            Rating = 0,
            Status = UniversityManagement.Core.Enums.StudentStatus.Active
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        await CreateWelcomeNotificationAsync(student.Id, "Welcome to Lumina", "Your student dashboard is ready. Your schedule and notifications will update automatically.");

        if (group != null)
        {
            await GenerateScheduleForGroupAsync(group.Id);
        }
    }

    private async Task CreateTeacherProfileAsync(ApplicationUser user, UserRole role)
    {
        var teacher = new Teacher
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Department = "General"
        };

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        await CreateWelcomeNotificationAsync(0, "Teacher workspace ready", "Your teacher dashboard is ready. You can manage attendance, grades, and lesson updates from here.");

        var courseIds = await _context.Courses
            .Where(c => c.TeacherId == teacher.Id)
            .Select(c => c.Id)
            .ToListAsync();

        if (courseIds.Count == 0)
        {
            return;
        }

        var lessons = await _context.Lessons
            .Where(l => courseIds.Contains(l.CourseId))
            .OrderBy(l => l.Date)
            .Take(10)
            .ToListAsync();

        foreach (var lesson in lessons)
        {
            var groups = await _context.Courses
                .Where(c => c.Id == lesson.CourseId)
                .SelectMany(c => c.Groups)
                .ToListAsync();

            foreach (var group in groups)
            {
                if (!await _context.Schedules.AnyAsync(s => s.GroupId == group.Id && s.LessonId == lesson.Id))
                {
                    _context.Schedules.Add(new Schedule
                    {
                        GroupId = group.Id,
                        LessonId = lesson.Id,
                        DayOfWeek = lesson.Date.DayOfWeek.ToString(),
                        StartTime = lesson.Date.ToString("HH:mm"),
                        EndTime = lesson.Date.AddMinutes(90).ToString("HH:mm")
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task CreateWelcomeNotificationAsync(int studentId, string title, string message)
    {
        _context.Notifications.Add(new Notification
        {
            Title = title,
            Message = message,
            StudentId = studentId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        });

        await _context.SaveChangesAsync();
    }

    private async Task GenerateScheduleForGroupAsync(int groupId)
    {
        var lessonIds = await _context.Courses
            .Where(c => c.Groups.Any(g => g.Id == groupId))
            .SelectMany(c => c.Lessons)
            .Select(l => l.Id)
            .ToListAsync();

        var lessons = await _context.Lessons
            .Where(l => lessonIds.Contains(l.Id))
            .OrderBy(l => l.Date)
            .Take(20)
            .ToListAsync();

        foreach (var lesson in lessons)
        {
            if (await _context.Schedules.AnyAsync(s => s.GroupId == groupId && s.LessonId == lesson.Id))
            {
                continue;
            }

            _context.Schedules.Add(new Schedule
            {
                GroupId = groupId,
                LessonId = lesson.Id,
                DayOfWeek = lesson.Date.DayOfWeek.ToString(),
                StartTime = lesson.Date.ToString("HH:mm"),
                EndTime = lesson.Date.AddMinutes(90).ToString("HH:mm")
            });
        }

        await _context.SaveChangesAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
}
