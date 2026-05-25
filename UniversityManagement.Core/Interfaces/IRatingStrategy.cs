using UniversityManagement.Core.Models;

namespace UniversityManagement.Core.Interfaces;

public interface IRatingStrategy
{
    double Calculate(Student student);
}