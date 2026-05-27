using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        ViewBag.Roles = Enum.GetValues<UserRole>();
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, string firstName, string lastName, string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<UserRole>(role, out var parsedRole))
        {
            ModelState.AddModelError(nameof(role), "Select a valid role.");
            ViewBag.Roles = Enum.GetValues<UserRole>();
            return View(user);
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Role = parsedRole;

        var existingRoles = await _userManager.GetRolesAsync(user);
        if (existingRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, existingRoles);
        }

        await _userManager.AddToRoleAsync(user, parsedRole.ToString());
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }
}
