using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class ReportService
{
    private readonly IRepository<Report> _repository;

    public ReportService(IRepository<Report> repository)
    {
        _repository = repository;
    }

    public async Task<List<Report>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Report?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddReportAsync(Report report)
    {
        await _repository.AddAsync(report);
    }

    public async Task UpdateReportAsync(Report report)
    {
        await _repository.UpdateAsync(report);
    }

    public async Task DeleteReportAsync(int id)
    {
        var report = await _repository.GetByIdAsync(id);
        if (report != null)
        {
            await _repository.DeleteAsync(id);
        }
    }
}