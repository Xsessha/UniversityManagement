using Microsoft.EntityFrameworkCore;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Services;

public class DashboardService
{
    private readonly UniversityDbContext _context;

    public DashboardService(
        UniversityDbContext context)
    {
        _context = context;
    }

    public object GetStatistics()
    {
        return new
        {
            Students = _context.Students.Count(),
            Teachers = _context.Teachers.Count(),
            Courses = _context.Courses.Count(),
            Groups = _context.Groups.Count(),
            Faculties = _context.Faculties.Count()
        };
    }

    public async Task<object> GetDashboardDataAsync()
    {
        return new
        {
            Students = await _context.Students.CountAsync(),
            Teachers = await _context.Teachers.CountAsync(),
            Courses = await _context.Courses.CountAsync(),
            Groups = await _context.Groups.CountAsync(),
            Faculties = await _context.Faculties.CountAsync()
        };
    }
}
