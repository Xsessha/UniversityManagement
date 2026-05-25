using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Data.Repositories;

public class CourseRepository : GenericRepository<Course>
{
    public CourseRepository(UniversityDbContext context) : base(context) { }

    public async Task<List<Course>> GetCoursesWithTeacherAsync()
    {
        return await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Groups)
            .ToListAsync();
    }
}