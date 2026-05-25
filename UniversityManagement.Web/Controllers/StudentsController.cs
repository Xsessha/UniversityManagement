using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class StudentsController : Controller
{
    private readonly StudentService _service;

    public StudentsController(StudentService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var students = await _service.GetAllAsync();
        return View(students);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Student student)
    {
        if (ModelState.IsValid)
        {
            await _service.AddStudentAsync(student);
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var student = await _service.GetByIdAsync(id);
        if (student == null)
            return NotFound();
        return View(student);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            await _service.UpdateStudentAsync(student);
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    public async Task<IActionResult> Details(int id)
    {
        var student = await _service.GetByIdAsync(id);
        if (student == null)
            return NotFound();
        return View(student);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var student = await _service.GetByIdAsync(id);
        if (student == null)
            return NotFound();
        return View(student);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteStudentAsync(id);
        return RedirectToAction(nameof(Index));
    }
}