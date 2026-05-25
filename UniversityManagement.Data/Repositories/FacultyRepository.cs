using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Data.Repositories;

public class FacultyRepository : GenericRepository<Faculty>
{
    public FacultyRepository(UniversityDbContext context) : base(context) { }

    public async Task<List<Faculty>> GetFacultiesWithGroupsAsync()
    {
        return await _context.Faculties
            .Include(f => f.Groups)
                .ThenInclude(g => g.Students)
            .ToListAsync();
    }

    public async Task<Faculty?> GetFacultyFullAsync(int id)
    {
        return await _context.Faculties
            .Include(f => f.Groups)
                .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
}