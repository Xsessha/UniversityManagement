using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Builder;

public abstract class ReportBuilder
{
    protected Report report = new();

    public abstract void BuildTitle();
    public abstract void BuildContent();

    public Report GetReport() => report;
}