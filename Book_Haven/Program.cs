using Fall2024_Assignment3_separal;
using Fall2024_Assignment3_separal.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)); // Ensure SQL Server is used
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Identity services with roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation for testing
    options.Password.RequireDigit = false; // Relax password requirements for testing
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Add Razor Pages services to the container
builder.Services.AddRazorPages();  // <-- Add this line

// Register AIServiceOptions for configuration
builder.Services.Configure<AIServiceOptions>(builder.Configuration.GetSection("AIService"));

// Register IHttpClientFactory (needed for Controllers using IHttpClientFactory)
builder.Services.AddHttpClient();

var app = builder.Build();

// Seed the database with roles and a test admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Ensure roles exist
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create a test admin user
        var adminEmail = "admin@example.com";
        var adminPassword = "Admin123!";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        var userEmail = "user@example.com";
        var userPassword = "User123!";
        var testUser = await userManager.FindByEmailAsync(userEmail);
        if (testUser == null)
        {
            testUser = new IdentityUser { UserName = userEmail, Email = userEmail, EmailConfirmed = true };
            await userManager.CreateAsync(testUser, userPassword);
            await userManager.AddToRoleAsync(testUser, "User");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding roles and admin user: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();  // Map Razor Pages

app.Run();
