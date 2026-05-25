namespace UniversityManagement.Patterns.Builder;

public class ExcelReportBuilder : ReportBuilder
{
    public override void BuildTitle()
    {
        report.Title = "Excel Report";
    }

    public override void BuildContent()
    {
        report.Content = "Excel content";
    }
}