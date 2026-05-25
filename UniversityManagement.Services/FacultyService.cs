using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class FacultyService
{
    private readonly List<Faculty> _faculties = new();

    public void Add(string name)
    {
        _faculties.Add(new Faculty
        {
            Id = _faculties.Count + 1,
            Name = name
        });
    }

    public List<Faculty> GetAll() => _faculties;
}