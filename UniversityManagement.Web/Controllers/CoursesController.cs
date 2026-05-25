using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly CourseService _courseService;

    public CoursesController(CourseService courseService)
    {
        _courseService = courseService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _courseService.GetAllAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseService.GetByIdAsync(id);

        if (course == null)
            return NotFound();

        return View(course);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid)
            return View(course);

        await _courseService.CreateAsync(course);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _courseService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}