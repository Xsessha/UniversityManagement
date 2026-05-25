using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Web.ViewModels;

public class StudentViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Range(0, 100)]
    public double Rating { get; set; }

    public int GroupId { get; set; }

    public string? GroupName { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}