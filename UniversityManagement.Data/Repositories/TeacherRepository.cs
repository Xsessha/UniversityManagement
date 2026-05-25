using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Data.Repositories;

public class TeacherRepository : GenericRepository<Teacher>
{
    public TeacherRepository(UniversityDbContext context) : base(context) { }

    public async Task<List<Teacher>> GetTeachersWithCoursesAsync()
    {
        return await _context.Teachers
            .Include(t => t.Courses)
            .ToListAsync();
    }
}