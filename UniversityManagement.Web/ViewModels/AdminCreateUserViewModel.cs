using System.ComponentModel.DataAnnotations;
using UniversityManagement.Core.Enums;

namespace UniversityManagement.Web.ViewModels;

public class AdminCreateUserViewModel
{
    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Role")]
    public UserRole Role { get; set; } = UserRole.Student;

    [Display(Name = "Assigned group")]
    public int GroupId { get; set; }

    [Display(Name = "Department")]
    public string Department { get; set; } = string.Empty;
}
