using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class CoursesController : Controller
{
    private readonly CourseService _service;

    public CoursesController(CourseService service)
    {
        _service = service;
    }

    public IActionResult Index() => View(_service.GetAll());

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Course course)
    {
        _service.Add(course.Name, course.Credits, course.Teacher!);
        return RedirectToAction("Index");
    }
}