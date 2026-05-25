using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    public new IActionResult StatusCode(int code)
    {
        ViewData["StatusCode"] = code;
        return View();
    }
}
