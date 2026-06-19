using LibrarySystem.Business.AuthorBusiness;
using LibrarySystem.Business.BookBusiness;
using LibrarySystem.Business.BorrowBusiness;
using LibrarySystem.Business.CategoryBusiness;
using LibrarySystem.Business.MemberBusiness;
using LibrarySystem.Business.PublicationBusiness;
using LibrarySystem.Business.ReportBusiness;
using LibrarySystem.Business.UserBusiness;
using LibrarySystem.Helpers;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

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

        // Read lockout settings from configuration, with safe defaults
        var maxFailedAttempts = builder.Configuration.GetValue<int>("IdentitySettings:MaxFailedAccessAttempts", 5);
        var lockoutMinutes = builder.Configuration.GetValue<int>("IdentitySettings:DefaultLockoutMinutes", 1);

        // Configure standard ASP.NET Core Identity with ApplicationUser
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Simple password policies for training demonstration
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            // Lockout settings read dynamically
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutMinutes); 
            options.Lockout.MaxFailedAccessAttempts = maxFailedAttempts;                     
            options.Lockout.AllowedForNewUsers = true;                                       
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure custom redirect paths for standard cookies
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Auth/Login";
            options.LogoutPath = "/Auth/Logout";
            options.AccessDeniedPath = "/Auth/AccessDenied";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Standard ASP.NET Core Authentication middleware
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        // Seed Roles and Default Users at application startup
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            SeedData.SeedIdentityAsync(services).GetAwaiter().GetResult();
        }

        app.Run();
    }
}
