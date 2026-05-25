using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Strategy;

public class AttendanceRatingStrategy : IRatingStrategy
{
    public double Calculate(Student student)
    {
        return student.Rating * 0.9;
    }
}