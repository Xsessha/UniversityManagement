using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class AttendanceController : Controller
{
    private readonly AttendanceService _service;

    public AttendanceController(AttendanceService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var attendance = await _service.GetAllAsync();
        return View(attendance);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Attendance attendance)
    {
        if (ModelState.IsValid)
        {
            await _service.MarkAttendanceAsync(attendance);
            return RedirectToAction(nameof(Index));
        }
        return View(attendance);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var attendance = await _service.GetByIdAsync(id);
        return attendance == null ? NotFound() : View(attendance);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Attendance attendance)
    {
        if (id != attendance.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _service.UpdateAttendanceAsync(attendance);
            return RedirectToAction(nameof(Index));
        }
        return View(attendance);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var attendance = await _service.GetByIdAsync(id);
        return attendance == null ? NotFound() : View(attendance);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteAttendanceAsync(id);
        return RedirectToAction(nameof(Index));
    }
}