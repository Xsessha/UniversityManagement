using UniversityManagement.Web.Extensions;
using UniversityManagement.Web.Middleware;
using UniversityManagement.Web.Hubs;
using Microsoft.AspNetCore.Identity;
using UniversityManagement.Core.Models;
using UniversityManagement.Core.Enums;
using UniversityManagement.Data.Context;
using UniversityManagement.Data.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";
var connectionStringName = databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
    ? "SqlServerConnection"
    : "DefaultConnection";
var connectionString = builder.Configuration.GetConnectionString(connectionStringName)
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' was not found.");
var autoCreateDatabase = builder.Configuration.GetValue<bool>("AutoCreateDatabase");
var seedMockData = builder.Configuration.GetValue("SeedMockData", true);

builder.Services.AddDbContext<UniversityDbContext>(options =>
{
    if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(connectionString);
        return;
    }

    options.UseSqlServer(connectionString);
});

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<UniversityDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddBusinessServices();
builder.Services.AddSignalR();
builder.Services.AddSession();

var app = builder.Build();

if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase) || autoCreateDatabase)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await SeedIdentityAsync(scope.ServiceProvider);

    if (seedMockData)
    {
        await SeedData.SeedAsync(dbContext);
    }
}

app.UseMiddleware<ExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Home/StatusCode/{0}");

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");

app.Run();

static async Task SeedIdentityAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var role in Enum.GetNames<UserRole>())
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var users = new[]
    {
        new { Email = "admin@lumina.edu", Password = "Admin123!", FirstName = "System", LastName = "Administrator", Role = UserRole.Admin },
        new { Email = "teacher@lumina.edu", Password = "Teacher123!", FirstName = "Hannah", LastName = "Lewis", Role = UserRole.Teacher },
        new { Email = "student@lumina.edu", Password = "Student123!", FirstName = "Olivia", LastName = "Bennett", Role = UserRole.Student }
    };

    foreach (var userSeed in users)
    {
        var user = await userManager.FindByEmailAsync(userSeed.Email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = userSeed.Email,
                Email = userSeed.Email,
                FirstName = userSeed.FirstName,
                LastName = userSeed.LastName,
                Role = userSeed.Role,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, userSeed.Password);
        }
        else
        {
            user.FirstName = userSeed.FirstName;
            user.LastName = userSeed.LastName;
            user.Role = userSeed.Role;
            await userManager.UpdateAsync(user);
        }

        var roleName = userSeed.Role.ToString();
        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
    }

    foreach (var user in userManager.Users.ToList())
    {
        var roleName = user.Role.ToString();
        if (!string.IsNullOrWhiteSpace(roleName) && await roleManager.RoleExistsAsync(roleName) && !await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}
