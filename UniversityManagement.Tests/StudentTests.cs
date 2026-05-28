using FluentAssertions;
using UniversityManagement.Core.Models;
using Xunit;

namespace UniversityManagement.Tests;

public class StudentTests
{
    [Fact]
    public void Student_Should_Create_Correctly()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "Ksenia",
            LastName = "Olkhovska",
            Email = "ksenia@test.com",
            Rating = 95
        };

        // Assert
        student.FirstName.Should().Be("Ksenia");
        student.LastName.Should().Be("Olkhovska");
        student.Email.Should().Be("ksenia@test.com");
        student.Rating.Should().Be(95);
    }
}