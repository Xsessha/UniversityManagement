using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Web.ViewModels;

namespace UniversityManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UniversityDbContext _context;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        UniversityDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToListAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Groups = await _context.Groups.Include(g => g.Faculty).OrderBy(g => g.Name).ToListAsync();
        return View(new AdminCreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminCreateUserViewModel model)
    {
        ViewBag.Groups = await _context.Groups.Include(g => g.Faculty).OrderBy(g => g.Name).ToListAsync();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Role != UserRole.Student && model.Role != UserRole.Teacher)
        {
            ModelState.AddModelError(nameof(model.Role), "Only Student and Teacher accounts can be created from this admin panel.");
            return View(model);
        }

        if (await _userManager.FindByEmailAsync(model.Email.Trim()) != null)
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
            return View(model);
        }

        if (model.Role == UserRole.Student && model.GroupId <= 0)
        {
            model.GroupId = await _context.Groups.OrderBy(g => g.Id).Select(g => g.Id).FirstOrDefaultAsync();
        }

        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Role = model.Role,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        if (!await _roleManager.RoleExistsAsync(model.Role.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(model.Role.ToString()));
        }

        await _userManager.AddToRoleAsync(user, model.Role.ToString());

        if (model.Role == UserRole.Student)
        {
            await CreateStudentProfileAsync(user, model.GroupId);
        }
        else
        {
            await CreateTeacherProfileAsync(user, model.Department);
        }

        TempData["SuccessMessage"] = $"{model.Role} account for {model.FirstName} {model.LastName} was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        ViewBag.Roles = Enum.GetValues<UserRole>();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, string firstName, string lastName, string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<UserRole>(role, out var parsedRole))
        {
            ModelState.AddModelError(nameof(role), "Select a valid role.");
            ViewBag.Roles = Enum.GetValues<UserRole>();
            return View(user);
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Role = parsedRole;

        var existingRoles = await _userManager.GetRolesAsync(user);
        if (existingRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, existingRoles);
        }

        await _userManager.AddToRoleAsync(user, parsedRole.ToString());
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    private async Task CreateStudentProfileAsync(ApplicationUser user, int groupId)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);

        var student = new Student
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            GroupId = group?.Id ?? 0,
            Rating = 0,
            Status = StudentStatus.Active
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        await CreateWelcomeNotificationAsync(student.Id, "Welcome to Lumina", "Your account is ready. Your schedule and notifications will update automatically.");

        if (group != null)
        {
            await GenerateScheduleForGroupAsync(group.Id);
        }
    }

    private async Task CreateTeacherProfileAsync(ApplicationUser user, string department)
    {
        var teacher = new Teacher
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Department = string.IsNullOrWhiteSpace(department) ? "General" : department
        };

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        await CreateWelcomeNotificationAsync(0, "Teacher workspace ready", "Your teacher dashboard is ready. You can manage attendance, grades, and lesson updates from here.");
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
}
