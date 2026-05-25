using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class ScheduleController : Controller
{
    private readonly ScheduleService _service;

    public ScheduleController(ScheduleService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var schedules = await _service.GetAllAsync();
        return View(schedules);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Schedule schedule)
    {
        if (ModelState.IsValid)
        {
            await _service.AddScheduleAsync(schedule);
            return RedirectToAction(nameof(Index));
        }
        return View(schedule);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var schedule = await _service.GetByIdAsync(id);
        return schedule == null ? NotFound() : View(schedule);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Schedule schedule)
    {
        if (id != schedule.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _service.UpdateScheduleAsync(schedule);
            return RedirectToAction(nameof(Index));
        }
        return View(schedule);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var schedule = await _service.GetByIdAsync(id);
        return schedule == null ? NotFound() : View(schedule);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteScheduleAsync(id);
        return RedirectToAction(nameof(Index));
    }
}