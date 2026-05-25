using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class ScheduleController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}