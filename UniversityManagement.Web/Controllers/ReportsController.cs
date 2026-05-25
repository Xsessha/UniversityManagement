using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class ReportsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult StudentReport()
    {
        return View();
    }

    public IActionResult FacultyReport()
    {
        return View();
    }
}