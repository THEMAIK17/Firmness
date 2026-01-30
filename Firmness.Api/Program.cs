using System.Text;
using Firmness.Application.Auth;
using Firmness.Application.Services;
using Firmness.Application.Services.Auth;
using Firmness.Application.Services.Email;
using Firmness.Application.Settings;
using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Debug: Log environment and configuration
Console.WriteLine($"---> ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
Console.WriteLine($"---> ConnectionStrings__DefaultConnection ENV: {Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")}");
Console.WriteLine($"---> ConnectionString from Configuration: {builder.Configuration.GetConnectionString("DefaultConnection")}");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//  Register DbContext (using PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentityCore<Client>() 
    .AddRoles<IdentityRole>()              
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager<SignInManager<Client>>();

// JWT Configuration

// Map the appsettings configuration to the strongly typed class
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Register the token generator 
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

// Configure the authentication scheme
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });



// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
// Configure swagger for Jwt
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Firmness API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddAutoMapper(typeof(Program));
// Configure the email settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register the email 
builder.Services.AddScoped<IEmailService, GmailEmailService>();

// Register the PDF service
builder.Services.AddScoped<IPdfService, PdfService>();

//Allow the React Frontend to connect
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()  
            .AllowAnyHeader()  
            .AllowAnyMethod();
    });
});
var app = builder.Build();

// This block ensures the DB is created and populated when Docker starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<Client>>();
        
        await context.Database.MigrateAsync();
        
        await Firmness.Infraestructure.Data.StoreSeeder.SeedAsync(context, userManager);
        
        Console.WriteLine("--> Database migration and seeding completed successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Activate CORS policy
app.UseCors("AllowReactApp");

// Add the authentication middleware to the pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();