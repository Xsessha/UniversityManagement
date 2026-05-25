using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class AuthenticationService
{
    private readonly List<ApplicationUser> _users = new();

    public void Register(string email, UserRole role)
    {
        _users.Add(new ApplicationUser
        {
            UserName = email,
            Email = email,
            Role = role
        });
    }

    public ApplicationUser? Login(string email)
    {
        return _users.FirstOrDefault(u => u.Email == email);
    }
}