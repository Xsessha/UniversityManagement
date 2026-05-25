using Microsoft.AspNetCore.Identity;
using UniversityManagement.Core.Enums;

namespace UniversityManagement.Core.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}