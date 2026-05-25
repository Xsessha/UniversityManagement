using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class RatingService
{
    private IRatingStrategy _strategy = null!;

    public void SetStrategy(IRatingStrategy strategy)
    {
        _strategy = strategy;
    }

    public double Calculate(Student student)
    {
        return _strategy.Calculate(student);
    }
}