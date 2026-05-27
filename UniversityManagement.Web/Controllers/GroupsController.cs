using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class GroupsController : Controller
{
    private readonly GroupService _service;
    private readonly UniversityDbContext _context;

    public GroupsController(GroupService service, UniversityDbContext context)
    {
        _service = service;
        _context = context;
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Index()
    {
        var groups = await _context.Groups
            .Include(g => g.Faculty)
            .Include(g => g.Students)
            .Include(g => g.Courses)
            .OrderBy(g => g.Name)
            .ToListAsync();
        return View(groups);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        PopulateGroupForm();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Group group)
    {
        if (group.FacultyId <= 0 || !await _context.Faculties.AnyAsync(f => f.Id == group.FacultyId))
        {
            ModelState.AddModelError(nameof(Group.FacultyId), "Please select a valid faculty.");
        }

        if (ModelState.IsValid)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateGroupForm();
        return View(group);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        PopulateGroupForm();
        return group == null ? NotFound() : View(group);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Group group)
    {
        if (id != group.Id) return NotFound();
        if (group.FacultyId <= 0 || !await _context.Faculties.AnyAsync(f => f.Id == group.FacultyId))
        {
            ModelState.AddModelError(nameof(Group.FacultyId), "Please select a valid faculty.");
        }
        if (ModelState.IsValid)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateGroupForm();
        return View(group);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Details(int id)
    {
        var group = await _context.Groups
            .Include(g => g.Faculty)
            .Include(g => g.Students)
            .Include(g => g.Courses)
            .ThenInclude(c => c.Teacher)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null) return NotFound();

        var schedules = await _context.Schedules
            .Where(s => s.GroupId == id)
            .Include(s => s.Lesson)
            .ThenInclude(l => l!.Course)
            .ThenInclude(c => c!.Teacher)
            .OrderBy(s => s.Lesson!.Date)
            .ToListAsync();

        ViewBag.GroupSchedules = schedules;
        return View(group);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var group = await _service.GetByIdAsync(id);
        return group == null ? NotFound() : View(group);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteGroupAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private void PopulateGroupForm()
    {
        ViewBag.Faculties = _context.Faculties.OrderBy(f => f.Name).ToList();
    }
}
