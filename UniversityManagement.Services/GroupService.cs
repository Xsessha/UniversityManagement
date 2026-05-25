using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class GroupService
{
    private readonly IRepository<Group> _repository;

    public GroupService(IRepository<Group> repository)
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

    public async Task AddGroupAsync(Group group)
    {
        await _repository.AddAsync(group);
    }

    public async Task UpdateGroupAsync(Group group)
    {
        await _repository.UpdateAsync(group);
    }

    public async Task DeleteGroupAsync(int id)
    {
        var group = await _repository.GetByIdAsync(id);
        if (group != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}