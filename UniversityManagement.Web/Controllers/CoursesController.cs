using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly CourseService _service;
    private readonly UniversityDbContext _context;

    public CoursesController(CourseService service, UniversityDbContext context)
    {
        _service = service;
        _context = context;
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Groups)
            .ThenInclude(g => g.Students)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(courses);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        PopulateCourseForm();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Course course, int[] groupIds)
    {
        ValidateCourse(course, groupIds);

        if (ModelState.IsValid)
        {
            course.Groups = await _context.Groups.Where(g => groupIds.Contains(g.Id)).ToListAsync();
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateCourseForm(groupIds);
        return View(course);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Groups)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
            return NotFound();
        PopulateCourseForm(course.Groups.Select(g => g.Id).ToArray());
        return View(course);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Course course, int[] groupIds)
    {
        if (id != course.Id)
            return NotFound();

        ValidateCourse(course, groupIds);

        if (ModelState.IsValid)
        {
            var existing = await _context.Courses
                .Include(c => c.Groups)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = course.Name;
            existing.Description = course.Description;
            existing.Credits = course.Credits;
            existing.TeacherId = course.TeacherId;
            existing.Groups.Clear();
            var selectedGroups = await _context.Groups.Where(g => groupIds.Contains(g.Id)).ToListAsync();
            foreach (var group in selectedGroups)
            {
                existing.Groups.Add(group);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateCourseForm(groupIds);
        return View(course);
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Details(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Groups)
            .ThenInclude(g => g.Students)
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
            return NotFound();
        return View(course);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _service.GetByIdAsync(id);
        if (course == null)
            return NotFound();
        return View(course);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteCourseAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private void PopulateCourseForm(int[]? selectedGroupIds = null)
    {
        ViewBag.Teachers = _context.Teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ToList();
        ViewBag.Groups = _context.Groups.Include(g => g.Faculty).OrderBy(g => g.Name).ToList();
        ViewBag.SelectedGroupIds = selectedGroupIds ?? Array.Empty<int>();
    }

    private void ValidateCourse(Course course, int[] groupIds)
    {
        if (course.TeacherId <= 0 || !_context.Teachers.Any(t => t.Id == course.TeacherId))
        {
            ModelState.AddModelError(nameof(Course.TeacherId), "Please select a valid teacher.");
        }

        if (groupIds.Length == 0)
        {
            ModelState.AddModelError("groupIds", "Select at least one group.");
        }
    }
}
