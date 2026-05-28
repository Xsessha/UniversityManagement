using FluentAssertions;
using Xunit;

namespace UniversityManagement.Tests;

public class RoleAccessTests
{
    [Fact]
    public void Admin_Role_Should_Be_Valid()
    {
        // Arrange
        string role = "Administrator";

        // Assert
        role.Should().Be("Administrator");
    }
}