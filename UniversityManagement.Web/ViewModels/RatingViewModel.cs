namespace UniversityManagement.Web.ViewModels;

public class RatingViewModel
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string GroupName { get; set; } = string.Empty;

    public double AverageGrade { get; set; }

    public double AttendancePercent { get; set; }

    public double FinalRating { get; set; }

    public string Status
    {
        get
        {
            if (FinalRating >= 90)
                return "Excellent";

            if (FinalRating >= 75)
                return "Good";

            return "Needs Improvement";
        }
    }
}