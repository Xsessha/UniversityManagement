using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewBag.Message = "University Dashboard";
        return View();
    }
}