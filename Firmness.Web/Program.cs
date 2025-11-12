using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Firmness.Web.DataSeeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  Get Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//  Register DbContext (using PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


//  Register Identity (using our Client entity AND configuring Identity services)
builder.Services.AddIdentity<Client, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // <-- This line is crucial for Razor Pages Fallback UI

// Add services to the container.
builder.Services.AddRazorPages();

// This configures the Cookie settings (fixes redirection loops)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

var app = builder.Build();

// --- Execute Seeder to Apply Migrations and Create Roles ---
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        //  Apply the migration (This creates the DB if it doesn't exist)
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure the database is created 
        await context.Database.MigrateAsync(); 
        
        //  Seed the Roles and Admin User (TASK 4)
        await IdentitySeeder.SeedAsync(serviceProvider);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();