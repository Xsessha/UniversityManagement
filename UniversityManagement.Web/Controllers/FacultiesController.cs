using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class FacultiesController : Controller
{
    private readonly FacultyService _service;

    public FacultiesController(FacultyService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var faculties = await _service.GetAllAsync();
        return View(faculties);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Faculty faculty)
    {
        if (ModelState.IsValid)
        {
            await _service.AddFacultyAsync(faculty);
            return RedirectToAction(nameof(Index));
        }
        return View(faculty);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var faculty = await _service.GetByIdAsync(id);
        return faculty == null ? NotFound() : View(faculty);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Faculty faculty)
    {
        if (id != faculty.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _service.UpdateFacultyAsync(faculty);
            return RedirectToAction(nameof(Index));
        }
        return View(faculty);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var faculty = await _service.GetByIdAsync(id);
        return faculty == null ? NotFound() : View(faculty);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteFacultyAsync(id);
        return RedirectToAction(nameof(Index));
    }
}