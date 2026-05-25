using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class AttendanceController : Controller
{
    private readonly AttendanceService _attendanceService;

    public AttendanceController(AttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _attendanceService.GetAllAsync());
    }

    public IActionResult MarkAttendance()
    {
        return View();
    }
}