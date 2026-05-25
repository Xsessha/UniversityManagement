using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class ScheduleService
{
    private readonly IRepository<Schedule> _repository;

    public ScheduleService(IRepository<Schedule> repository)
    {
        _repository = repository;
    }

    public async Task<List<Schedule>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddScheduleAsync(Schedule schedule)
    {
        await _repository.AddAsync(schedule);
    }

    public async Task UpdateScheduleAsync(Schedule schedule)
    {
        await _repository.UpdateAsync(schedule);
    }

    public async Task DeleteScheduleAsync(int id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        if (schedule != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}