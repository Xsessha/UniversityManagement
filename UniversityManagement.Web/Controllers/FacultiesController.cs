using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class FacultiesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}