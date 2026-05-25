using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class TeachersController : Controller
{
    private readonly TeacherService _service;

    public TeachersController(TeacherService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View(_service.GetAll());
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Teacher teacher)
    {
        _service.Add(teacher.FirstName, teacher.LastName, teacher.Email, teacher.Department);
        return RedirectToAction("Index");
    }
}