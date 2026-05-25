using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly CourseService _service;

    public CoursesController(CourseService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _service.GetAllAsync();
        return View(courses);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Course course)
    {
        if (ModelState.IsValid)
        {
            await _service.AddCourseAsync(course);
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var course = await _service.GetByIdAsync(id);
        if (course == null)
            return NotFound();
        return View(course);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            await _service.UpdateCourseAsync(course);
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }

    public async Task<IActionResult> Details(int id)
    {
        var course = await _service.GetByIdAsync(id);
        if (course == null)
            return NotFound();
        return View(course);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var course = await _service.GetByIdAsync(id);
        if (course == null)
            return NotFound();
        return View(course);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteCourseAsync(id);
        return RedirectToAction(nameof(Index));
    }
}