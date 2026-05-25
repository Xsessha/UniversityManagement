using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class FacultiesController : Controller
{
    private readonly FacultyService _facultyService;

    public FacultiesController(FacultyService facultyService)
    {
        _facultyService = facultyService;
    }

    public async Task<IActionResult> Index()
    {
        var faculties = await _facultyService.GetAllAsync();

        return View(faculties);
    }

    public async Task<IActionResult> TreeView()
    {
        var tree = await _facultyService.GetFacultyTreeAsync();

        return View(tree);
    }
}