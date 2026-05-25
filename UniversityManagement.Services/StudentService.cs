using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class StudentService
{
    private readonly List<Student> _students = new();

    public void AddStudent(string firstName, string lastName, string email)
    {
        _students.Add(new Student
        {
            Id = _students.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Rating = 0
        });
    }

    public List<Student> GetAll()
    {
        return _students;
    }

    public Student? GetById(int id)
    {
        return _students.FirstOrDefault(s => s.Id == id);
    }
}