# Backend Architecture Documentation

**Proyecto:** Cinema Management System - Backend API
**Framework:** ASP.NET Core 9.0
**Arquitectura:** Clean Architecture
**Última actualización:** 2025-11-03

---

## Tabla de Contenidos

1. [Visión General](#visión-general)
2. [Clean Architecture Layers](#clean-architecture-layers)
3. [Estructura de Directorios](#estructura-de-directorios)
4. [Entidades del Dominio](#entidades-del-dominio)
5. [Repositorios](#repositorios)
6. [Servicios](#servicios)
7. [Controllers](#controllers)
8. [Autenticación y Autorización](#autenticación-y-autorización)
9. [Integración con Firebase](#integración-con-firebase)
10. [Logging y Auditoría](#logging-y-auditoría)
11. [Feature Flags](#feature-flags)
12. [Configuración](#configuración)
13. [Dependency Injection](#dependency-injection)
14. [Mejores Prácticas](#mejores-prácticas)

---

## Visión General

El backend del Cinema Management System está construido siguiendo los principios de **Clean Architecture**, separando las responsabilidades en capas independientes y manteniendo el dominio del negocio aislado de las tecnologías externas.

### Principios Fundamentales

- **Independencia de Frameworks:** El dominio no depende de ASP.NET Core
- **Testeable:** Lógica de negocio testeable sin UI, DB, o servicios externos
- **Independencia de UI:** La UI puede cambiar sin afectar el dominio
- **Independencia de Base de Datos:** Podemos cambiar Firestore por otra DB
- **Independencia de Agentes Externos:** Reglas de negocio no conocen el mundo exterior

---

## Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                     Cinema.Api (Presentation Layer)          │
│                                                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ Controllers   │  │  Services    │  │  Middleware  │     │
│  │ (REST API)    │  │ (Firebase)   │  │  (Audit)     │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│                            │                                │
│                            ↓                                │
└─────────────────────────────────────────────────────────────┘
                             │
                             ↓
┌─────────────────────────────────────────────────────────────┐
│              Cinema.Application (Application Layer)          │
│                                                             │
│  ┌──────────────────────────────────────────────────┐     │
│  │         Repository Interfaces                     │     │
│  │  (IMovieRepository, IUserRepository, etc.)       │     │
│  └──────────────────────────────────────────────────┘     │
│                            │                                │
└─────────────────────────────────────────────────────────────┘
                             │
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                Cinema.Domain (Domain Layer)                  │
│                      (Business Logic)                        │
│                                                             │
│  ┌──────────────────────────────────────────────────┐     │
│  │              Domain Entities                      │     │
│  │  (User, Movie, Screening, TheaterRoom, etc.)     │     │
│  └──────────────────────────────────────────────────┘     │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                             ↑
                             │
┌─────────────────────────────────────────────────────────────┐
│           Cinema.Infrastructure (Infrastructure Layer)       │
│                                                             │
│  ┌──────────────┐  ┌──────────────────────────────┐       │
│  │ Repositories  │  │  External Integrations       │       │
│  │ (Firestore)   │  │  (Firebase, Logging, etc.)   │       │
│  └──────────────┘  └──────────────────────────────┘       │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Responsabilidades por Capa

| Capa | Responsabilidad | Dependencias |
|------|----------------|--------------|
| **Cinema.Api** | Exposición de endpoints, validación de requests, autenticación | Application, Domain, Infrastructure |
| **Cinema.Application** | Definición de casos de uso, interfaces de repositorios | Domain |
| **Cinema.Domain** | Entidades de negocio, reglas de dominio | Ninguna (capa central) |
| **Cinema.Infrastructure** | Implementación de repositorios, acceso a datos externos | Application, Domain |

---

## Estructura de Directorios

```
C:\Users\Guillermo Parini\Documents\Cinema/
├── src/
│   ├── Cinema.Api/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── MoviesController.cs
│   │   │   ├── ScreeningsController.cs
│   │   │   ├── TheaterRoomsController.cs
│   │   │   ├── FoodCombosController.cs
│   │   │   ├── FoodOrdersController.cs
│   │   │   └── FirebaseTestController.cs
│   │   ├── Services/
│   │   │   ├── UserService.cs           # Firebase Auth integration
│   │   │   └── FirestoreUserService.cs  # Firestore user operations
│   │   ├── Utilities/
│   │   │   └── UserActionAuditMiddleware.cs
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Program.cs
│   │   └── Cinema.Api.csproj
│   │
│   ├── Cinema.Application/
│   │   └── Cinema/
│   │       ├── IMovieRepository.cs
│   │       └── IUserRepository.cs
│   │
│   ├── Cinema.Domain/
│   │   └── Entities/
│   │       ├── User.cs
│   │       ├── Movie.cs
│   │       ├── Screening.cs
│   │       ├── TheaterRoom.cs
│   │       ├── FoodCombo.cs
│   │       ├── FoodOrder.cs
│   │       └── LoginDto.cs
│   │
│   └── Cinema.Infrastructure/
│       ├── Repositories/
│       │   ├── FirestoreMovieRepository.cs
│       │   ├── InMemoryMovieRepository.cs
│       │   ├── FirestoreUserRepository.cs
│       │   └── InMemoryUserRepository.cs
│       ├── DependencyInjection.cs
│       └── Cinema.Infrastructure.csproj
│
├── docs/                                # Documentación del proyecto
├── Cinema.sln
├── Directory.Build.props
├── README.md
└── stylecop.json
```

---

## Entidades del Dominio

Todas las entidades están decoradas con atributos de Firestore para mapeo automático.

### User.cs
```csharp
[FirestoreData]
public class User
{
    [FirestoreProperty] public string Uid { get; set; }
    [FirestoreProperty] public string Email { get; set; }
    [FirestoreProperty] public string DisplayName { get; set; }
    [FirestoreProperty] public bool EmailVerified { get; set; }
    [FirestoreProperty] public bool Disabled { get; set; }
    [FirestoreProperty] public string Role { get; set; }
    [FirestoreProperty] public string Password { get; set; }  // ⚠️ Debe hashearse
}
```

**Colección Firestore:** `users`

---

### Movie.cs
```csharp
[FirestoreData]
public class Movie
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string Title { get; set; }
    [FirestoreProperty] public string Description { get; set; }
    [FirestoreProperty] public int DurationMinutes { get; set; }
    [FirestoreProperty] public string Genre { get; set; }
    [FirestoreProperty] public string Director { get; set; }

    // ⚠️ TODO: Agregar campos adicionales
    // - ReleaseDate (DateTime)
    // - Rating (decimal)
    // - PosterUrl (string)
    // - TrailerUrl (string)
    // - Classification (string)
    // - IsActive (bool)
    // - Language (string)
}
```

**Colección Firestore:** `movies`

---

### Screening.cs
```csharp
[FirestoreData]
public class Screening
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string MovieId { get; set; }
    [FirestoreProperty] public string TheaterRoomId { get; set; }
    [FirestoreProperty] public DateTime StartTime { get; set; }
    [FirestoreProperty] public DateTime EndTime { get; set; }

    // ⚠️ TODO: Agregar campos adicionales
    // - Price (decimal)
    // - AvailableSeats (int)
    // - IsActive (bool)
}
```

**Colección Firestore:** `screenings` (pendiente)

---

### TheaterRoom.cs
```csharp
[FirestoreData]
public class TheaterRoom
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public int Capacity { get; set; }

    // ⚠️ TODO: Agregar campos adicionales
    // - Rows (int)
    // - Columns (int)
    // - ScreenType (string)
    // - Features (List<string>)
    // - IsActive (bool)
}
```

**Colección Firestore:** `theaterRooms` (pendiente)

---

### FoodCombo.cs
```csharp
[FirestoreData]
public class FoodCombo
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public string Description { get; set; }
    [FirestoreProperty] public decimal Price { get; set; }
    [FirestoreProperty] public List<string> Items { get; set; }

    // ⚠️ TODO: Agregar campos adicionales
    // - ImageUrl (string)
    // - IsAvailable (bool)
    // - Category (string)
}
```

**Colección Firestore:** `foodCombos` (pendiente)

---

### FoodOrder.cs
```csharp
[FirestoreData]
public class FoodOrder
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string UserId { get; set; }
    [FirestoreProperty] public List<string> FoodComboIds { get; set; }
    [FirestoreProperty] public decimal TotalPrice { get; set; }
    [FirestoreProperty] public string Status { get; set; }

    // ⚠️ TODO: Agregar campos adicionales
    // - BookingId (string)
    // - CreatedAt (DateTime)
    // - UpdatedAt (DateTime)
    // - PaymentMethod (string)
}
```

**Colección Firestore:** `foodOrders` (pendiente)

---

## Repositorios

### Patrón Repository

El patrón Repository abstrae el acceso a datos, permitiendo cambiar la implementación sin afectar la lógica de negocio.

#### Interfaces (Cinema.Application)

**IMovieRepository.cs:**
```csharp
public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> ListAsync(CancellationToken ct);

    // ⚠️ TODO: Agregar métodos
    // Task<Movie?> GetByIdAsync(string id, CancellationToken ct);
    // Task<Movie> AddAsync(Movie movie, CancellationToken ct);
    // Task<Movie> UpdateAsync(Movie movie, CancellationToken ct);
    // Task DeleteAsync(string id, CancellationToken ct);
}
```

**IUserRepository.cs:**
```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(string uid, CancellationToken ct);
    Task<List<User>> ListAsync(CancellationToken ct);
    Task<User> AddAsync(User user, string password, CancellationToken ct);
    Task<User> UpdateAsync(User user, CancellationToken ct);
    Task DeleteAsync(string uid, CancellationToken ct);
}
```

---

#### Implementaciones Firestore (Cinema.Infrastructure)

**FirestoreMovieRepository.cs:**
```csharp
public class FirestoreMovieRepository : IMovieRepository
{
    private readonly FirestoreDb _db;
    private const string CollectionName = "movies";

    public FirestoreMovieRepository(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Movie>> ListAsync(CancellationToken ct)
    {
        var snapshot = await _db.Collection(CollectionName).GetSnapshotAsync(ct);
        return snapshot.Documents
            .Select(doc => doc.ConvertTo<Movie>())
            .ToList();
    }

    // ⚠️ TODO: Implementar métodos restantes
}
```

**Ventajas:**
- Mapeo automático con `ConvertTo<T>()`
- Queries tipadas
- Soporte de `CancellationToken`

---

#### Implementaciones In-Memory (Cinema.Infrastructure)

**InMemoryMovieRepository.cs:**
```csharp
public class InMemoryMovieRepository : IMovieRepository
{
    private static readonly List<Movie> _movies = new()
    {
        new Movie { Id = "1", Title = "Inception", ... },
        new Movie { Id = "2", Title = "Interstellar", ... },
        new Movie { Id = "3", Title = "Dune", ... }
    };

    public Task<IReadOnlyList<Movie>> ListAsync(CancellationToken ct)
    {
        return Task.FromResult<IReadOnlyList<Movie>>(_movies);
    }
}
```

**Uso:**
- Desarrollo local sin Firebase
- Tests unitarios
- Demos rápidas

---

### Estrategia de Selección de Repositorio

**DependencyInjection.cs:**
```csharp
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
    var firebaseEnabled = configuration.GetValue<bool>("Firebase:Enabled");

    if (firebaseEnabled)
    {
        // Configurar Firebase
        var serviceAccountPath = configuration["Firebase:ServiceAccountPath"];
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);

        services.AddSingleton(FirestoreDb.Create(projectId));
        services.AddScoped<IMovieRepository, FirestoreMovieRepository>();
        services.AddScoped<IUserRepository, FirestoreUserRepository>();
    }
    else
    {
        // Fallback a in-memory
        services.AddScoped<IMovieRepository, InMemoryMovieRepository>();
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
    }

    return services;
}
```

**Configuración en appsettings.Development.json:**
```json
{
  "Firebase": {
    "Enabled": false
  }
}
```

---

## Servicios

### UserService.cs (Firebase Authentication Integration)

**Ubicación:** `Cinema.Api/Services/UserService.cs`

**Responsabilidad:** Gestión de usuarios en Firebase Authentication

```csharp
public class UserService
{
    private readonly string _apiKey;
    private readonly ConcurrentDictionary<string, string> _userRoles;

    public UserService(IConfiguration configuration)
    {
        _apiKey = configuration["Firebase:apiKey"];
        _userRoles = new ConcurrentDictionary<string, string>();
    }

    // Verificar password usando Firebase REST API
    public async Task<bool> VerifyUserPasswordAsync(string email, string password);

    // Crear usuario en Firebase Auth
    public async Task<UserRecord> CreateUserAsync(string email, string password, string displayName, string role);

    // Obtener usuario por UID
    public async Task<UserRecord> GetUserAsync(string uid);

    // Obtener usuario por email
    public async Task<UserRecord> GetUserByEmailAsync(string email);

    // Eliminar usuario
    public async Task DeleteUserAsync(string uid);

    // Actualizar usuario
    public async Task<UserRecord> UpdateUserAsync(string uid, UserRecordArgs args);

    // Listar todos los usuarios
    public async Task<List<ExportedUserRecord>> ListUsersAsync();
}
```

**⚠️ Problema Conocido:** Los roles se almacenan en memoria (`ConcurrentDictionary`), se pierden al reiniciar. **Solución:** Almacenar roles en Firestore o usar Custom Claims de Firebase.

---

### FirestoreUserService.cs (Firestore User Operations)

**Ubicación:** `Cinema.Api/Services/FirestoreUserService.cs`

**Responsabilidad:** Operaciones CRUD de usuarios en Firestore

```csharp
public class FirestoreUserService
{
    private readonly FirestoreDb _firestoreDb;
    private const string CollectionName = "users";

    public async Task<string> AddUserAsync(User user);
    public async Task<User?> GetUserAsync(string uid);
    public async Task<List<User>> GetAllUsersAsync();
    public async Task<bool> UpdateUserAsync(string uid, User updatedUser);
    public async Task<bool> DeleteUserAsync(string uid);
    public async Task<User?> GetUserByEmailAsync(string email);
    public async Task<bool> VerifyUserPasswordAsync(string email, string password);
}
```

**⚠️ Problema de Seguridad:** `VerifyUserPasswordAsync` compara passwords en texto plano. **Solución:** Implementar bcrypt o usar Firebase Auth exclusivamente.

---

## Controllers

### AuthController.cs

**Ruta base:** `/api/`

```csharp
[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst("user_id")?.Value
                  ?? User.FindFirst("uid")?.Value;
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("role")?.Value;

        return Ok(new { userId, email, role });
    }
}
```

**Claims esperados en JWT:**
- `user_id` o `uid`: ID del usuario
- `email`: Email del usuario
- `role`: Rol (admin, user)

---

### MoviesController.cs

**Ruta base:** `/api/movies`

```csharp
[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetMovies(
        [FromServices] IMovieRepository movieRepository,
        CancellationToken ct)
    {
        var movies = await movieRepository.ListAsync(ct);
        return Ok(movies);
    }

    // ⚠️ TODO: Implementar endpoints restantes
    [HttpPost("add-movie")]
    [Authorize]
    public IActionResult AddMovie() => Ok();

    [HttpGet("get-movie/{id}")]
    [Authorize]
    public IActionResult GetMovie(string id) => Ok();

    [HttpPut("edit-movie/{id}")]
    [Authorize]
    public IActionResult EditMovie(string id) => Ok();

    [HttpDelete("delete-movie/{id}")]
    [Authorize]
    public IActionResult DeleteMovie(string id) => Ok();
}
```

---

### FirebaseTestController.cs

**Ruta base:** `/api/FirebaseTest`

**Endpoints de Login y User Management:**

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
{
    // 1. Verificar password con Firebase
    bool isValid = await _userService.VerifyUserPasswordAsync(email, password);

    // 2. Obtener usuario de Firebase Auth
    UserRecord firebaseUser = await _userService.GetUserByEmailAsync(email);

    // 3. Generar JWT token
    var token = GenerateJwtToken(firebaseUser.Uid, email, displayName, role);

    // 4. Retornar respuesta
    return Ok(new {
        success = true,
        uid = firebaseUser.Uid,
        email,
        displayName,
        role,
        token
    });
}

[HttpGet("get-all-users")]
[FeatureGate("DatabaseReadAll")]
public async Task<IActionResult> GetAllUsers()
{
    var users = await _firestoreUserService.GetAllUsersAsync();
    return Ok(users);
}
```

**JWT Token Generation:**
```csharp
private string GenerateJwtToken(string uid, string email, string displayName, string role)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, uid),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim("uid", uid),
        new Claim("role", role),
        new Claim("displayName", displayName)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "CinemaApi",
        audience: "CinemaApiUsers",
        claims: claims,
        expires: DateTime.Now.AddMinutes(60),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

---

## Autenticación y Autorización

### Configuración en Program.cs

```csharp
var firebaseEnabled = builder.Configuration.GetValue<bool>("Firebase:Enabled");

if (firebaseEnabled)
{
    var projectId = builder.Configuration["Firebase:ProjectId"];

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://securetoken.google.com/{projectId}";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://securetoken.google.com/{projectId}",
                ValidateAudience = true,
                ValidAudience = projectId,
                ValidateLifetime = true
            };
        });
}
else
{
    // Custom JWT para desarrollo local
    var jwtKey = builder.Configuration["Jwt:Key"];
    var key = Encoding.UTF8.GetBytes(jwtKey);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "CinemaApi",
                ValidateAudience = true,
                ValidAudience = "CinemaApiUsers",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true
            };
        });
}

builder.Services.AddAuthorization();
```

---

### Uso en Controllers

```csharp
// Requiere autenticación (cualquier usuario logueado)
[Authorize]
[HttpGet("protected-endpoint")]
public IActionResult ProtectedEndpoint() { }

// Requiere rol específico
[Authorize(Roles = "admin")]
[HttpGet("admin-only")]
public IActionResult AdminOnly() { }

// Endpoint público
[AllowAnonymous]
[HttpGet("public")]
public IActionResult PublicEndpoint() { }
```

---

## Integración con Firebase

### Configuración

**appsettings.json:**
```json
{
  "Firebase": {
    "ConfigPath": "Config/magiacinema-adminsdk.json",
    "apiKey": "YOUR_FIREBASE_API_KEY",
    "Enabled": true,
    "ProjectId": "magiacinema"
  }
}
```

**Service Account JSON:**
- Ubicación: `Config/magiacinema-adminsdk.json`
- Generado desde Firebase Console → Project Settings → Service Accounts
- **⚠️ NO COMMITEAR** este archivo (agregarlo a `.gitignore`)

---

### Inicialización de Firebase Admin SDK

**Program.cs:**
```csharp
var configPath = builder.Configuration["Firebase:ConfigPath"];
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
FirebaseApp.Create(new AppOptions());
```

---

### Inicialización de Firestore

**DependencyInjection.cs:**
```csharp
var projectId = configuration["Firebase:ProjectId"];
var db = FirestoreDb.Create(projectId);
services.AddSingleton(db);
```

---

## Logging y Auditoría

### Serilog Configuration

**appsettings.json:**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/cinema-api-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

**Program.cs:**
```csharp
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});
```

---

### User Action Audit Middleware

**UserActionAuditMiddleware.cs:**
```csharp
public class UserActionAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserActionAuditMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        var userId = context.User.FindFirst("user_id")?.Value ?? "Anonymous";
        var ip = context.Connection.RemoteIpAddress?.ToString();

        await _next(context);

        var statusCode = context.Response.StatusCode;

        _logger.LogInformation(
            "User {UserId} from {IP} - {Method} {Path} - Status: {StatusCode}",
            userId, ip, method, path, statusCode
        );
    }
}
```

**Registro en Program.cs:**
```csharp
app.UseMiddleware<UserActionAuditMiddleware>();
```

**Logs generados:**
```
[2025-11-03 12:34:56] User abc123 from 192.168.1.100 - POST /api/movies/add-movie - Status: 201
[2025-11-03 12:35:10] User Anonymous from 192.168.1.101 - GET /api/movies - Status: 200
```

---

## Feature Flags

### Microsoft.FeatureManagement

**appsettings.json:**
```json
{
  "FeatureManagement": {
    "DatabaseConnection": true,
    "DatabaseReadAll": true
  }
}
```

**Program.cs:**
```csharp
builder.Services.AddFeatureManagement();
```

**Uso en Controllers:**
```csharp
[HttpGet("test-connection")]
[FeatureGate("DatabaseConnection")]
public async Task<IActionResult> TestFirestoreConnection()
{
    // Solo se ejecuta si DatabaseConnection = true
}
```

**Verificación programática:**
```csharp
private readonly IFeatureManager _featureManager;

if (await _featureManager.IsEnabledAsync("DatabaseConnection"))
{
    // Lógica condicional
}
```

---

## Configuración

### CORS

**Program.cs:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterClient", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:4200",
            "http://localhost:5173",
            "http://localhost:5500",
            "http://127.0.0.1:5500"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

app.UseCors("AllowFlutterClient");
```

---

### Swagger/OpenAPI

**Program.cs:**
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

**Acceso:** `https://localhost:7238/swagger`

---

## Dependency Injection

### Registro de Servicios

**Program.cs:**
```csharp
// Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FirestoreUserService>();

// Feature management
builder.Services.AddFeatureManagement();

// Logging
builder.Services.AddLogging();
```

**DependencyInjection.cs:**
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var firebaseEnabled = configuration.GetValue<bool>("Firebase:Enabled");

        if (firebaseEnabled)
        {
            // Configurar Firebase y repositorios Firestore
        }
        else
        {
            // Configurar repositorios In-Memory
        }

        return services;
    }
}
```

---

## Mejores Prácticas

### 1. Separación de Responsabilidades
- **Controllers:** Solo reciben requests y retornan responses
- **Services:** Lógica de negocio y coordinación
- **Repositories:** Acceso a datos

### 2. Async/Await
- Todos los métodos I/O son asíncronos
- Usar `CancellationToken` para operaciones cancelables

### 3. Error Handling
```csharp
try
{
    var result = await _repository.GetByIdAsync(id, ct);
    if (result == null)
        return NotFound();
    return Ok(result);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error getting movie {Id}", id);
    return StatusCode(500, "Internal server error");
}
```

### 4. Validación
- Usar Data Annotations en DTOs
- Validar en Controllers antes de procesar
- Retornar `BadRequest` con mensajes claros

### 5. Seguridad
- **NUNCA** almacenar passwords en texto plano
- Siempre validar autorización en endpoints críticos
- Sanitizar inputs para prevenir injection

### 6. Testing
- Unit tests para lógica de negocio
- Integration tests con Firestore Emulator
- Mocking de dependencias externas

---

## Próximos Pasos

1. ✅ Completar CRUD de Movies, Screenings, TheaterRooms, FoodCombos
2. ✅ Implementar Bookings & Seats
3. ✅ Agregar validaciones robustas
4. ✅ Implementar tests unitarios
5. ✅ Hashear passwords (bcrypt)
6. ✅ Almacenar roles en Firestore o Custom Claims
7. ✅ Implementar refresh tokens
8. ✅ Agregar rate limiting
9. ✅ Dockerizar aplicación
10. ✅ CI/CD pipeline

---

**Mantenido por:** Equipo de Desarrollo Cinema System
**Última revisión:** 2025-11-03
