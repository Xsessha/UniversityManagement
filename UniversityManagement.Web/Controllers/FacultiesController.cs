using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class FacultiesController : Controller
{
    private readonly FacultyService _service;
    private readonly UniversityDbContext _context;

    public FacultiesController(FacultyService service, UniversityDbContext context)
    {
        _service = service;
        _context = context;
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Index()
    {
        var faculties = await _context.Faculties
            .Include(f => f.Groups)
            .ThenInclude(g => g.Students)
            .OrderBy(f => f.Name)
            .ToListAsync();
        return View(faculties);
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Details(int id)
    {
        var faculty = await _context.Faculties
            .Include(f => f.Groups)
            .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(f => f.Id == id);
        return faculty == null ? NotFound() : View(faculty);
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> TreeView(int id)
    {
        var faculty = await _context.Faculties
            .Include(f => f.Groups)
            .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(f => f.Id == id);
        return faculty == null ? NotFound() : View(faculty);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Faculty faculty)
    {
        if (ModelState.IsValid)
        {
            await _service.AddFacultyAsync(faculty);
            return RedirectToAction(nameof(Index));
        }
        return View(faculty);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var faculty = await _service.GetByIdAsync(id);
        return faculty == null ? NotFound() : View(faculty);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var faculty = await _service.GetByIdAsync(id);
        return faculty == null ? NotFound() : View(faculty);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteFacultyAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
