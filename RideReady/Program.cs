using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Data.Seeds;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.Bookings.Repositories;
using RepositoryLibrary.Features.Bookings.Services;
using RepositoryLibrary.Features.DashBoard.Interfaces;
using RepositoryLibrary.Features.DashBoard.Services;
using RepositoryLibrary.Features.Entitlements.Interfaces;
using RepositoryLibrary.Features.Entitlements.Repositories;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Horses.Repositories;
using RepositoryLibrary.Features.Horses.Services;
using RepositoryLibrary.Features.Images.Interfaces;
using RepositoryLibrary.Features.Images.Services;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Lessons.Repositories;
using RepositoryLibrary.Features.Lessons.Services;
using RepositoryLibrary.Features.Products;
using RepositoryLibrary.Features.Products.Interfaces;
using RepositoryLibrary.Features.Products.Repositories;
using RepositoryLibrary.Features.Products.Services;
using RepositoryLibrary.Features.Purchases.Interfaces;
using RepositoryLibrary.Features.Purchases.Repositories;
using RepositoryLibrary.Features.Purchases.Services;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Schools.Repositories;
using RepositoryLibrary.Features.Schools.Services;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using RepositoryLibrary.Features.Users.Repositories;
using RepositoryLibrary.Features.Users.Repository;
using RepositoryLibrary.Features.Users.Service;
using RepositoryLibrary.Features.Users.Services;
using RideReady.Components;
using RideReady.Components.Account;
using RideReady.Data;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/rideready-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

var identityConn = builder.Configuration.GetConnectionString("IdentityDb")
    ?? throw new InvalidOperationException("IdentityDb not found");

var rideConn = builder.Configuration.GetConnectionString("RideReadyDB")
    ?? throw new InvalidOperationException("RideReadyDB not found");

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(identityConn, x =>
        x.MigrationsHistoryTable("__EFMigrationsHistory_Identity")));

builder.Services.AddDbContext<RideReadyDbContext>(options =>
    options.UseSqlServer(rideConn, x =>
        x.MigrationsHistoryTable("__EFMigrationsHistory_RideReady")));

builder.Services.AddIdentityCore<EMUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<EMUser>, CustomUserClaimsPrincipalFactory>();

builder.Services.AddSingleton<IEmailSender<EMUser>, IdentityNoOpEmailSender>();

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();
builder.Services.AddScoped<ITeacherDashboardService, TeacherDashboardService>();
builder.Services.AddScoped<IEntitlementRepository, EntitlementRepository>();
builder.Services.AddScoped<IHorsePhotoRepository, HorsePhotoRepository>();
builder.Services.AddScoped<IHorseRepository, HorseRepository>();
builder.Services.AddScoped<IHorseService, HorseService>();
builder.Services.AddScoped<ITeacherHorseService, TeacherHorseService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ILessonTypeRepository, LessonTypeRepository>();
builder.Services.AddScoped<ILessonTypeService, LessonTypeService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<ISchoolService, SchoolService>();

builder.Services.AddScoped<ISchoolUserService, SchoolUserService>();
builder.Services.AddScoped<ISchoolUsersRepository, SchoolUsersRepository>();
builder.Services.AddScoped<IStudentExportService, StudentExportService>();
//builder.Services.AddScoped<IUserHorseService, UserHorseService>();
builder.Services.AddScoped<IUserFotoRepository, UserFotoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ILessonScheduleService, LessonScheduleService>();

builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();

builder.Services.AddRadzenComponents();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    await SeedData.InitializeAsync(services);
}


app.Run();


