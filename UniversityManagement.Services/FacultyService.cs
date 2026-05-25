using UniversityManagement.Core.Models;
using UniversityManagement.Data.Repositories;

namespace UniversityManagement.Services;

public class FacultyService
{
    private readonly FacultyRepository _repository;

    public FacultyService(
        FacultyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Faculty>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<List<Faculty>> GetFacultyTreeAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Faculty?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(Faculty faculty)
    {
        await _repository.AddAsync(faculty);
    }

    public async Task UpdateAsync(Faculty faculty)
    {
        await _repository.UpdateAsync(faculty);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
