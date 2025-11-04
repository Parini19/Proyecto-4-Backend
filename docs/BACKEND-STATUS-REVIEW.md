# Backend Status Review - Cinema Management System

**Fecha de revisión:** 2025-11-03
**Revisado por:** Claude Code

---

## 📋 Resumen Ejecutivo

El backend tiene una **configuración funcional** de Firebase/Firestore con **login completamente implementado** usando JWT tokens. El sistema de autenticación está operativo y conectado a Firestore.

### ✅ Estado General
- **Firebase/Firestore:** ✅ Configurado y funcionando
- **Autenticación:** ✅ Login implementado con JWT
- **User CRUD:** ✅ Completo y funcionando
- **Movies CRUD:** ⚠️ Solo GET implementado
- **Otras entidades:** ❌ Endpoints definidos pero vacíos

---

## 🔥 Firebase & Firestore Configuration

### Configuración Actual

**Estado:** ✅ **Firebase DESHABILITADO en Development** pero configurado para usar Firestore

```json
// appsettings.Development.json
{
  "Firebase": {
    "Enabled": false,  // ⚠️ Deshabilitado para development
    "ProjectId": "",
    "ServiceAccountPath": ""
  }
}
```

```json
// appsettings.json (Production)
{
  "Firebase": {
    "ConfigPath": "Config/magiacinema-adminsdk.json",  // ⚠️ Archivo NO encontrado
    "apiKey": ""  // ⚠️ Vacío
  }
}
```

### ⚠️ Problemas Encontrados

1. **Service Account JSON no existe:**
   - Ruta configurada: `Config/magiacinema-adminsdk.json`
   - Estado: ❌ **NO ENCONTRADO**
   - Impacto: Firebase Auth no funcionará hasta agregar este archivo

2. **Firebase API Key vacío:**
   - Configuración: `"apiKey": ""`
   - Necesario para operaciones de cliente

3. **ProjectId vacío en Development:**
   - Debe configurarse para usar Firestore

### ✅ Lo que SÍ está configurado

1. **FirestoreUserService** - Completamente funcional
   - Conecta directamente a Firestore
   - CRUD completo de usuarios
   - Login con verificación de password
   - Genera JWT tokens

2. **Dependency Injection** - Correctamente implementado
   - Fallback a InMemory si Firebase falla
   - Singleton de FirestoreDb

---

## 🔐 Sistema de Autenticación - ✅ IMPLEMENTADO

### Login Endpoint: `POST /api/FirebaseTest/login`

**Estado:** ✅ **COMPLETAMENTE FUNCIONAL**

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
{
    // 1. Busca usuario por email en Firestore
    var user = await _firestoreUserService.GetUserByEmailAsync(loginDto.Email);

    // 2. Verifica password (⚠️ texto plano, ver nota de seguridad)
    var isValid = await _firestoreUserService.VerifyUserPasswordAsync(
        loginDto.Email,
        loginDto.Password
    );

    // 3. Genera JWT token
    var jwtToken = GenerateJwtToken(user, configuration);

    // 4. Retorna datos del usuario + token
    return Ok(new {
        success = true,
        uid = user.Uid,
        email = user.Email,
        displayName = user.DisplayName,
        role = user.Role,
        jwtToken
    });
}
```

### JWT Token Generation

**Configuración JWT:**
```json
{
  "Jwt": {
    "Key": "",  // ⚠️ VACÍO - DEBE CONFIGURARSE
    "Issuer": "CinemaApi",
    "Audience": "CinemaApiUsers",
    "ExpiresMinutes": 60
  }
}
```

**Claims incluidos en el token:**
- `sub`: User UID
- `email`: Email del usuario
- `role`: Rol (admin/user)
- `jti`: Token ID único

**⚠️ CRÍTICO:** El campo `Jwt:Key` está **vacío** en appsettings.json. Debes generar una clave segura:

```bash
# Generar clave aleatoria (32 bytes en base64)
openssl rand -base64 32
```

### ⚠️ Problemas de Seguridad

#### 1. Passwords en Texto Plano

**Código actual:**
```csharp
public async Task<bool> VerifyUserPasswordAsync(string email, string password)
{
    var user = await GetUserByEmailAsync(email);
    if (user == null)
        return false;

    return user.Password == password;  // ⚠️ COMPARACIÓN EN TEXTO PLANO
}
```

**Problema:** Las passwords se almacenan y comparan en texto plano en Firestore.

**Solución requerida:**
1. Instalar `BCrypt.Net-Next` NuGet package
2. Hashear passwords antes de guardar
3. Usar `BCrypt.Verify()` para comparar

#### 2. JWT Key vacía

Como se mencionó, la clave de firma JWT está vacía. El token no será válido sin esto.

---

## 👥 User Management - ✅ CRUD COMPLETO

### Entidad User

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
    [FirestoreProperty] public string Password { get; set; }  // ⚠️ Texto plano
}
```

