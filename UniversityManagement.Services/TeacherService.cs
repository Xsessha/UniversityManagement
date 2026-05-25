using UniversityManagement.Core.Models;
using UniversityManagement.Data.Repositories;

namespace UniversityManagement.Services;

public class TeacherService
{
    private readonly TeacherRepository _repository;

    public TeacherService(
        TeacherRepository repository)
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

    public async Task CreateAsync(Teacher teacher)
    {
        await _repository.AddAsync(teacher);
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        await _repository.UpdateAsync(teacher);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
