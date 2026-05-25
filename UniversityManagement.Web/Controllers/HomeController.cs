using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}