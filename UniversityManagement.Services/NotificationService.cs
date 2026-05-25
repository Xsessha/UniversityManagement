using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Services;

public class NotificationService
{
    private readonly UniversityDbContext? _context;

    private readonly List<Notification> _notifications
        = new();

    public NotificationService()
    {
    }

    public NotificationService(
        UniversityDbContext context)
    {
        _context = context;
    }

    public void Send(
        string title,
        string message)
    {
        var notification = new Notification
        {
            Title = title,
            Message = message,
            CreatedAt = DateTime.Now
        };

        if (_context != null)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return;
        }

        _notifications.Add(notification);
    }

    public async Task<List<Notification>> GetAllAsync()
    {
        if (_context == null)
            return _notifications;

        return await _context.Notifications
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public List<Notification> GetAll()
    {
        if (_context != null)
        {
            return _context.Notifications
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        return _notifications;
    }
}
