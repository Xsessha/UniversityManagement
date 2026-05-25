namespace UniversityManagement.Web.ViewModels;

public class RatingViewModel
{
    public string StudentName { get; set; } = "";
    public string GroupName { get; set; } = "";

    public double AverageRating { get; set; }

    public string ScholarshipStatus { get; set; } = "";
}

public class RatingStatisticsViewModel
{
    public double AverageRating { get; set; }

    public double MaxRating { get; set; }

    public double MinRating { get; set; }

    public Dictionary<string, double> AverageByGroup { get; set; } = new();
}
