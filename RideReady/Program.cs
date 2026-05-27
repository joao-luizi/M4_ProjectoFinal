using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Data.Seeds;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.Bookings.Services;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Horses.Services;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Lessons.Services;
using RepositoryLibrary.Features.Products;
using RepositoryLibrary.Features.Products.Interfaces;
using RepositoryLibrary.Features.Products.Services;
using RepositoryLibrary.Features.Purchases.Interfaces;
using RepositoryLibrary.Features.Purchases.Services;
using RepositoryLibrary.Features.Schools;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using RepositoryLibrary.Features.Users.Service;
using RepositoryLibrary.IRepository;
using RepositoryLibrary.IServices;
using RepositoryLibrary.Repository;
using RepositoryLibrary.Services;
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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IHorseService, HorseService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<ILessonTypeService, LessonTypeService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IProductService, ProductService>();
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


public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await RoleSeed.SeedRolesAsync(services);
        await UserSeed.UserSeedWithRole(services);
        await SchoolSeed.SeedSchoolAsync(services);
        await SchoolSeed.SeedSchoolUserAsync(services);
        await LessonTypeSeed.SeedLessonTypeAsync(services);
        await LessonSeed.SeedLessons(services);
        await HorseSeed.SeedHorses(services);
        await ProductSeed.SeedProductsAsync(services);
        await EntitlementSeed.SeedEntitlementsAsync(services);

    }
}