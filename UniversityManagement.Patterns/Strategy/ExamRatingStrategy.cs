using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Strategy;

public class ExamRatingStrategy : IRatingStrategy
{
    public double Calculate(Student student)
    {
        return student.Rating + 15;
    }
}