using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
