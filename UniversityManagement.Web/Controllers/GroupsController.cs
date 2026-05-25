using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

[Authorize]
public class GroupsController : Controller
{
    private readonly GroupService _service;

    public GroupsController(GroupService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var groups = await _service.GetAllAsync();
        return View(groups);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Group group)
    {
        if (ModelState.IsValid)
        {
            await _service.AddGroupAsync(group);
            return RedirectToAction(nameof(Index));
        }
        return View(group);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var group = await _service.GetByIdAsync(id);
        return group == null ? NotFound() : View(group);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Group group)
    {
        if (id != group.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _service.UpdateGroupAsync(group);
            return RedirectToAction(nameof(Index));
        }
        return View(group);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var group = await _service.GetByIdAsync(id);
        return group == null ? NotFound() : View(group);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteGroupAsync(id);
        return RedirectToAction(nameof(Index));
    }
}