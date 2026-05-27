using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Data.Context;
using UniversityManagement.Web.ViewModels;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class SearchController : Controller
{
    private readonly UniversityDbContext _context;

    public SearchController(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = (q ?? string.Empty).Trim();
        var model = new SearchViewModel { Query = query };

        if (string.IsNullOrWhiteSpace(query))
        {
            return View(model);
        }

        var pattern = $"%{query}%";

        model.Results.AddRange(await _context.Students
            .Include(s => s.Group)
            .Where(s => EF.Functions.Like(s.FirstName, pattern) || EF.Functions.Like(s.LastName, pattern) || EF.Functions.Like(s.Email, pattern))
            .Select(s => new SearchResultViewModel
            {
                Type = "Student",
                Title = s.FirstName + " " + s.LastName,
                Subtitle = (s.Group != null ? s.Group.Name : "No group") + " - " + s.Email,
                Url = Url.Action("Details", "Students", new { id = s.Id }) ?? "#"
            })
            .Take(10)
            .ToListAsync());

        model.Results.AddRange(await _context.Teachers
            .Where(t => EF.Functions.Like(t.FirstName, pattern) || EF.Functions.Like(t.LastName, pattern) || EF.Functions.Like(t.Department, pattern) || EF.Functions.Like(t.Email, pattern))
            .Select(t => new SearchResultViewModel
            {
                Type = "Teacher",
                Title = t.FirstName + " " + t.LastName,
                Subtitle = t.Department + " - " + t.Email,
                Url = Url.Action("Details", "Teachers", new { id = t.Id }) ?? "#"
            })
            .Take(10)
            .ToListAsync());

        model.Results.AddRange(await _context.Courses
            .Include(c => c.Teacher)
            .Where(c => EF.Functions.Like(c.Name, pattern) || EF.Functions.Like(c.Description, pattern))
            .Select(c => new SearchResultViewModel
            {
                Type = "Course",
                Title = c.Name,
                Subtitle = c.Teacher != null ? c.Teacher.FirstName + " " + c.Teacher.LastName : "No teacher",
                Url = Url.Action("Details", "Courses", new { id = c.Id }) ?? "#"
            })
            .Take(10)
            .ToListAsync());

        model.Results.AddRange(await _context.Groups
            .Include(g => g.Faculty)
            .Where(g => EF.Functions.Like(g.Name, pattern) || (g.Faculty != null && EF.Functions.Like(g.Faculty.Name, pattern)))
            .Select(g => new SearchResultViewModel
            {
                Type = "Group",
                Title = g.Name,
                Subtitle = g.Faculty != null ? g.Faculty.Name : "No faculty",
                Url = Url.Action("Details", "Groups", new { id = g.Id }) ?? "#"
            })
            .Take(10)
            .ToListAsync());

        model.Results.AddRange(await _context.Reports
            .Where(r => EF.Functions.Like(r.Title, pattern) || EF.Functions.Like(r.Description, pattern) || EF.Functions.Like(r.GeneratedBy, pattern))
            .Select(r => new SearchResultViewModel
            {
                Type = "Report",
                Title = r.Title,
                Subtitle = r.GeneratedBy + " - " + r.CreatedAt.ToShortDateString(),
                Url = Url.Action("Details", "Reports", new { id = r.Id }) ?? "#"
            })
            .Take(10)
            .ToListAsync());

        return View(model);
    }
}
