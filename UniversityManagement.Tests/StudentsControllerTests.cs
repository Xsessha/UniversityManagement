using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;
using UniversityManagement.Services;
using UniversityManagement.Web.Controllers;
using Xunit;

namespace UniversityManagement.Tests;

public class StudentsControllerTests
{
    private static UniversityDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new UniversityDbContext(options);
    }

    private static StudentsController CreateController(UniversityDbContext context)
        => new StudentsController(
            new StudentService(new FakeStudentRepository()),
            context,
            null!,
            null!);

    [Fact]
    public async Task Index_Should_Return_ViewResult_With_Students()
    {
        using var context = CreateContext();
        context.Students.Add(new Student
        {
            FirstName = "Ksenia",
            LastName  = "Olkhovska",
            Email     = "ksenia@test.com",
            Rating    = 95
        });
        await context.SaveChangesAsync();

        var result = await CreateController(context).Index();

        // FluentAssertions 8.x: використовуємо Assert.IsType для reference types
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Index_Should_Return_ViewResult_When_No_Students()
    {
        using var context = CreateContext();

        var result = await CreateController(context).Index();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Details_Should_Return_NotFound_When_Student_Missing()
    {
        using var context = CreateContext();

        var result = await CreateController(context).Details(999);

        Assert.IsType<NotFoundResult>(result);
    }


    [Fact]
    public async Task Edit_Should_Return_NotFound_When_Student_Missing()
    {
        using var context = CreateContext();

        var result = await CreateController(context).Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }


    private class FakeStudentRepository : IRepository<Student>
    {
        private readonly List<Student> _data = new();

        public Task AddAsync(Student entity)
        {
            _data.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            _data.RemoveAll(s => s.Id == id);
            return Task.CompletedTask;
        }

        public Task<Student?> GetByIdAsync(int id)
            => Task.FromResult(_data.FirstOrDefault(s => s.Id == id));

        public Task<List<Student>> GetAllAsync()
            => Task.FromResult(_data.ToList());

        public Task UpdateAsync(Student entity)
            => Task.CompletedTask;
    }
}