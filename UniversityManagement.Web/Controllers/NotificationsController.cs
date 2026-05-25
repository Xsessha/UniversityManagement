using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class NotificationsController : Controller
{
    private readonly NotificationService _service;

    public NotificationsController(NotificationService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View(new List<string>());
    }
}