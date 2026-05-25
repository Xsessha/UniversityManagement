using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class ScheduleController : Controller
{
    private readonly ScheduleService _scheduleService;

    public ScheduleController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _scheduleService.GetAllAsync());
    }

    public IActionResult Calendar()
    {
        return View();
    }

    public IActionResult CreateLesson()
    {
        return View();
    }
}