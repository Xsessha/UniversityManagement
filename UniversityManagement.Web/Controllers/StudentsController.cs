using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class StudentsController : Controller
{
    private readonly StudentService _service;
    private readonly UniversityDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public StudentsController(StudentService service, UniversityDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _service = service;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Index()
    {
        var students = await _context.Students
            .Include(s => s.Group)
            .ThenInclude(g => g!.Faculty)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
        return View(students);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        PopulateStudentForm();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Student student, string password)
    {
        if (student.GroupId <= 0 || !await _context.Groups.AnyAsync(g => g.Id == student.GroupId))
        {
            ModelState.AddModelError(nameof(Student.GroupId), "Please select a valid group.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(nameof(password), "Please enter a password for the new student.");
        }

        if (!ModelState.IsValid)
        {
            PopulateStudentForm();
            return View(student);
        }

        var existingUser = await _userManager.FindByEmailAsync(student.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(student.Email), "A user with this email already exists.");
            PopulateStudentForm();
            return View(student);
        }

        var identityUser = new ApplicationUser
        {
            UserName = student.Email,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Role = UserRole.Student
        };

        var createResult = await _userManager.CreateAsync(identityUser, password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            PopulateStudentForm();
            return View(student);
        }

        var studentRole = UserRole.Student.ToString();
        if (!await _roleManager.RoleExistsAsync(studentRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(studentRole));
        }

        await _userManager.AddToRoleAsync(identityUser, studentRole);

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();
        PopulateStudentForm();
        return View(student);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.Id)
            return NotFound();

        if (student.GroupId <= 0 || !await _context.Groups.AnyAsync(g => g.Id == student.GroupId))
        {
            ModelState.AddModelError(nameof(Student.GroupId), "Please select a valid group.");
        }

        if (ModelState.IsValid)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateStudentForm();
        return View(student);
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Details(int id)
    {
        var student = await _context.Students
            .Include(s => s.Group)
            .ThenInclude(g => g!.Faculty)
            .Include(s => s.Grades)
            .ThenInclude(g => g.Course)
            .Include(s => s.Attendances)
            .ThenInclude(a => a.Lesson)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student == null)
            return NotFound();

        var schedule = student.GroupId > 0
            ? await _context.Schedules
                .Where(s => s.GroupId == student.GroupId)
                .Include(s => s.Lesson)
                .ThenInclude(l => l!.Course)
                .ThenInclude(c => c!.Teacher)
                .OrderBy(s => s.Lesson!.Date)
                .ToListAsync()
            : new List<UniversityManagement.Core.Models.Schedule>();

        ViewBag.StudentSchedule = schedule;
        return View(student);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _service.GetByIdAsync(id);
        if (student == null)
            return NotFound();
        return View(student);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteStudentAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private void PopulateStudentForm()
    {
        ViewBag.Groups = _context.Groups
            .Include(g => g.Faculty)
            .OrderBy(g => g.Name)
            .ToList();
        ViewBag.Statuses = Enum.GetValues<StudentStatus>();
    }
}
