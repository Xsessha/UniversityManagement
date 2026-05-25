using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class ReportsController : Controller
{
    private readonly ReportService _service;

    public ReportsController(ReportService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult StudentReport(int? id)
    {
        // TODO: Retrieve student and generate report
        return View();
    }
}