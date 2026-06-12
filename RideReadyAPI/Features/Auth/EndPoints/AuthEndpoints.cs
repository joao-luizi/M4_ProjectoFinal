using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RepositoryLibrary.Features.Users.Entities;
using RideReadyAPI.Features.Auth.DTOs;
using RideReadyAPI.Features.Auth.Services;
using System.Security.Claims;
namespace RideReadyAPI.Features.Auth.EndPoints
{

    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {


            app.MapGet("/api/health", () =>
            {
                return Results.Ok(new
                {
                    status = "Healthy",
                    uptime = "Running",
                    serverTime = DateTime.UtcNow
                });
            });

            app.MapGet("/api", () =>
            {
                return Results.Ok(new
                {
                    project = "RideReady",
                    modules = new[]
                    {
            "Authentication (JWT + Identity)",
            "Booking system",
            "Subscriptions",
            "Roles (Student/Teacher/Admin)"
        }
                });
            });

            app.MapGet("/api/docs", () =>
            {
                return Results.Ok(new
                {
                    login = "POST /api/auth/login",
                    debug = "GET /api/auth/debug",
                    studentTest = "GET /api/auth/test_student",
                    teacherTest = "GET /api/auth/test_teacher",
                    adminTest = "GET /api/auth/test_admin"
                });
            });

            // TEST ENDPOINTS

            app.MapGet("/api/auth/test_student",
            () =>
                {
                    return Results.Ok("Hello Student");
                }).RequireAuthorization(policy =>
    policy.RequireRole(StaticRole.Student)); ; 

            app.MapGet("/api/auth/test_teacher",
            () =>
                {
                    return Results.Ok("Hello Teacher");
                }).RequireAuthorization(policy =>
    policy.RequireRole(StaticRole.Teacher));

            app.MapGet("/api/auth/test_admin",
            () =>
                {
                    return Results.Ok("Hello Admin");
                }).RequireAuthorization(policy =>
    policy.RequireRole(StaticRole.Admin));

            app.MapGet("/api/auth/authenticated",
            () =>
                {
                    return Results.Ok("Authenticated");
                });

            app.MapGet("/api/auth/debug", (ClaimsPrincipal user) =>
            {
                return Results.Ok(new
                {
                    isAuthenticated = user.Identity?.IsAuthenticated,
                    name = user.Identity?.Name,
                    claims = user.Claims.Select(c => new { c.Type, c.Value })
                });
            });


            app.MapPost("/api/auth/login", async (
                Microsoft.AspNetCore.Identity.Data.LoginRequest request,
                UserManager<EMUser> userManager,
                JwtTokenService jwtService
            ) =>
            {
                var user = await userManager.FindByEmailAsync(request.Email);

                if (user is null)
                    return Results.Unauthorized();

                var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);

                if (!passwordValid)
                    return Results.Unauthorized();

                var roles = await userManager.GetRolesAsync(user);

                var token = jwtService.CreateToken(user, roles);

                return Results.Ok(new LoginResponse(token, user.Email!));
            });


            return app;
        }
    }
}
