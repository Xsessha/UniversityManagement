using UniversityManagement.Data.Context;

namespace UniversityManagement.Services;

public class StatisticsService
{
    private readonly UniversityDbContext _context;

    public StatisticsService(
        UniversityDbContext context)
    {
        _context = context;
    }

    public double GetAverageStudentRating()
    {
        if (!_context.Students.Any())
            return 0;

        return _context.Students
            .Average(x => x.Rating);
    }

    public int GetTotalStudents()
    {
        return _context.Students.Count();
    }

    public int GetTotalTeachers()
    {
        return _context.Teachers.Count();
    }
}