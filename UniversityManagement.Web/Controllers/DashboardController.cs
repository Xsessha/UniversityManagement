using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewBag.Name =
            User.Identity?.Name;

        ViewBag.Email =
            User.Claims.FirstOrDefault(x =>
                x.Type.Contains("email"))?.Value;

        return View();
    }
}