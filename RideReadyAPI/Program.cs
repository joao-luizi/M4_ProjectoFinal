using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users.Entities;
using RideReady.Data;
using RideReadyAPI.Features.Auth.EndPoints;
using RideReadyAPI.Features.Auth.Services;
using System.Security.Claims;
using System.Text;

namespace RideReadyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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


            builder.Services.AddIdentity<EMUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

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

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
    ),

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };
            });
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "RideReady API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter JWT like: Bearer {your token}"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });


            builder.Services.AddScoped<JwtTokenService>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.

            

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapAuthEndpoints();


            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.MapGet("/", (IConfiguration config) =>
            {
                return Results.Ok(new
                {
                    name = "RideReady API",
                    status = "Running",
                    version = "1.0",
                    environment = app.Environment.EnvironmentName,
                    authentication = "JWT + ASP.NET Identity",
                    database = "SQL Server (Identity + RideReadyDB)",
                    endpoints = new
                    {
                        auth = "/api/auth/login",
                        debug = "/api/auth/debug",
                        health = "/api/health"
                    },
                    timestamp = DateTime.UtcNow
                });
            });


            app.Run();
        }
    }
}
