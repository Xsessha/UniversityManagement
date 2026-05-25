using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly ReportService _service;

    public ReportsController(ReportService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var reports = await _service.GetAllAsync();
        return View(reports);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Report report)
    {
        if (ModelState.IsValid)
        {
            await _service.AddReportAsync(report);
            return RedirectToAction(nameof(Index));
        }
        return View(report);
    }

    public async Task<IActionResult> Details(int id)
    {
        var report = await _service.GetByIdAsync(id);
        return report == null ? NotFound() : View(report);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var report = await _service.GetByIdAsync(id);
        return report == null ? NotFound() : View(report);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteReportAsync(id);
        return RedirectToAction(nameof(Index));
    }
}