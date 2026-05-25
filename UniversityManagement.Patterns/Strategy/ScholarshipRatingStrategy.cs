using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Strategy;

public class ScholarshipRatingStrategy : IRatingStrategy
{
    public double Calculate(Student student)
    {
        return student.Rating * 1.2;
    }
}