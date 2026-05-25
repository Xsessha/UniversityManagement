using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class TeacherService
{
    private readonly List<Teacher> _teachers = new();

    public void Add(string firstName, string lastName, string email, string department)
    {
        _teachers.Add(new Teacher
        {
            Id = _teachers.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Department = department
        });
    }

    public List<Teacher> GetAll() => _teachers;
}