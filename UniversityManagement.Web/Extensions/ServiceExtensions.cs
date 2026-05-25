using UniversityManagement.Services;

namespace UniversityManagement.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddSingleton<StudentService>();
        services.AddSingleton<TeacherService>();
        services.AddSingleton<CourseService>();
        services.AddSingleton<RatingService>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<ReportService>();

        return services;
    }
}