using UniversityManagement.Services;
using UniversityManagement.Data.Context;
using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace UniversityManagement.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<StudentService>();
        services.AddScoped<TeacherService>();
        services.AddScoped<CourseService>();
        services.AddScoped<RatingService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<ReportService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<FacultyService>();
        services.AddScoped<GroupService>();
        services.AddScoped<ScheduleService>();
        services.AddScoped<StatisticsService>();
        services.AddScoped<AuthenticationService>();
        services.AddScoped<AttendanceService>();

        // Repositories
        services.AddScoped<IRepository<Student>, GenericRepository<Student>>();
        services.AddScoped<IRepository<Teacher>, GenericRepository<Teacher>>();
        services.AddScoped<IRepository<Course>, GenericRepository<Course>>();
        services.AddScoped<IRepository<Faculty>, GenericRepository<Faculty>>();
        services.AddScoped<IRepository<Group>, GenericRepository<Group>>();
        services.AddScoped<IRepository<Lesson>, GenericRepository<Lesson>>();
        services.AddScoped<IRepository<Attendance>, GenericRepository<Attendance>>();
        services.AddScoped<IRepository<Grade>, GenericRepository<Grade>>();
        services.AddScoped<IRepository<Notification>, GenericRepository<Notification>>();
        services.AddScoped<IRepository<Report>, GenericRepository<Report>>();
        services.AddScoped<IRepository<Schedule>, GenericRepository<Schedule>>();

        return services;
    }
}

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly UniversityDbContext _context;

    public GenericRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllAsync() => await BuildQuery().ToListAsync();
    public async Task<T?> GetByIdAsync(int id)
    {
        var keyName = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name;

        if (string.IsNullOrWhiteSpace(keyName))
        {
            return await _context.Set<T>().FindAsync(id);
        }

        return await BuildQuery().FirstOrDefaultAsync(entity => EF.Property<int>(entity, keyName) == id);
    }
    public async Task AddAsync(T entity) 
    { 
        _context.Set<T>().Add(entity); 
        await _context.SaveChangesAsync(); 
    }
    public async Task UpdateAsync(T entity) 
    { 
        _context.Set<T>().Update(entity); 
        await _context.SaveChangesAsync(); 
    }
    public async Task DeleteAsync(int id) 
    { 
        var entity = await GetByIdAsync(id); 
        if (entity != null) 
        { 
            _context.Set<T>().Remove(entity); 
            await _context.SaveChangesAsync(); 
        } 
    }

    private IQueryable<T> BuildQuery()
    {
        var query = _context.Set<T>().AsQueryable();
        var entityType = _context.Model.FindEntityType(typeof(T));

        if (entityType == null)
        {
            return query;
        }

        foreach (var navigation in entityType.GetNavigations())
        {
            query = query.Include(navigation.Name);
        }

        return query.AsSplitQuery();
    }
}
