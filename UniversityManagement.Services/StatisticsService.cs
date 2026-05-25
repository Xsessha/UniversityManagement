using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class StatisticsService
{
    public double GetAverageRating(List<Student> students)
    {
        if (!students.Any()) return 0;
        return students.Average(s => s.Rating);
    }
}