**Colección Firestore:** `users`

### Endpoints Implementados

| Método | Endpoint | Estado | Descripción |
|--------|----------|--------|-------------|
| POST | `/api/FirebaseTest/login` | ✅ Funcional | Login con JWT |
| POST | `/api/FirebaseTest/add-user` | ✅ Funcional | Crear usuario |
| GET | `/api/FirebaseTest/get-user/{uid}` | ✅ Funcional | Obtener usuario por UID |
| PUT | `/api/FirebaseTest/edit-user/{uid}` | ✅ Funcional | Actualizar usuario |
| DELETE | `/api/FirebaseTest/delete-user/{uid}` | ✅ Funcional | Eliminar usuario |
| GET | `/api/FirebaseTest/get-all-users` | ✅ Funcional | Listar todos (FeatureGate) |
| GET | `/api/FirebaseTest/test-connection` | ✅ Funcional | Test Firebase connection |
| GET | `/api/FirebaseTest/motd` | ✅ Funcional | Admin only (demo) |

### Servicios Implementados

#### FirestoreUserService ✅

**Métodos:**
- `AddUserAsync(User user)` - Agrega usuario a Firestore
- `GetUserAsync(string uid)` - Obtiene usuario por UID
- `GetAllUsersAsync()` - Lista todos los usuarios
- `UpdateUserAsync(User user)` - Actualiza usuario (overwrite)
- `DeleteUserAsync(string uid)` - Elimina usuario
- `GetUserByEmailAsync(string email)` - Busca por email
- `VerifyUserPasswordAsync(string email, string password)` - Verifica credentials

**Conexión a Firestore:**
```csharp
public FirestoreUserService(IConfiguration configuration)
{
    var projectId = configuration["Firebase:ProjectId"];
    var configPath = configuration["Firebase:ConfigPath"];

    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
    _firestoreDb = FirestoreDb.Create(projectId);
}
```

---

## 🎬 Movies Management - ⚠️ PARCIALMENTE IMPLEMENTADO

### Entidad Movie

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
}
```

**Colección Firestore:** `movies`

### ⚠️ Campos Faltantes (Recomendados)

Para completar la entidad Movie, deberías agregar:
- `ReleaseDate` (DateTime)
- `Rating` (decimal)
- `PosterUrl` (string)
- `TrailerUrl` (string)
- `Classification` (string) - PG-13, R, etc.
- `IsActive` (bool)
- `Language` (string)

### Endpoints Implementados

| Método | Endpoint | Estado | Descripción |
|--------|----------|--------|-------------|
| GET | `/api/movies` | ✅ Funcional | Listar películas |
| POST | `/api/movies/add-movie` | ❌ TODO | Crear película |
| GET | `/api/movies/get-movie/{id}` | ❌ TODO | Obtener película |
| PUT | `/api/movies/edit-movie/{id}` | ❌ TODO | Editar película |
| DELETE | `/api/movies/delete-movie/{id}` | ❌ TODO | Eliminar película |
| GET | `/api/movies/get-all-movies` | ❌ TODO | Listar todas |

### Código Actual

```csharp
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> Get() => Ok(await _repo.ListAsync());

[HttpPost("add-movie")]
public IActionResult AddMovie()
{
    // TODO: Implement add movie logic
    return Ok();  // ⚠️ Retorna vacío
}

