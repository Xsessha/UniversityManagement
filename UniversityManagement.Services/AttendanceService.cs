using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class AttendanceService
{
    private readonly IRepository<Attendance> _repository;

    public AttendanceService(IRepository<Attendance> repository)
    {
        _repository = repository;
    }

    public async Task<List<Attendance>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Attendance?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task MarkAttendanceAsync(Attendance attendance)
    {
        await _repository.AddAsync(attendance);
    }

    public async Task UpdateAttendanceAsync(Attendance attendance)
    {
        await _repository.UpdateAsync(attendance);
    }

    public async Task DeleteAttendanceAsync(int id)
    {
        var attendance = await _repository.GetByIdAsync(id);
        if (attendance != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}