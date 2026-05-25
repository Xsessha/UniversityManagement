using UniversityManagement.Web.Extensions;
using UniversityManagement.Web.Middleware;
using UniversityManagement.Web.Hubs;
using Microsoft.AspNetCore.Identity;
using UniversityManagement.Core.Models;
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
