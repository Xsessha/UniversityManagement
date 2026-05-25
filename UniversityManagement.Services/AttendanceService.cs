using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Services;

public class AttendanceService
{
    private readonly UniversityDbContext? _context;

    public AttendanceService()
    {
    }

    public AttendanceService(
        UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<List<Attendance>> GetAllAsync()
    {
        if (_context == null)
            return new List<Attendance>();

        return await _context.Attendances
            .Include(x => x.Student)
            .ToListAsync();
    }

    public double CalculateAttendancePercent(
        Student student)
    {
        if (!student.Attendances.Any())
            return 0;

        var presentCount =
            student.Attendances.Count(x =>
                x.Status == AttendanceStatus.Present);

        return (double)presentCount /
               student.Attendances.Count * 100;
    }
}
