using UniversityManagement.Core.Models;
using UniversityManagement.Data.Repositories;

namespace UniversityManagement.Services;

public class CourseService
{
    private readonly CourseRepository _repository;

    public CourseService(
        CourseRepository repository)
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

    public async Task CreateAsync(Course course)
    {
        await _repository.AddAsync(course);
    }

    public async Task UpdateAsync(Course course)
    {
        await _repository.UpdateAsync(course);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
