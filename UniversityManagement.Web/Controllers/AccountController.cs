using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Web.Controllers;

public class AccountController : Controller
{
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        if (email == "admin@uni.com")
            return RedirectToAction("Index", "Dashboard");

        ViewBag.Error = "Invalid credentials";
        return View();
    }

    public IActionResult Register() => View();

    public IActionResult Logout()
    {
        return RedirectToAction("Login");
    }
}