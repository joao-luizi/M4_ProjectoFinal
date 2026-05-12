# RideReady

RideReady is a web application built with ASP.NET Core designed to manage school-related transport and user services.

It uses a layered architecture with separate concerns for authentication, domain logic, and data persistence.

## Tech Stack

- ASP.NET Core
- Entity Framework Core
- SQL Server
- ASP.NET Identity

---




## Requirements

Antes de iniciar, garantir que tens instalado:

- .NET SDK 8.0
- SQL Server / SQL Server Express
- Visual Studio 2022 ou JetBrains Rider
- Git
- dotnet-ef CLI tool (if not installed):

```bash
dotnet tool install --global dotnet-ef
```

---




## Clone Repository

```bash
git clone <repository-url>
cd RideReady
````

---

## Local Configuration

This project uses local configuration files that are not committed to the repository.


### Criar ficheiros de configuração

Start by creating a local copy of the configuration file:

```text
.\RideReady\appsettings.example.json
````

Create a new file based on it:

```text
.\RideReady\appsettings.json
```

Optionally, you can also create an environment-specific configuration for development:

```text
.\RideReady\appsettings.Development.json
```

> O ficheiro `appsettings.Development.json` sobrepõe configurações do `appsettings.json` quando a aplicação é executada em ambiente `Development`.

> The `appsettings.Development.json` file overrides values from `appsettings.json` when the application runs in `Development` mode.



---



## Configure Connection Strings

Open `appsettings.json` and update the connection strings according to your local SQL Server setup.

Exemplo:

```json
"ConnectionStrings": {
  "RideReadyDB": "Server=localhost;Database=RideReadyDB;Trusted_Connection=True;TrustServerCertificate=True;",
  "IdentityDb": "Server=localhost;Database=IdentityDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
````


---

## Apply Database Migrations

### 1. Verify EF Core CLI tool

Run:

```bash
dotnet ef
```

If the command is not found, install it globally:

```bash
dotnet tool install --global dotnet-ef
```

Open the solution and in the developer terminal type:

```bash
dotnet ef database update --context ApplicationDbContext --project RideReady --startup-project RideReady
```

And: 
```bash
dotnet ef database update --context EM_DbContext --project RepositoryLibrary --startup-project RideReady
```
You should see the output `Build started...` and `Build succeeded`. a cascade of info output and finally `Done.`


---



## Run the Project

### Visual Studio

Oprn solution and start the project.

### CLI

```bash
dotnet run
````


---


## Troubleshooting

### Database connection issues

If the application cannot connect to the database, verify the following:

- SQL Server is running locally
- Connection strings in appsettings.json are correct
- The SQL Server instance name matches your local setup (e.g. localhost, localhost\SQLEXPRESS)
- The database user has sufficient permissions (if not using Windows Authentication)
- TrustServerCertificate=True is set if using a local/development environment

### Migration errors

If database migrations fail, check the following:

- The correct startup project is selected (RideReady)
- The correct DbContext is being targeted:
- ApplicationDbContext (Identity)
- EM_DbContext (Application data)
- Migrations exist in the correct project (RepositoryLibrary)
- Entity Framework CLI tools are installed:


---
# Old Readme

# RideReady

## Getting Started

To get this project up and running on your local machine, follow the steps below:

### 1. Update the Connection Strings

Before running the application, you **must** update the connection strings in the `appSettings` file.  
These connection strings are required for the application to connect to the correct database and services.

- Open the `appSettings.json`.
- Locate the section with the connection strings.
- Replace the placeholders or existing values with your own valid connection strings.

### 2. Run the Application

Once the connection strings have been updated, run the **RideReady** program.
