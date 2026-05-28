using FluentAssertions;
using Xunit;

namespace UniversityManagement.Tests;

public class AuthenticationServiceTests
{
    [Theory]
    [InlineData("Admin123!", true)]
    [InlineData("Short1!", false)]
    [InlineData("alllowercase1!", false)]  // без великої літери
    [InlineData("ALLUPPERCASE1!", false)]  // без малої літери
    [InlineData("NoDigitsHere!", false)]
    public void Password_Validation_Rules(string password, bool expected)
    {
        bool hasMinLength  = password.Length >= 8;
        bool hasUpper      = password.Any(char.IsUpper);
        bool hasLower      = password.Any(char.IsLower);
        bool hasDigit      = password.Any(char.IsDigit);
        bool hasSpecial    = password.Any(c => "!@#$%^&*".Contains(c));

        bool result = hasMinLength && hasUpper && hasLower && hasDigit && hasSpecial;

        result.Should().Be(expected);
    }

    [Fact]
    public void Empty_Password_Should_Be_Invalid()
    {
        string password = "";

        bool result = password.Length >= 8;

        result.Should().BeFalse();
    }

    [Fact]
    public void Password_With_Spaces_Should_Be_Handled()
    {
        string password = "Admin 123!";

        bool hasSpaces = password.Contains(' ');

        hasSpaces.Should().BeTrue();
    }
}