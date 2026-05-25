using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class RatingsController : Controller
{
    private readonly RatingService _ratingService;

    public RatingsController(RatingService ratingService)
    {
        _ratingService = ratingService;
    }

    public async Task<IActionResult> Index()
    {
        var ratings = await _ratingService.GetRatingsAsync();

        return View(ratings);
    }

    public async Task<IActionResult> Statistics()
    {
        var statistics = await _ratingService.GetStatisticsAsync();

        return View(statistics);
    }

    public async Task<IActionResult> TopStudents()
    {
        var students = await _ratingService.GetTopStudentsAsync();

        return View(students);
    }
}