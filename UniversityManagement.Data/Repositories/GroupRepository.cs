using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Data.Repositories;

public class GroupRepository : GenericRepository<Group>
{
    public GroupRepository(UniversityDbContext context) : base(context) { }

    public async Task<List<Group>> GetGroupsWithStudentsAsync()
    {
        return await _context.Groups
            .Include(g => g.Students)
            .Include(g => g.Faculty)
            .ToListAsync();
    }

    public async Task<Group?> GetGroupDetailsAsync(int id)
    {
        return await _context.Groups
            .Include(g => g.Students)
            .Include(g => g.Courses)
            .Include(g => g.Faculty)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}