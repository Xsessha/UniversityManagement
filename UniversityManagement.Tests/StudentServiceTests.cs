using FluentAssertions;
using Moq;
using UniversityManagement.Core.Interfaces;
using UniversityManagement.Core.Models;
using UniversityManagement.Services;
using Xunit;

namespace UniversityManagement.Tests;

public class StudentServiceTests
{
    private static Mock<IRepository<Student>> CreateMock(List<Student>? students = null)
    {
        var mock = new Mock<IRepository<Student>>();
        mock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(students ?? new List<Student>());
        return mock;
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Students()
    {
        var students = new List<Student>
        {
            new() { Id = 1, FirstName = "Ksenia", LastName = "Olkhovska" },
            new() { Id = 2, FirstName = "Ivan",   LastName = "Ivanov"    }
        };

        var service = new StudentService(CreateMock(students).Object);
        var result  = await service.GetAllAsync();

        result.Should().HaveCount(2);
        result[0].FirstName.Should().Be("Ksenia");
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Students()
    {
        var service = new StudentService(CreateMock().Object);
        var result  = await service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Correct_Student()
    {
        var student = new Student { Id = 1, FirstName = "Ksenia", LastName = "Olkhovska" };
        var mock    = new Mock<IRepository<Student>>();
        mock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(student);

        var service = new StudentService(mock.Object);
        var result  = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Ksenia",    result.FirstName);
        Assert.Equal("Olkhovska", result.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        var mock = new Mock<IRepository<Student>>();
        mock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Student?)null);

        var service = new StudentService(mock.Object);
        var result  = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddStudentAsync_Should_Call_Repository_Once()
    {
        var mock = new Mock<IRepository<Student>>();
        mock.Setup(x => x.AddAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask);

        var service = new StudentService(mock.Object);
        var student = new Student { FirstName = "New", LastName = "Student" };

        await service.AddStudentAsync(student);

        mock.Verify(x => x.AddAsync(student), Times.Once);
    }

    [Fact]
    public async Task DeleteStudentAsync_Should_Call_Delete_When_Student_Exists()
    {
        var student = new Student { Id = 5, FirstName = "To", LastName = "Delete" };
        var mock    = new Mock<IRepository<Student>>();
        mock.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(student);
        mock.Setup(x => x.DeleteAsync(5)).Returns(Task.CompletedTask);

        var service = new StudentService(mock.Object);
        await service.DeleteStudentAsync(5);

        mock.Verify(x => x.DeleteAsync(5), Times.Once);
    }

    [Fact]
    public async Task DeleteStudentAsync_Should_Not_Call_Delete_When_Student_Missing()
    {
        var mock = new Mock<IRepository<Student>>();
        mock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Student?)null);

        var service = new StudentService(mock.Object);
        await service.DeleteStudentAsync(999);

        mock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}