namespace UniversityManagement.Core.DTO;

public class StudentDTO
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public double Rating { get; set; }
}