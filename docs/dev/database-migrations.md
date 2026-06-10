# Database Migrations Guide

## Requirements

Before running any Entity Framework Core migration commands, ensure the following prerequisites are met:

### 1. EF Core Tools Installed

The Entity Framework Core CLI tools must be installed in the development environment.

You can install them globally using:

```bash
dotnet tool install --global dotnet-ef

Or update them if already installed:

dotnet tool update --global dotnet-ef

Additionally, the project must reference the EF Core design package:

Microsoft.EntityFrameworkCore.Design

2. Correct Startup Project

All commands must be executed using the correct startup project:

Startup Project: RideReady

This is required because EF Core reads configuration (such as connection strings) from the startup project. 

3. Where to Run Commands

Migration commands can be executed in any of the following environments:

Visual Studio Package Manager Console (PowerShell)
Windows PowerShell
Terminal inside Visual Studio
External terminal (e.g. Windows Terminal, CMD, or Bash)

When using Visual Studio, ensure that:

The Default Project in the Package Manager Console is set to RepositoryLibrary
The correct startup project (RideReady) is selected in the solution configuration
4. General Rule

All EF Core commands in this project follow this structure:

--project RepositoryLibrary → where DbContexts and migrations live
--startup-project RideReady → where configuration and connection strings are defined

## Overview

RideReady uses two separate SQL Server databases:

1. **Identity Database** (`AppIdentityDbContext`)

   * Stores ASP.NET Identity data.
   * Users, roles, claims, authentication-related information.

2. **Application Database** (`RideReadyDbContext`)

   * Stores all RideReady business data.
   * Lessons, bookings, horses, subscriptions, purchases, payments, etc.

This separation was a deliberate architectural choice to isolate Identity concerns from application domain data.

The project uses **Entity Framework Core Code First**, meaning database schema changes are generated from C# entity and DbContext changes through migrations.

---

## Important Architecture Notes

All migration commands are executed against:

* **Project containing the DbContexts and entities:** `RepositoryLibrary`
* **Startup project:** `RideReady`

The startup project is important because Entity Framework reads the connection strings from its configuration.

Example configuration from `Program.cs`:

```csharp
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
```

### Connection Strings

Migration commands do **not** contain database names.

The actual target database is determined by the connection strings configured in the startup project (`RideReady`).

Before running migration commands, verify:

* `appsettings.json`
* `appsettings.Development.json`
* User secrets (if applicable)

to ensure the correct databases are configured.

---

# Creating a New Migration

Whenever an entity, relationship, configuration, or DbContext model changes, a migration should be created.

## Identity Database

```bash
dotnet ef migrations add MigrationName 
    --context AppIdentityDbContext 
    --project RepositoryLibrary 
    --startup-project RideReady 
    --output-dir Data/Migrations/Identity
```

Example:

```bash
dotnet ef migrations add AddTeacherRole 
    --context AppIdentityDbContext 
    --project RepositoryLibrary 
    --startup-project RideReady 
    --output-dir Data/Migrations/Identity
```

---

## RideReady Database

```bash
dotnet ef migrations add MigrationName --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady --output-dir Data/Migrations/RideReady
```

Example:

```bash
dotnet ef migrations add AddMigrationName --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady --output-dir Data/Migrations/RideReady
```

---

# Applying Migrations

Apply pending migrations to the database.

## RideReady Database

```bash
dotnet ef database update --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady
```

## Identity Database

```bash
dotnet ef database update --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady
```

---

# Initial Database Creation

Initial migrations were created using:

## Identity

```bash
dotnet ef migrations add InitialIdentity --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady --output-dir Data/Migrations/Identity
```

## RideReady

```bash
dotnet ef migrations add InitialCreate --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady --output-dir Data/Migrations/RideReady
```

After creating migrations:

```bash
dotnet ef database update --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady

dotnet ef database update --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady
```

---

# Removing the Last Migration

If a migration was created but has not yet been applied to the database:

## Identity

```bash
dotnet ef migrations remove --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady
```

## RideReady

```bash
dotnet ef migrations remove --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady
```

---

# Dropping Databases

For development purposes it may be useful to completely remove a database and recreate it.

## Identity Database

```bash
dotnet ef database drop --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady --force
```

## RideReady Database

```bash
dotnet ef database drop --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady --force
```

After dropping databases, recreate them by running:

```bash
dotnet ef database update --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady

dotnet ef database update --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady
```
## Recreating Databases

If the databases were dropped but migration files still exist in the repository, recreate them by running:

```bash
dotnet ef database update --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady

dotnet ef database update --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady
---

Entity Framework will automatically:

- Create the databases (if they do not exist)
- Apply all existing migrations in order

If the databases were dropped and migration files do not exist (or were deleted), you need to recreate the initial migrations before updating:

dotnet ef migrations add InitialIdentity --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady --output-dir Data/Migrations/Identity

dotnet ef migrations add InitialCreate --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady --output-dir Data/Migrations/RideReady


# Recommended Workflow

When modifying entities:

1. Modify entity classes.
2. Update Fluent API configurations if required.
3. Create a migration.
4. Review the generated migration code.
5. Apply the migration locally.
6. Verify the database schema.
7. Commit:

   * Entity changes
   * Configuration changes
   * Migration files

Typical workflow:

```bash
dotnet ef migrations add AddBookingFundingType --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady --output-dir Data/Migrations/RideReady

dotnet ef database update --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady
```

---

# Migration History

Each database maintains its own migration history table:

| Database  | History Table                   |
| --------- | ------------------------------- |
| Identity  | __EFMigrationsHistory_Identity  |
| RideReady | __EFMigrationsHistory_RideReady |

This allows both DbContexts to evolve independently while sharing the same solution.