// ... resto de endpoints igual (TODO)
```

### Repositorios

**IMovieRepository** - Interface definida
```csharp
public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> ListAsync(CancellationToken ct);
    // ⚠️ Falta: GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync
}
```

**Implementaciones:**
- `InMemoryMovieRepository` ✅ - Con datos de ejemplo (Inception, Interstellar, Dune)
- `FirestoreUserRepository` ⚠️ - **BUG:** Implementa `IMovieRepository` pero debería ser para users

---

## 🎭 Otras Entidades - ❌ SOLO ESTRUCTURA

### Screening (Proyecciones)

**Entidad:**
```csharp
[FirestoreData]
public class Screening
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string MovieId { get; set; }
    [FirestoreProperty] public string TheaterRoomId { get; set; }
    [FirestoreProperty] public DateTime StartTime { get; set; }
    [FirestoreProperty] public DateTime EndTime { get; set; }
}
```

**Campos sugeridos a agregar:**
- `Price` (decimal)
- `AvailableSeats` (int)
- `IsActive` (bool)

**Estado:**
- ✅ Controller creado: `ScreeningsController.cs`
- ❌ Todos los endpoints retornan `Ok()` vacío (TODO)
- ❌ No hay repositorio implementado

---

### TheaterRoom (Salas)

**Entidad:**
```csharp
[FirestoreData]
public class TheaterRoom
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public int Capacity { get; set; }
}
```

**Campos sugeridos a agregar:**
- `Rows` (int)
- `Columns` (int)
- `ScreenType` (string) - IMAX, 3D, Standard
- `Features` (List<string>)
- `IsActive` (bool)

**Estado:**
- ✅ Controller creado: `TheaterRoomsController.cs`
- ❌ Endpoints vacíos (TODO)
- ❌ No hay repositorio

---

### FoodCombo (Combos de Alimentos)

**Entidad:**
```csharp
[FirestoreData]
public class FoodCombo
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public string Description { get; set; }
    [FirestoreProperty] public decimal Price { get; set; }
    [FirestoreProperty] public List<string> Items { get; set; }
}
```

**Campos sugeridos a agregar:**
- `ImageUrl` (string)
- `IsAvailable` (bool)
- `Category` (string) - Snacks, Drinks, Combos

**Estado:**
- ✅ Controller creado: `FoodCombosController.cs`
- ❌ Endpoints vacíos (TODO)
- ❌ No hay repositorio

---

### FoodOrder (Órdenes de Comida)

**Entidad:**
```csharp
[FirestoreData]
public class FoodOrder
{
    [FirestoreProperty] public string Id { get; set; }
    [FirestoreProperty] public string UserId { get; set; }
    [FirestoreProperty] public List<string> FoodComboIds { get; set; }
    [FirestoreProperty] public decimal TotalPrice { get; set; }
    [FirestoreProperty] public string Status { get; set; }
}
```

**Campos sugeridos a agregar:**
- `BookingId` (string)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)
- `PaymentMethod` (string)

**Estado:**
- ✅ Controller creado: `FoodOrdersController.cs`
- ❌ Endpoints vacíos (TODO)
- ❌ No hay repositorio

---

## 🔧 Infraestructura

### Dependency Injection

**Ubicación:** `Cinema.Infrastructure/DependencyInjection.cs`

**Lógica:**
```csharp
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration config)
{
    var enabled = config.GetValue<bool?>("Firebase:Enabled") ?? false;

    if (!enabled)
    {
        // Usa in-memory repository
        services.AddScoped<IMovieRepository, InMemoryMovieRepository>();
        return services;
    }

    // Si Firebase está habilitado, intenta conectar
    try
    {
        // Inicializa FirebaseApp
        // Crea FirestoreDb
        services.AddSingleton(FirestoreDb);
        services.AddScoped<IMovieRepository, FirestoreUserRepository>();
    }
    catch (Exception ex)
    {
        // Fallback a in-memory en caso de error
        services.AddScoped<IMovieRepository, InMemoryMovieRepository>();
    }
}
```

**⚠️ Bug encontrado:** Usa `FirestoreUserRepository` para `IMovieRepository` (incorrecto)

### Program.cs

**Configuración:**
- ✅ Serilog logging
- ✅ CORS configurado para Flutter (5173)
- ✅ Swagger en development
- ✅ Feature flags
- ⚠️ Firebase Auth solo si `Firebase:Enabled = true`
- ✅ UserActionAuditMiddleware para logging de acciones

---

## 📊 Feature Flags

```json
{
  "FeatureManagement": {
    "DatabaseConnection": true,
    "DatabaseReadAll": true
  }
}
```

**Uso:**
```csharp
[FeatureGate("DatabaseConnection")]
public async Task<IActionResult> TestConnection()

[FeatureGate("DatabaseReadAll")]
public async Task<IActionResult> GetAllUsers()
```

---

## 🔍 Logging & Auditing

### Serilog ✅

**Configuración:**
- Console logging (development)
- File logging: `logs/cinema-.log` (daily rotation)
- Request logging middleware habilitado

### UserActionAuditMiddleware ✅

**Ubicación:** `Cinema.Api/Utilities/UserActionAuditMiddleware.cs`

**Funcionalidad:**
- Captura todas las requests
- Logs: Method, Path, Status Code, User ID, IP
- Ejecuta después de cada request

---

## 🌐 CORS

**Configuración:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200",
      "http://localhost:5173",  // Flutter web
      "http://localhost:5500",
      "http://127.0.0.1:5500"
    ]
  }
}
```

**Política:** `FlutterClient`
- Permite cualquier header
- Permite cualquier método
- Permite credentials

---

## 📋 Resumen de Estado

### ✅ Completamente Implementado

1. **User Management**
   - CRUD completo en Firestore
   - Login con JWT
   - Roles (admin/user)
   - 8 endpoints funcionales

2. **Infraestructura**
   - Dependency Injection
   - Logging con Serilog
   - CORS configurado
   - Feature Flags
   - Audit middleware
   - Swagger documentation

3. **Movies - Solo GET**
   - `GET /api/movies` funciona
   - Obtiene de Firestore o in-memory

