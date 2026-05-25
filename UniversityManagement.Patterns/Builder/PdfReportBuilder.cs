namespace UniversityManagement.Patterns.Builder;

public class PdfReportBuilder : ReportBuilder
{
    public override void BuildTitle()
    {
        report.Title = "PDF Report";
    }

    public override void BuildContent()
    {
        report.Content = "PDF content";
    }
}