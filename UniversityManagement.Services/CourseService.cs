using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class CourseService
{
    private readonly List<Course> _courses = new();

    public void Add(string name, int credits, Teacher teacher)
    {
        _courses.Add(new Course
        {
            Id = _courses.Count + 1,
            Name = name,
            Credits = credits,
            Teacher = teacher
        });
    }

    public List<Course> GetAll() => _courses;
}