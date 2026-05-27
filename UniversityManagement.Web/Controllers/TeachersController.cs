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
public class TeachersController : Controller
{
    private readonly TeacherService _service;
    private readonly UniversityDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public TeachersController(TeacherService service, UniversityDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _service = service;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Index()
    {
        var teachers = await _context.Teachers
            .Include(t => t.Courses)
            .OrderBy(t => t.LastName)
            .ThenBy(t => t.FirstName)
            .ToListAsync();
        return View(teachers);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Teacher teacher, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(nameof(password), "Please enter a password for the new teacher.");
        }

        if (!ModelState.IsValid)
        {
            return View(teacher);
        }

        var existingUser = await _userManager.FindByEmailAsync(teacher.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(teacher.Email), "A user with this email already exists.");
            return View(teacher);
        }

        var identityUser = new ApplicationUser
        {
            UserName = teacher.Email,
            Email = teacher.Email,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Role = UserRole.Teacher
        };

        var createResult = await _userManager.CreateAsync(identityUser, password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(teacher);
        }

        var teacherRole = UserRole.Teacher.ToString();
        if (!await _roleManager.RoleExistsAsync(teacherRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(teacherRole));
        }

        await _userManager.AddToRoleAsync(identityUser, teacherRole);
        await _service.AddTeacherAsync(teacher);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var teacher = await _service.GetByIdAsync(id);
        if (teacher == null)
            return NotFound();
        return View(teacher);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Teacher teacher)
    {
        if (id != teacher.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            await _service.UpdateTeacherAsync(teacher);
            return RedirectToAction(nameof(Index));
        }
        return View(teacher);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Details(int id)
    {
        var teacher = await _context.Teachers
            .Include(t => t.Courses)
            .ThenInclude(c => c.Groups)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (teacher == null)
            return NotFound();

        var schedules = await _context.Schedules
            .Include(s => s.Group)
            .Include(s => s.Lesson)
            .ThenInclude(l => l!.Course)
            .Where(s => s.Lesson!.Course!.TeacherId == teacher.Id)
            .OrderBy(s => s.Lesson!.Date)
            .ToListAsync();

        ViewBag.TeacherSchedule = schedules;
        return View(teacher);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await _service.GetByIdAsync(id);
        if (teacher == null)
            return NotFound();
        return View(teacher);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteTeacherAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