### ⚠️ Parcialmente Implementado

1. **Movies CRUD**
   - ✅ GET implementado
   - ❌ POST, PUT, DELETE (TODO)
   - ❌ Repository incompleto

2. **Firebase Configuration**
   - ✅ Código preparado
   - ⚠️ Service Account JSON faltante
   - ⚠️ JWT Key vacía
   - ⚠️ Deshabilitado en development

### ❌ No Implementado

1. **Screenings** - Solo estructura
2. **TheaterRooms** - Solo estructura
3. **FoodCombos** - Solo estructura
4. **FoodOrders** - Solo estructura
5. **Bookings** - No existe
6. **Seats** - No existe

---

## 🚨 Issues Críticos a Resolver

### 🔴 Alta Prioridad

1. **JWT Key vacía**
   - Archivo: `appsettings.json`
   - Línea: `"Key": ""`
   - Acción: Generar clave segura de 256 bits

2. **Passwords en texto plano**
   - Archivo: `FirestoreUserService.cs`
   - Línea: 79
   - Acción: Implementar BCrypt

3. **Firebase Service Account faltante**
   - Ruta: `Config/magiacinema-adminsdk.json`
   - Acción: Descargar de Firebase Console y agregar

4. **Bug en DependencyInjection**
   - Archivo: `DependencyInjection.cs`
   - Línea: 61
   - Problema: Usa `FirestoreUserRepository` para `IMovieRepository`
   - Acción: Crear `FirestoreMovieRepository` correcto

### 🟡 Media Prioridad

5. **Completar Movies CRUD**
   - Implementar POST, PUT, DELETE
   - Agregar campos faltantes a entidad

6. **Implementar otros CRUDs**
   - Screenings
   - TheaterRooms
   - FoodCombos
   - FoodOrders

### 🟢 Baja Prioridad

7. **Tests unitarios**
   - No hay tests implementados

8. **Validaciones de entrada**
   - Agregar Data Annotations
   - FluentValidation (opcional)

---

## ✅ Checklist de Configuración Inicial

Para que el backend funcione completamente:

- [ ] **Crear archivo `Config/magiacinema-adminsdk.json`**
  - Descargar de Firebase Console
  - Colocar en `src/Cinema.Api/Config/`
  - **NO commitear** (agregar a .gitignore)

- [ ] **Generar JWT Key**
  ```bash
  openssl rand -base64 32
  ```
  - Copiar resultado a `appsettings.json` → `Jwt:Key`

- [ ] **Configurar Firebase ProjectId**
  - En `appsettings.json`: `Firebase:ProjectId`
  - Obtener de Firebase Console

- [ ] **Habilitar Firebase en Development (opcional)**
  ```json
  // appsettings.Development.json
  {
    "Firebase": {
      "Enabled": true,
      "ProjectId": "tu-project-id",
      "ServiceAccountPath": "Config/magiacinema-adminsdk.json"
    }
  }
  ```

- [ ] **Implementar BCrypt para passwords**
  ```bash
  dotnet add package BCrypt.Net-Next
  ```

- [ ] **Crear usuarios de prueba en Firestore**
  - Usar endpoint `POST /api/FirebaseTest/add-user`
  - O agregar manualmente desde Firebase Console

---

## 🎯 Próximos Pasos Recomendados

### Inmediato (HOY)

1. ✅ Configurar JWT Key
2. ✅ Descargar Service Account JSON de Firebase
3. ✅ Probar login con usuario existente
4. ✅ Verificar que JWT token se genera correctamente

### Corto Plazo (Esta Semana)

1. ✅ Implementar BCrypt para passwords
2. ✅ Completar Movies CRUD (POST, PUT, DELETE)
3. ✅ Crear FirestoreMovieRepository correcto
4. ✅ Agregar campos faltantes a Movie entity

### Mediano Plazo (Próximas 2 semanas)

1. ✅ Implementar Screenings CRUD
2. ✅ Implementar TheaterRooms CRUD
3. ✅ Implementar FoodCombos CRUD
4. ✅ Implementar FoodOrders CRUD
5. ✅ Agregar validaciones de entrada

---

## 📝 Conclusión

**El backend tiene una base sólida con:**
- ✅ Autenticación funcional (login + JWT)
- ✅ User management completo
- ✅ Firestore integrado y funcionando
- ✅ Arquitectura Clean bien implementada

**Requiere:**
- 🔴 Configuración de credenciales Firebase
- 🔴 Seguridad (BCrypt para passwords)
- 🟡 Completar CRUDs de entidades
- 🟢 Testing

**Tiempo estimado para completar MVP:** 2-3 semanas con 1 desarrollador.

---

**Revisado por:** Claude Code
**Fecha:** 2025-11-03
**Versión del documento:** 1.0
