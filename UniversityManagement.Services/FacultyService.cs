using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class FacultyService
{
    private readonly IRepository<Faculty> _repository;

    public FacultyService(IRepository<Faculty> repository)
    {
        _repository = repository;
    }

    public async Task<List<Faculty>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Faculty?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddFacultyAsync(Faculty faculty)
    {
        await _repository.AddAsync(faculty);
    }

    public async Task UpdateFacultyAsync(Faculty faculty)
    {
        await _repository.UpdateAsync(faculty);
    }

    public async Task DeleteFacultyAsync(int id)
    {
        var faculty = await _repository.GetByIdAsync(id);
        if (faculty != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}