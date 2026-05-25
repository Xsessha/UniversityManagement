using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class ScheduleService
{
    private readonly List<Schedule> _schedule = new();

    public void Add(string dayOfWeek, string startTime, string endTime, int groupId, int lessonId)
    {
        _schedule.Add(new Schedule
        {
            Id = _schedule.Count + 1,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            GroupId = groupId,
            LessonId = lessonId
        });
    }

    public List<Schedule> GetAll() => _schedule;
}