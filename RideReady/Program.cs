using EquestrianManagement.Seeds;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using RepositoryLibrary.IServices;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Seeds;
using RepositoryLibrary.Services;
using RideReady.Components;
using RideReady.Components.Account;
using RideReady.Data;
using SharedLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

var connectionString = builder.Configuration.GetConnectionString("UsersContextConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var rrCnString = builder.Configuration.GetConnectionString("RRCnString") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<EM_DbContext>(options =>
    options.UseSqlServer(rrCnString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<EMUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeed.SeedRolesAsync(services);
    await UserSeed.UserSeedWithRole(services);
    await SchoolSeed.SeedSchoolAsync(services);
    await SchoolSeed.SeedSchoolUserAsync(services);
    await LessonTypeSeed.SeedLessonTypeAsync(services);
    await LessonSeed.SeedLessons(services);
    await HorseSeed.SeedHorses(services);
    await PackageSeed.SeedPackages(services);
    await PaymentSeed.SeedPayments(services);
}

app.Run();
