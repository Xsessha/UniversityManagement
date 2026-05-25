using UniversityManagement.Core.Models;
using UniversityManagement.Data.Repositories;

namespace UniversityManagement.Services;

public class StudentService
{
    private readonly StudentRepository _repository;

    public StudentService(
        StudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(Student student)
    {
        await _repository.AddAsync(student);
    }

    public async Task UpdateAsync(Student student)
    {
        await _repository.UpdateAsync(student);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}