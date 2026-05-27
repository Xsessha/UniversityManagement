using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var email = User?.Identity?.Name;
        var data = await _dashboardService.GetDashboardDataAsync(email);
        return View(data);
    }
}