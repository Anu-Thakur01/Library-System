using System.Text;
using LibrarySystem.Business.AuthorBusiness;
using LibrarySystem.Business.BookBusiness;
using LibrarySystem.Business.BorrowBusiness;
using LibrarySystem.Business.CategoryBusiness;
using LibrarySystem.Business.MemberBusiness;
using LibrarySystem.Business.PublicationBusiness;
using LibrarySystem.Business.ReportBusiness;
using LibrarySystem.Business.UserBusiness;
using LibrarySystem.Repository.AuthorRepository;
using LibrarySystem.Repository.BookRepository;
using LibrarySystem.Repository.BorrowRepository;
using LibrarySystem.Repository.CategoryRepository;
using LibrarySystem.Repository.Data;
using LibrarySystem.Repository.MemberRepository;
using LibrarySystem.Repository.Models;
using LibrarySystem.Repository.PublicationRepository;
using LibrarySystem.Repository.ReportRepository;
using LibrarySystem.Repository.UserRepository;
using LibrarySystem.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Enable Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library Management System API",
        Version = "v1",
        Description = "Web API for Library Management System with JWT Authentication and PostgreSQL/SQLite support."
    });

    // Configure Swagger to support JWT Bearer authorization
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer <your_token_here>"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Business & Repository DI
builder.Services.AddScoped<IBookBusiness, BookBusiness>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<ICategoryBusiness, CategoryBusiness>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPublicationBusiness, PublicationBusiness>();
builder.Services.AddScoped<IPublicationRepository, PublicationRepository>();

builder.Services.AddScoped<IAuthorBusiness, AuthorBusiness>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddScoped<IMemberBusiness, MemberBusiness>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();

builder.Services.AddScoped<IBorrowBusiness, BorrowBusiness>();
builder.Services.AddScoped<IBorrowRepository, BorrowRepository>();

builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IReportBusiness, ReportBusiness>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Configure Dynamic Database Provider (SQLite or PostgreSQL)
var provider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        var pgConn = builder.Configuration.GetConnectionString("PostgresConnection");
        options.UseNpgsql(pgConn);
    }
    else
    {
        var sqliteConn = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlite(sqliteConn);
    }
});

// Configure Identity with same options as MVC Web project
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    var maxFailedAttempts = builder.Configuration.GetValue<int>("IdentitySettings:MaxFailedAccessAttempts", 5);
    var lockoutMinutes = builder.Configuration.GetValue<int>("IdentitySettings:DefaultLockoutMinutes", 1);
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutMinutes);
    options.Lockout.MaxFailedAccessAttempts = maxFailedAttempts;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSecret = builder.Configuration.GetValue<string>("JwtSettings:Secret") ?? "DefaultSuperSecretKey1234567890123456";
var jwtIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer") ?? "LibrarySystem.API";
var jwtAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience") ?? "LibrarySystem.Clients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure Database is Created and Seeded on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Dynamically create the database tables if they do not exist
        context.Database.EnsureCreated();

        // Seed default Identity Roles and Users (SuperAdmin & Staff)
        SeedData.SeedIdentityAsync(services).GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}

app.Run();
