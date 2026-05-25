using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class RatingsController : Controller
{
    private readonly RatingService _service;

    public RatingsController(RatingService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View(new List<object>());
    }
}