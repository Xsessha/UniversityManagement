using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class GroupsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}