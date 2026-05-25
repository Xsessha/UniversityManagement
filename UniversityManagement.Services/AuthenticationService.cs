using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Services;

public class AuthenticationService
{
    private readonly UniversityDbContext _context;

    public AuthenticationService(
        UniversityDbContext context)
    {
        _context = context;
    }

    public User? Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
            return null;

        var user = _context.Users.FirstOrDefault(u =>
            u.Email == email &&
            u.Password == password);

        return user;
    }

    public User? Register(
        string fullName,
        string email,
        string password)
    {
        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
            return null;

        var existingUser = _context.Users
            .FirstOrDefault(u => u.Email == email);

        if (existingUser != null)
            return null;

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Password = password,
            Role = "Student"
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public bool UserExists(string email)
    {
        return _context.Users.Any(u =>
            u.Email == email);
    }

    public async Task LogoutAsync()
    {
        await Task.CompletedTask;
    }
}