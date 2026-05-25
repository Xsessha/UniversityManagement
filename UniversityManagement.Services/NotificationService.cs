using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class NotificationService
{
    private readonly IRepository<Notification> _repository;

    public NotificationService(IRepository<Notification> repository)
    {
        _repository = repository;
    }

    public async Task<List<Notification>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task SendNotificationAsync(Notification notification)
    {
        await _repository.AddAsync(notification);
    }

    public async Task DeleteNotificationAsync(int id)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}