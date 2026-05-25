using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Services;

namespace UniversityManagement.Web.Controllers;

public class GroupsController : Controller
{
    private readonly GroupService _groupService;

    public GroupsController(GroupService groupService)
    {
        _groupService = groupService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _groupService.GetAllAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var group = await _groupService.GetByIdAsync(id);

        if (group == null)
            return NotFound();

        return View(group);
    }
}