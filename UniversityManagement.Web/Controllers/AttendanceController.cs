using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class AttendanceController : Controller
{
    private readonly AttendanceService _service;
    private readonly UniversityDbContext _context;

    public AttendanceController(AttendanceService service, UniversityDbContext context)
    {
        _service = service;
        _context = context;
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Index()
    {
        var query = _context.Attendances
            .Include(a => a.Student)
            .ThenInclude(s => s!.Group)
            .Include(a => a.Lesson)
            .ThenInclude(l => l!.Course)
            .ThenInclude(c => c!.Teacher)
            .AsQueryable();

        var currentEmail = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(currentEmail))
        {
            var normalized = currentEmail.ToLowerInvariant();

            if (User.IsInRole("Student"))
            {
                var student = _context.Students.AsEnumerable().FirstOrDefault(s => string.Equals(s.Email, currentEmail, StringComparison.OrdinalIgnoreCase));
                if (student == null)
                    query = query.Where(a => false);
                else
                    query = query.Where(a => a.StudentId == student.Id);
            }
            else if (User.IsInRole("Teacher"))
            {
                var teacher = _context.Teachers.AsEnumerable().FirstOrDefault(t => string.Equals(t.Email, currentEmail, StringComparison.OrdinalIgnoreCase));
                if (teacher == null)
                    query = query.Where(a => false);
                else
                    query = query.Where(a => a.Lesson != null && a.Lesson.Course != null && a.Lesson.Course.TeacherId == teacher.Id);
            }
        }

        var attendance = await query.OrderByDescending(a => a.Date).ToListAsync();
        return View(attendance);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public IActionResult Create()
    {
        PopulateAttendanceForm();
        return View("MarkAttendance");
    }

    [Authorize(Roles = "Admin,Teacher")]
    public IActionResult MarkAttendance()
    {
        PopulateAttendanceForm();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create(Attendance attendance)
    {
        return await SaveAttendance(attendance);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> MarkAttendance(Attendance attendance)
    {
        return await SaveAttendance(attendance);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Edit(int id)
    {
        var attendance = await _service.GetByIdAsync(id);
        if (attendance == null)
        {
            return NotFound();
        }

        PopulateAttendanceForm();
        return View("MarkAttendance", attendance);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Edit(int id, Attendance attendance)
    {
        if (id != attendance.Id) return NotFound();
        ValidateAttendance(attendance);
        if (ModelState.IsValid)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateAttendanceForm();
        return View("MarkAttendance", attendance);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Delete(int id)
    {
        var attendance = await _service.GetByIdAsync(id);
        return attendance == null ? NotFound() : View(attendance);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteAttendanceAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> SaveAttendance(Attendance attendance)
    {
        ValidateAttendance(attendance);
        if (ModelState.IsValid)
        {
            if (attendance.Date == default)
            {
                var lesson = await _context.Lessons.FindAsync(attendance.LessonId);
                attendance.Date = lesson?.Date.Date ?? DateTime.Today;
            }

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateAttendanceForm();
        return View("MarkAttendance", attendance);
    }

    private void ValidateAttendance(Attendance attendance)
    {
        if (attendance.StudentId <= 0 || !_context.Students.Any(s => s.Id == attendance.StudentId))
        {
            ModelState.AddModelError(nameof(Attendance.StudentId), "Please select a valid student.");
        }

        if (attendance.LessonId <= 0 || !_context.Lessons.Any(l => l.Id == attendance.LessonId))
        {
            ModelState.AddModelError(nameof(Attendance.LessonId), "Please select a valid lesson.");
        }
    }

    private void PopulateAttendanceForm()
    {
        ViewBag.Students = _context.Students
            .Include(s => s.Group)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToList();

        var lessonsQuery = _context.Lessons
            .Include(l => l.Course)
            .ThenInclude(c => c!.Teacher)
            .OrderByDescending(l => l.Date)
            .AsQueryable();

        var currentEmail = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(currentEmail) && User.IsInRole("Teacher"))
        {
            var normalized = currentEmail.ToLowerInvariant();
            lessonsQuery = lessonsQuery.Where(l => l.Course!.Teacher!.Email.ToLower() == normalized);
        }

        ViewBag.Lessons = lessonsQuery.ToList();
        ViewBag.Statuses = Enum.GetValues<AttendanceStatus>();
    }
}
