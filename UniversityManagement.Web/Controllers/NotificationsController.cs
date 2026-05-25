using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly NotificationService _service;

    public NotificationsController(NotificationService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var notifications = await _service.GetAllAsync();
        return View(notifications);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Notification notification)
    {
        if (ModelState.IsValid)
        {
            await _service.SendNotificationAsync(notification);
            return RedirectToAction(nameof(Index));
        }
        return View(notification);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var notification = await _service.GetByIdAsync(id);
        return notification == null ? NotFound() : View(notification);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteNotificationAsync(id);
        return RedirectToAction(nameof(Index));
    }
}