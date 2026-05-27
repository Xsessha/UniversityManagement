using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class GradesController : Controller
{
    private readonly UniversityDbContext _context;

    public GradesController(UniversityDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Index()
    {
        var query = _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Course)
            .ThenInclude(c => c!.Teacher)
            .AsQueryable();

        var currentEmail = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(currentEmail))
        {
            if (User.IsInRole("Student"))
            {
                query = query.Where(g => g.Student!.Email.Equals(currentEmail, StringComparison.OrdinalIgnoreCase));
            }
            else if (User.IsInRole("Teacher"))
            {
                query = query.Where(g => g.Course!.Teacher!.Email.Equals(currentEmail, StringComparison.OrdinalIgnoreCase));
            }
        }

        var grades = await query.OrderByDescending(g => g.Date).ToListAsync();
        return View(grades);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public IActionResult Create()
    {
        PopulateGradeForm();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create(int studentId, int lessonId, int value, string? comment)
    {
        if (studentId <= 0 || !await _context.Students.AnyAsync(s => s.Id == studentId))
        {
            ModelState.AddModelError(nameof(studentId), "Please select a valid student.");
        }

        var lesson = await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == lessonId);
        if (lesson == null)
        {
            ModelState.AddModelError(nameof(lessonId), "Please select a valid lesson.");
        }

        if (value < 0 || value > 100)
        {
            ModelState.AddModelError(nameof(value), "Grade must be between 0 and 100.");
        }

        if (!ModelState.IsValid)
        {
            PopulateGradeForm();
            return View();
        }

        var grade = new Grade
        {
            StudentId = studentId,
            CourseId = lesson!.CourseId,
            Value = value,
            Comment = comment?.Trim() ?? string.Empty,
            Date = lesson.Date
        };

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private void PopulateGradeForm()
    {
        ViewBag.Students = _context.Students
            .Include(s => s.Group)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToList();

        var lessonsQuery = _context.Lessons
            .Include(l => l.Course)
            .ThenInclude(c => c!.Teacher)
            .OrderBy(l => l.Date)
            .AsQueryable();

        var currentEmail = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(currentEmail) && User.IsInRole("Teacher"))
        {
            lessonsQuery = lessonsQuery.Where(l => l.Course!.Teacher!.Email.Equals(currentEmail, StringComparison.OrdinalIgnoreCase));
        }

        ViewBag.Lessons = lessonsQuery.ToList();
    }
}
