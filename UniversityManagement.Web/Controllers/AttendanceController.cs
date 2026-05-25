using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class AttendanceController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}