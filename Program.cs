using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GiftOfGivers.Data;
using GiftOfGivers.Models;
using GiftOfGivers.Repositories;
using GiftOfGivers.Middleware;
using GiftOfGivers.Configuration;

// Testing Azure DevOps Continuous Integration Trigger
var builder = WebApplication.CreateBuilder(args);

// Configure application settings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Application"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("Security"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // Add API controllers

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings - configurable
    var securitySettings = builder.Configuration.GetSection("Security").Get<SecuritySettings>();
    if (securitySettings != null)
    {
        options.Password.RequireDigit = securitySettings.PasswordRequireDigit;
        options.Password.RequireLowercase = securitySettings.PasswordRequireLowercase;
        options.Password.RequireUppercase = securitySettings.PasswordRequireUppercase;
        options.Password.RequireNonAlphanumeric = securitySettings.PasswordRequireNonAlphanumeric;
        options.Password.RequiredLength = securitySettings.PasswordMinLength;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(securitySettings.DefaultLockoutTimeSpanMinutes);
        options.Lockout.MaxFailedAccessAttempts = securitySettings.MaxFailedAccessAttempts;
        options.Lockout.AllowedForNewUsers = true;
    }
    else
    {
        // Fallback defaults
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    }

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    var securitySettings = builder.Configuration.GetSection("Security").Get<SecuritySettings>();
    options.ExpireTimeSpan = TimeSpan.FromHours(securitySettings?.CookieExpirationHours ?? 2);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Register Repositories
builder.Services.AddScoped<IDonationRepository, DonationRepository>();
builder.Services.AddScoped<IVolunteerRepository, VolunteerRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// Register IHttpClientFactory for calling Azure Functions
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // Map API controllers

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Create database if it doesn't exist
        context.Database.EnsureCreated();

        // Seed roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("Employee"))
        {
            await roleManager.CreateAsync(new IdentityRole("Employee"));
        }
        if (!await roleManager.RoleExistsAsync("Donor"))
        {
            await roleManager.CreateAsync(new IdentityRole("Donor"));
        }

        // Seed admin user
        var adminUser = await userManager.FindByEmailAsync("admin@giftofthegivers.org");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@giftofthegivers.org",
                Email = "admin@giftofthegivers.org",
                FullName = "System Administrator",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Seed employee user
        var employeeUser = await userManager.FindByEmailAsync("employee@giftofthegivers.org");
        if (employeeUser == null)
        {
            employeeUser = new ApplicationUser
            {
                UserName = "employee@giftofthegivers.org",
                Email = "employee@giftofthegivers.org",
                FullName = "Employee User",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(employeeUser, "Password123!");
            await userManager.AddToRoleAsync(employeeUser, "Employee");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

app.Run();