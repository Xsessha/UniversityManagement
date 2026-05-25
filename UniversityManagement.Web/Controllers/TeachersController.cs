using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class TeachersController : Controller
{
    private readonly TeacherService _service;

    public TeachersController(TeacherService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var teachers = await _service.GetAllAsync();
        return View(teachers);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Teacher teacher)
    {
        if (ModelState.IsValid)
        {
            await _service.AddTeacherAsync(teacher);
            return RedirectToAction(nameof(Index));
        }
        return View(teacher);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var teacher = await _service.GetByIdAsync(id);
        if (teacher == null)
            return NotFound();
        return View(teacher);
    }

    [HttpPost]
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

    public async Task<IActionResult> Details(int id)
    {
        var teacher = await _service.GetByIdAsync(id);
        if (teacher == null)
            return NotFound();
        return View(teacher);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await _service.GetByIdAsync(id);
        if (teacher == null)
            return NotFound();
        return View(teacher);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteTeacherAsync(id);
        return RedirectToAction(nameof(Index));
    }
}