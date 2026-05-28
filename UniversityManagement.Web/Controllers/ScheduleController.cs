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
public class ScheduleController : Controller
{
    private readonly ScheduleService _service;
    private readonly UniversityDbContext _context;

    public ScheduleController(ScheduleService service, UniversityDbContext context)
    {
        _service = service;
        _context = context;
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Index()
    {
        var schedules = await GetSchedulesForCurrentUser()
            .Where(s => s.Lesson != null)
            .OrderBy(s => s.Lesson!.Date)
            .ToListAsync();
        return View(schedules);
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Today()
    {
        var schedules = await GetSchedulesForCurrentUser()
            .Where(s => s.Lesson != null && s.Lesson.Date.Date == DateTime.Today)
            .OrderBy(s => s.Lesson!.Date)
            .ToListAsync();

        ViewData["Title"] = "Today\'s Schedule";
        return View("Calendar", schedules);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        PopulateScheduleForm();
        return View("CreateLesson");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult CreateLesson()
    {
        PopulateScheduleForm();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Schedule schedule)
    {
        return await SaveSchedule(schedule);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLesson(int groupId, int courseId, LessonType lessonType, DateTime date, string? topic)
    {
        if (groupId <= 0 || !await _context.Groups.AnyAsync(g => g.Id == groupId))
        {
            ModelState.AddModelError(nameof(groupId), "Please select a valid group.");
        }

        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        if (course == null)
        {
            ModelState.AddModelError(nameof(courseId), "Please select a valid course.");
        }

        if (date == default)
        {
            ModelState.AddModelError(nameof(date), "Please select a valid date and time.");
        }

        if (!ModelState.IsValid)
        {
            PopulateScheduleForm();
            return View(new Schedule { GroupId = groupId });
        }

        var lesson = new Lesson
        {
            CourseId = courseId,
            Type = lessonType,
            Date = date,
            Topic = string.IsNullOrWhiteSpace(topic) ? course!.Name : topic.Trim()
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        var schedule = new Schedule
        {
            GroupId = groupId,
            LessonId = lesson.Id,
            DayOfWeek = date.DayOfWeek.ToString(),
            StartTime = date.ToString("HH:mm"),
            EndTime = date.AddMinutes(90).ToString("HH:mm")
        };

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Lesson)
            .FirstOrDefaultAsync(s => s.Id == id);
        PopulateScheduleForm();
        return schedule == null ? NotFound() : View("CreateLesson", schedule);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Schedule schedule)
    {
        if (id != schedule.Id) return NotFound();
        ValidateSchedule(schedule);
        if (ModelState.IsValid)
        {
            var lesson = await _context.Lessons.FindAsync(schedule.LessonId);
            if (lesson != null)
            {
                schedule.DayOfWeek = lesson.Date.DayOfWeek.ToString();
                schedule.StartTime = lesson.Date.ToString("HH:mm");
                schedule.EndTime = lesson.Date.AddMinutes(90).ToString("HH:mm");
            }

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateScheduleForm();
        return View("CreateLesson", schedule);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var schedule = await _service.GetByIdAsync(id);
        return schedule == null ? NotFound() : View(schedule);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteScheduleAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Calendar()
    {
        var schedules = await _context.Schedules
            .Include(s => s.Group)
            .Include(s => s.Lesson)
            .ThenInclude(l => l!.Course)
            .ThenInclude(c => c!.Teacher)
            .OrderBy(s => s.Lesson!.Date)
            .ToListAsync();
        return View(schedules);
    }

    private async Task<IActionResult> SaveSchedule(Schedule schedule)
    {
        ValidateSchedule(schedule);
        if (ModelState.IsValid)
        {
            var lesson = await _context.Lessons.FindAsync(schedule.LessonId);
            if (lesson != null)
            {
                schedule.DayOfWeek = lesson.Date.DayOfWeek.ToString();
                schedule.StartTime = lesson.Date.ToString("HH:mm");
                schedule.EndTime = lesson.Date.AddMinutes(90).ToString("HH:mm");
            }

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateScheduleForm();
        return View("CreateLesson", schedule);
    }

    private void ValidateSchedule(Schedule schedule)
    {
        if (schedule.GroupId <= 0 || !_context.Groups.Any(g => g.Id == schedule.GroupId))
        {
            ModelState.AddModelError(nameof(Schedule.GroupId), "Please select a valid group.");
        }

        if (schedule.LessonId <= 0 || !_context.Lessons.Any(l => l.Id == schedule.LessonId))
        {
            ModelState.AddModelError(nameof(Schedule.LessonId), "Please select a valid lesson.");
        }
    }

    private IQueryable<Schedule> GetSchedulesForCurrentUser()
    {
        var query = _context.Schedules
            .Include(s => s.Group)
            .ThenInclude(g => g!.Faculty)
            .Include(s => s.Lesson)
            .ThenInclude(l => l!.Course)
            .ThenInclude(c => c!.Teacher)
            .AsQueryable();

        var currentEmail = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(currentEmail))
        {
            return query.Where(s => false);
        }

        var normalized = currentEmail.ToLowerInvariant();

        if (User.IsInRole("Student"))
        {
            var student = _context.Students.AsEnumerable().FirstOrDefault(s => string.Equals(s.Email, currentEmail, StringComparison.OrdinalIgnoreCase));
            if (student == null || student.GroupId == 0)
                return query.Where(s => false);
            return query.Where(s => s.GroupId == student.GroupId);
        }

        if (User.IsInRole("Teacher"))
        {
            var teacher = _context.Teachers.AsEnumerable().FirstOrDefault(t => string.Equals(t.Email, currentEmail, StringComparison.OrdinalIgnoreCase));
            if (teacher == null)
                return query.Where(s => false);
            return query.Where(s => s.Lesson != null && s.Lesson.Course != null && s.Lesson.Course.TeacherId == teacher.Id);
        }

        return query;
    }

    private void PopulateScheduleForm()
    {
        ViewBag.Groups = _context.Groups
            .Include(g => g.Faculty)
            .OrderBy(g => g.Name)
            .ToList();
        ViewBag.Lessons = _context.Lessons
            .Include(l => l.Course)
            .ThenInclude(c => c!.Teacher)
            .OrderBy(l => l.Date)
            .ToList();
        ViewBag.Courses = _context.Courses
            .Include(c => c.Teacher)
            .OrderBy(c => c.Name)
            .ToList();
        ViewBag.LessonTypes = Enum.GetValues<LessonType>();
    }
}
