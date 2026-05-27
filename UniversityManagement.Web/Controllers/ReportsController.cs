using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly ReportService _service;
    private readonly UniversityDbContext _context;

    public ReportsController(ReportService service, UniversityDbContext context)
    {
        _service = service;
        _context = context;
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Index()
    {
        var reports = await _context.Reports.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return View(reports);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> StudentReport()
    {
        var students = await _context.Students
            .Include(s => s.Group)
            .ThenInclude(g => g!.Faculty)
            .Include(s => s.Grades)
            .ThenInclude(g => g.Course)
            .OrderBy(s => s.Group!.Name)
            .ThenBy(s => s.LastName)
            .ToListAsync();
        return View(students);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> FacultyReport()
    {
        var faculties = await _context.Faculties
            .Include(f => f.Groups)
            .ThenInclude(g => g.Students)
            .Include(f => f.Groups)
            .ThenInclude(g => g.Courses)
            .OrderBy(f => f.Name)
            .ToListAsync();
        return View(faculties);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create(Report report)
    {
        if (ModelState.IsValid)
        {
            report.CreatedAt = DateTime.Now;
            report.GeneratedBy = User.Identity?.Name ?? "System";
            await _service.AddReportAsync(report);
            return RedirectToAction(nameof(Index));
        }
        return View(report);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Details(int id)
    {
        var report = await _service.GetByIdAsync(id);
        return report == null ? NotFound() : View(report);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var report = await _service.GetByIdAsync(id);
        return report == null ? NotFound() : View(report);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteReportAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
