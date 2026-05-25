namespace UniversityManagement.Core.DTO;

public class RatingDTO
{
    public string StudentName { get; set; } = string.Empty;

    public double AverageGrade { get; set; }

    public double AttendancePercent { get; set; }

    public double FinalRating { get; set; }
}