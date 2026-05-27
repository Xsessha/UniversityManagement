using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly NotificationService _service;
    private readonly UniversityDbContext _context;

    public NotificationsController(NotificationService service, UniversityDbContext context)
    {
        _service = service;
        _context = context;
    }

    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> Index()
    {
        var notifications = await _context.Notifications
            .Include(n => n.Student)
            .ThenInclude(s => s!.Group)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return View(notifications);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create(Notification notification)
    {
        if (ModelState.IsValid)
        {
            await _service.SendNotificationAsync(notification);
            return RedirectToAction(nameof(Index));
        }
        return View(notification);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Delete(int id)
    {
        var notification = await _service.GetByIdAsync(id);
        return notification == null ? NotFound() : View(notification);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteNotificationAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
