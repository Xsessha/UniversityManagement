using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Services;

public class ScheduleService
{
    private readonly UniversityDbContext? _context;

    private readonly List<Schedule> _schedules = new();

    public ScheduleService()
    {
    }

    public ScheduleService(
        UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<List<Schedule>> GetAllAsync()
    {
        if (_context == null)
            return _schedules;

        return await _context.Schedules
            .Include(x => x.Course)
            .Include(x => x.Teacher)
            .Include(x => x.Group)
            .ToListAsync();
    }

    public List<Schedule> GetAll()
    {
        if (_context != null)
        {
            return _context.Schedules
                .Include(x => x.Course)
                .Include(x => x.Teacher)
                .Include(x => x.Group)
                .ToList();
        }

        return _schedules;
    }

    public void Add(Schedule schedule)
    {
        if (_context != null)
        {
            _context.Schedules.Add(schedule);
            _context.SaveChanges();
            return;
        }

        _schedules.Add(schedule);
    }
}
