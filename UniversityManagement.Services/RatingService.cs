using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Services;

public class RatingService
{
    private readonly UniversityDbContext? _context;

    public RatingService()
    {
    }

    public RatingService(
        UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetRatingsAsync()
    {
        if (_context == null)
            return new List<Student>();

        return await _context.Students
            .Include(x => x.Group)
            .OrderByDescending(x => x.Rating)
            .ToListAsync();
    }

    public async Task<List<Student>> GetTopStudentsAsync()
    {
        if (_context == null)
            return new List<Student>();

        return await _context.Students
            .Include(x => x.Group)
            .OrderByDescending(x => x.Rating)
            .Take(10)
            .ToListAsync();
    }

    public async Task<object> GetStatisticsAsync()
    {
        if (_context == null)
        {
            return new
            {
                StudentCount = 0,
                AverageRating = 0.0,
                TopRating = 0.0
            };
        }

        var students = await _context.Students
            .ToListAsync();

        return new
        {
            StudentCount = students.Count,
            AverageRating = students.Count == 0
                ? 0
                : students.Average(x => x.Rating),
            TopRating = students.Count == 0
                ? 0
                : students.Max(x => x.Rating)
        };
    }

    public double CalculateRating(Student student)
    {
        if (!student.Grades.Any())
            return 0;

        return student.Grades
            .Average(x => x.Value);
    }
}
