using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Data.Repositories;

public class StudentRepository : GenericRepository<Student>
{
    public StudentRepository(UniversityDbContext context) : base(context) { }

    public async Task<List<Student>> GetStudentsWithGroupAsync()
    {
        return await _context.Students
            .Include(s => s.Group)
            .ToListAsync();
    }
}