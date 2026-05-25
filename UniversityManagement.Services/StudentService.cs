using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class StudentService
{
    private readonly IRepository<Student> _repository;

    public StudentService(IRepository<Student> repository)
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

    public async Task AddStudentAsync(Student student)
    {
        await _repository.AddAsync(student);
    }

    public async Task UpdateStudentAsync(Student student)
    {
        await _repository.UpdateAsync(student);
    }

    public async Task DeleteStudentAsync(int id)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}