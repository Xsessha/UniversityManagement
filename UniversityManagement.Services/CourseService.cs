using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class CourseService
{
    private readonly IRepository<Course> _repository;

    public CourseService(IRepository<Course> repository)
    {
        _repository = repository;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddCourseAsync(Course course)
    {
        await _repository.AddAsync(course);
    }

    public async Task UpdateCourseAsync(Course course)
    {
        await _repository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(int id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}