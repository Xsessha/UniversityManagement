using UniversityManagement.Core.Models;
using UniversityManagement.Data.Repositories;

namespace UniversityManagement.Services;

public class GroupService
{
    private readonly GroupRepository _repository;

    public GroupService(
        GroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Group>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Group?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(Group group)
    {
        await _repository.AddAsync(group);
    }

    public async Task UpdateAsync(Group group)
    {
        await _repository.UpdateAsync(group);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
