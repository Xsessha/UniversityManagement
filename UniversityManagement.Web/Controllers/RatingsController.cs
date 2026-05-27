using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;
using UniversityManagement.Web.ViewModels;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class RatingsController : Controller
{
    private readonly StudentService _studentService;

    public RatingsController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Index()
    {
        var students = await _studentService.GetAllAsync();
        var ratings = students
            .OrderByDescending(s => s.Rating)
            .Select(s => new RatingViewModel
            {
                StudentName = $"{s.FirstName} {s.LastName}",
                GroupName = s.Group?.Name ?? "No group",
                AverageRating = s.Rating,
                ScholarshipStatus = s.Rating >= 90 ? "Eligible" : "Not eligible"
            })
            .ToList();

        return View(ratings);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Statistics()
    {
        var students = await _studentService.GetAllAsync();
        var model = new RatingStatisticsViewModel
        {
            AverageRating = students.Any() ? students.Average(s => s.Rating) : 0,
            MaxRating = students.Any() ? students.Max(s => s.Rating) : 0,
            MinRating = students.Any() ? students.Min(s => s.Rating) : 0,
            AverageByGroup = students
                .GroupBy(s => s.Group?.Name ?? "No group")
                .ToDictionary(g => g.Key, g => g.Average(s => s.Rating))
        };

        return View(model);
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> TopStudents()
    {
        var students = await _studentService.GetAllAsync();
        var topStudents = students
            .OrderByDescending(s => s.Rating)
            .Take(10)
            .ToList();

        return View(topStudents);
    }
}
