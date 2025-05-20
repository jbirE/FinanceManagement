using FinanceManagement.Data;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Implementation;
using FinanceManagement.Repositories.Interface;
using FinanceManagement.Services;
using FinanceManagement.SignalRjobs.Notification;
using FinanceManagement.SignalRjobs.Hubs;
using FinanceTool.Repositories.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid.Extensions.DependencyInjection;
using System.Text;
using FinanceManagement.Repositories;
using FinanceManagement.SignalRjobs;

var builder = WebApplication.CreateBuilder(args);

// Load the configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("Properties/launchSettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.
var connectionString = configuration.GetConnectionString("cnx");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<Utilisateur, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Ajouter le provider
builder.Services.AddSingleton<NotificationProvider>();

// Register Repositories
builder.Services.AddScoped<IDepartementRepository, DepartementRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRapportDepenseRepository, RapportDepenseRepository>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IBudgetDepartementRepository, BudgetDepartementRepository>();
builder.Services.AddScoped<IBudgetProjetRepository, BudgetProjetRepository>();



// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<BudgetProjetService>();
builder.Services.AddScoped<BudgetDepartementService>();

builder.Services.AddScoped<ProjetService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ProjetService>();
builder.Services.AddScoped<DepartementService>();
builder.Services.AddScoped<UtilisateurService>();
builder.Services.AddSignalR();









// Add SendGrid
builder.Services.AddSendGrid(options =>
{
    options.ApiKey = config["SendGrid:ApiKey"];
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("latest", new OpenApiInfo { Title = "API", Version = "latest" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] then the valid token in the text input below"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["JWT:Issuer"],
        ValidAudience = config["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("NgOrigins", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Map the SignalR hub
app.MapHub<WorkerHub>("/hubs/worker");

// Only seed in development/production, not when running migrations
if (!args.Contains("--no-seed"))
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await DataSeeder.SeedAdminUser(services);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
        // Continue with application startup even if seeding fails
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("/swagger/latest/swagger.json", "API latest");
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("/swagger/latest/swagger.json", "API latest");
    });
}

app.UseCors("NgOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();