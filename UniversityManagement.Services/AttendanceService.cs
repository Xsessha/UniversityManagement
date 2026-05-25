using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class AttendanceService
{
    private readonly List<Attendance> _attendance = new();

    public void Mark(int studentId, AttendanceStatus status)
    {
        _attendance.Add(new Attendance
        {
            Id = _attendance.Count + 1,
            StudentId = studentId,
            Status = status
        });
    }

    public List<Attendance> GetAll() => _attendance;
}