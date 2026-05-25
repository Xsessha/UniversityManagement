using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class StudentsController : Controller
{
    private readonly StudentService _service;

    public StudentsController(StudentService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View(_service.GetAll());
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Student student)
    {
        _service.AddStudent(student.FirstName, student.LastName, student.Email);
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        // TODO: Implement delete functionality in StudentService
        return RedirectToAction("Index");
    }
}