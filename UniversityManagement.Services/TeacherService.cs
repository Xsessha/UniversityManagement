using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class TeacherService
{
    private readonly IRepository<Teacher> _repository;

    public TeacherService(IRepository<Teacher> repository)
    {
        _repository = repository;
    }

    public async Task<List<Teacher>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Teacher?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddTeacherAsync(Teacher teacher)
    {
        await _repository.AddAsync(teacher);
    }

    public async Task UpdateTeacherAsync(Teacher teacher)
    {
        await _repository.UpdateAsync(teacher);
    }

    public async Task DeleteTeacherAsync(int id)
    {
        var teacher = await _repository.GetByIdAsync(id);
        if (teacher != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}