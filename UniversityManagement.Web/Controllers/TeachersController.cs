using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class TeachersController : Controller
{
    private readonly TeacherService _teacherService;

    public TeachersController(TeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _teacherService.GetAllAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var teacher = await _teacherService.GetByIdAsync(id);

        if (teacher == null)
            return NotFound();

        return View(teacher);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Teacher teacher)
    {
        if (!ModelState.IsValid)
            return View(teacher);

        await _teacherService.CreateAsync(teacher);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _teacherService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}