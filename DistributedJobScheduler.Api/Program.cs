using DistributedJobScheduler.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static DistributedJobScheduler.Api.AppDbContext;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DistributedJobScheduler.Api.Repositories;
using DistributedJobScheduler.Api.Services;
using DistributedJobScheduler.Api.Config;
using Microsoft.Extensions.Configuration;
using DistributedJobScheduler.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Get configuration
var configuration = builder.Configuration;

// Register database context with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


// Register configuration classes
builder.Services.Configure<RedisConfig>(configuration.GetSection("RedisSettings"));
builder.Services.Configure<DatabaseConfig>(configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

builder.Services.AddSingleton<RedisService>();



// Add repositories
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWorkerNodeRepository, WorkerNodeRepository>();

// Add services
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkerNodeService, WorkerNodeService>();


// Add Identity
builder.Services.AddIdentity<DistributedJobScheduler.Api.Models.ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// Load JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey is missing"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });


// Add authorization policies
builder.Services.AddAuthorizationBuilder()
                                 // Add authorization policies
                                 .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
                                 // Add authorization policies
                                 .AddPolicy("UserOnly", policy => policy.RequireRole("User"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
