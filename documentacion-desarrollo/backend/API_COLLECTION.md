# 📡 Colección de APIs - Cinema Backend

## Información General

- **Base URL:** `http://localhost:5000`
- **Versión:** 1.0
- **Autenticación:** JWT Bearer Token (para endpoints protegidos)

---

## 📁 Tabla de Contenidos

1. [Health & Status](#health--status)
2. [Authentication](#authentication)
3. [Movies](#movies)
4. [Screenings](#screenings)
5. [Users](#users)

---

## Health & Status

### 1. Health Check

Verifica que el servidor está corriendo.

**Endpoint:** `GET /health`
**Auth:** No requerida
**Status esperado:** 200 OK

```
GET http://localhost:5000/health
```

**Response:**
```json
{
  "status": "ok",
  "time": "2025-11-04T15:30:00.000Z"
}
```

---

## Authentication

### 2. Registrar Usuario

Crea un nuevo usuario en el sistema.

**Endpoint:** `POST /api/FirebaseTest/add-user`
**Auth:** No requerida
**Status esperado:** 200 OK

```
POST http://localhost:5000/api/FirebaseTest/add-user
Content-Type: application/json

{
  "email": "usuario@test.com",
  "password": "123456",
  "displayName": "Usuario de Prueba",
  "role": "user"
}
```

**Response:**
```json
{
  "success": true,
  "uid": "generated-uuid-here"
}
```

**Notas:**
- La contraseña se guardará hasheada con BCrypt
- El UID se genera automáticamente si no se proporciona
- Role por defecto: "user"

---

### 3. Login

Inicia sesión y obtiene un JWT token.

**Endpoint:** `POST /api/FirebaseTest/login`
**Auth:** No requerida
**Status esperado:** 200 OK

```
POST http://localhost:5000/api/FirebaseTest/login
Content-Type: application/json

{
  "email": "usuario@test.com",
  "password": "123456"
}
```

**Response:**
```json
{
  "success": true,
  "uid": "user-uid",
  "email": "usuario@test.com",
  "displayName": "Usuario de Prueba",
  "role": "user",
  "jwtToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**⚠️ IMPORTANTE:** Guarda el `jwtToken` para usarlo en requests protegidos.

---

### 4. Login con Credenciales Incorrectas (Test Negativo)

**Endpoint:** `POST /api/FirebaseTest/login`
**Auth:** No requerida
**Status esperado:** 401 Unauthorized

```
POST http://localhost:5000/api/FirebaseTest/login
Content-Type: application/json

{
  "email": "usuario@test.com",
  "password": "passwordincorrecto"
}
```

**Response:**
```json
{
  "success": false,
  "message": "Invalid credentials."
}
```

---

## Movies

### 5. Listar Todas las Películas

Obtiene todas las películas disponibles (InMemory: 3 películas).

**Endpoint:** `GET /api/movies`
**Auth:** No requerida (AllowAnonymous)
**Status esperado:** 200 OK

```
GET http://localhost:5000/api/movies
```

**Response:**
```json
[
  {
    "id": "1",
    "title": "Inception",
    "description": "A thief who steals corporate secrets",
    "durationMinutes": 148,
    "genre": "Science Fiction",
    "director": "Christopher Nolan",
    "year": 2010
  },
  {
    "id": "2",
    "title": "Interstellar",
    "description": "A team of explorers travel through a wormhole",
    "durationMinutes": 169,
    "genre": "Science Fiction",
    "director": "Christopher Nolan",
    "year": 2014
  },
  {
    "id": "3",
    "title": "Dune",
    "description": "Paul Atreides arrives on the dangerous planet Arrakis",
    "durationMinutes": 155,
    "genre": "Science Fiction",
    "director": "Denis Villeneuve",
    "year": 2021
  }
]
```

---

### 6. Obtener Película por ID

Obtiene los detalles de una película específica.

**Endpoint:** `GET /api/movies/{id}`
**Auth:** No requerida (AllowAnonymous)
**Status esperado:** 200 OK

```
GET http://localhost:5000/api/movies/1
```

**Response:**
```json
{
  "id": "1",
  "title": "Inception",
  "description": "A thief who steals corporate secrets",
  "durationMinutes": 148,
  "genre": "Science Fiction",
  "director": "Christopher Nolan",
  "year": 2010
}
```

**Test Negativo - ID Inexistente:**
```
GET http://localhost:5000/api/movies/999
```

**Response (404):**
```json
{
  "message": "Movie with id '999' not found"
}
```

---

### 7. Crear Película (Requiere Auth)

Crea una nueva película.

**Endpoint:** `POST /api/movies`
**Auth:** ✅ Requerida (Bearer Token)
**Status esperado:** 201 Created

```
POST http://localhost:5000/api/movies
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "title": "The Matrix",
  "description": "A computer hacker learns about the true nature of reality",
  "durationMinutes": 136,
  "genre": "Science Fiction / Action",
  "director": "The Wachowskis",
  "year": 1999
}
```

**Response:**
```json
{
  "id": "4",
  "title": "The Matrix",
  "description": "A computer hacker learns about the true nature of reality",
  "durationMinutes": 136,
  "genre": "Science Fiction / Action",
  "director": "The Wachowskis",
  "year": 1999
}
```

**Test Negativo - Sin Token:**
```
POST http://localhost:5000/api/movies
Content-Type: application/json

{
  "title": "Test Movie",
  "durationMinutes": 120
}
```

**Response (401):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

---

### 8. Actualizar Película (Requiere Auth)

Actualiza una película existente.

**Endpoint:** `PUT /api/movies/{id}`
**Auth:** ✅ Requerida (Bearer Token)
**Status esperado:** 200 OK

```
PUT http://localhost:5000/api/movies/4
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "title": "The Matrix Reloaded",
  "description": "Neo and the rebel leaders estimate they have 72 hours",
  "durationMinutes": 138,
  "genre": "Science Fiction / Action",
  "director": "The Wachowskis",
  "year": 2003
}
```

**Response:**
```json
{
  "id": "4",
  "title": "The Matrix Reloaded",
  "description": "Neo and the rebel leaders estimate they have 72 hours",
  "durationMinutes": 138,
  "genre": "Science Fiction / Action",
  "director": "The Wachowskis",
  "year": 2003
}
```

---

### 9. Eliminar Película (Requiere Auth)

Elimina una película del sistema.

**Endpoint:** `DELETE /api/movies/{id}`
**Auth:** ✅ Requerida (Bearer Token)
**Status esperado:** 204 No Content

```
DELETE http://localhost:5000/api/movies/4
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:**
- No body
- Status: 204 No Content

**Verificación:**
```
GET http://localhost:5000/api/movies/4
```
Debería retornar 404 Not Found

---

## Screenings

### 10. Listar Todos los Screenings

Obtiene todas las funciones de cine.

**Endpoint:** `GET /api/screenings`
**Auth:** No requerida (AllowAnonymous)
**Status esperado:** 200 OK

```
GET http://localhost:5000/api/screenings
```

**Response:**
```json
[
  {
    "id": "1",
    "movieId": "1",
    "theaterRoomId": "1",
    "startTime": "2025-11-04T14:00:00Z",
    "endTime": "2025-11-04T16:30:00Z"
  },
  {
    "id": "2",
    "movieId": "2",
    "theaterRoomId": "1",
    "startTime": "2025-11-04T17:00:00Z",
    "endTime": "2025-11-04T20:00:00Z"
  }
]
```

---

### 11. Obtener Screening por ID

Obtiene los detalles de un screening específico.

**Endpoint:** `GET /api/screenings/{id}`
**Auth:** No requerida (AllowAnonymous)
**Status esperado:** 200 OK

```
GET http://localhost:5000/api/screenings/1
```

**Response:**
```json
{
  "id": "1",
  "movieId": "1",
  "theaterRoomId": "1",
  "startTime": "2025-11-04T14:00:00Z",
  "endTime": "2025-11-04T16:30:00Z"
}
```

---

### 12. Obtener Screenings por Película

Obtiene todas las funciones de una película específica.

**Endpoint:** `GET /api/screenings/by-movie/{movieId}`
**Auth:** No requerida (AllowAnonymous)
**Status esperado:** 200 OK

```
GET http://localhost:5000/api/screenings/by-movie/1
```

**Response:**
```json
[
  {
    "id": "1",
    "movieId": "1",
    "theaterRoomId": "1",
    "startTime": "2025-11-04T14:00:00Z",
    "endTime": "2025-11-04T16:30:00Z"
  }
]
```

---

### 13. Crear Screening (Requiere Auth)

Crea una nueva función de cine.

**Endpoint:** `POST /api/screenings`
**Auth:** ✅ Requerida (Bearer Token)
**Status esperado:** 201 Created

```
POST http://localhost:5000/api/screenings
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "movieId": "1",
  "theaterRoomId": "2",
  "startTime": "2025-11-05T18:00:00Z",
  "endTime": "2025-11-05T20:30:00Z"
}
```

**Response:**
```json
{
  "id": "3",
  "movieId": "1",
  "theaterRoomId": "2",
  "startTime": "2025-11-05T18:00:00Z",
  "endTime": "2025-11-05T20:30:00Z"
}
```

---

### 14. Actualizar Screening (Requiere Auth)

Actualiza una función existente.

**Endpoint:** `PUT /api/screenings/{id}`
**Auth:** ✅ Requerida (Bearer Token)
**Status esperado:** 200 OK

```
PUT http://localhost:5000/api/screenings/3
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "movieId": "1",
  "theaterRoomId": "2",
  "startTime": "2025-11-05T19:00:00Z",
  "endTime": "2025-11-05T21:30:00Z"
}
```

---

### 15. Eliminar Screening (Requiere Auth)

Elimina una función del sistema.

**Endpoint:** `DELETE /api/screenings/{id}`
**Auth:** ✅ Requerida (Bearer Token)
**Status esperado:** 204 No Content

```
DELETE http://localhost:5000/api/screenings/3
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Users

### 16. Obtener Usuario por UID

Obtiene los detalles de un usuario específico.

**Endpoint:** `GET /api/FirebaseTest/get-user/{uid}`
**Auth:** No especificada (puede requerir Auth según configuración)
**Status esperado:** 200 OK

```
GET http://localhost:5000/api/FirebaseTest/get-user/{uid}
```

**Response:**
```json
{
  "success": true,
  "user": {
    "uid": "user-uid",
    "email": "usuario@test.com",
    "displayName": "Usuario de Prueba",
    "emailVerified": false,
    "disabled": false,
    "role": "user"
  }
}
```

---

### 17. Listar Todos los Usuarios

Obtiene lista de todos los usuarios (puede requerir feature flag habilitado).

**Endpoint:** `GET /api/FirebaseTest/get-all-users`
**Auth:** ⚠️ Puede requerir Bearer Token
**Feature Flag:** `DatabaseReadAll` debe estar habilitado
**Status esperado:** 200 OK

```
GET http://localhost:5000/api/FirebaseTest/get-all-users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:**
```json
{
  "success": true,
  "users": [
    {
      "uid": "user-uid-1",
      "email": "usuario1@test.com",
      "displayName": "Usuario 1",
      "emailVerified": false,
      "disabled": false,
      "role": "user"
    },
    {
      "uid": "user-uid-2",
      "email": "usuario2@test.com",
      "displayName": "Usuario 2",
      "emailVerified": false,
      "disabled": false,
      "role": "user"
    }
  ]
}
```

---

### 18. Actualizar Usuario

Actualiza los datos de un usuario.

**Endpoint:** `PUT /api/FirebaseTest/edit-user/{uid}`
**Auth:** Puede requerir Auth
**Status esperado:** 200 OK

```
PUT http://localhost:5000/api/FirebaseTest/edit-user/{uid}
Content-Type: application/json

{
  "email": "usuario@test.com",
  "displayName": "Nombre Actualizado",
  "role": "admin",
  "emailVerified": true,
  "disabled": false
}
```

**⚠️ Nota:** Si se incluye password en el body, será hasheado con BCrypt.

---

### 19. Eliminar Usuario

Elimina un usuario del sistema.

**Endpoint:** `DELETE /api/FirebaseTest/delete-user/{uid}`
**Auth:** Puede requerir Auth
**Status esperado:** 200 OK

```
DELETE http://localhost:5000/api/FirebaseTest/delete-user/{uid}
```

**Response:**
```json
{
  "success": true,
  "message": "User {uid} deleted."
}
```

---

## 🔒 Notas de Seguridad

### Cómo usar el Bearer Token

1. **Obtener el token:** Haz login (request #3) y copia el `jwtToken` de la respuesta
2. **Usar el token:** En requests protegidos, agrega el header:
   ```
   Authorization: Bearer {tu-token-aqui}
   ```

### Estructura del JWT Token

El token tiene 3 partes separadas por puntos:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.  <- Header
eyJzdWIiOiJ1c2VyLXVpZCIsImVtYWlsIjoi...  <- Payload (claims)
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV...    <- Signature
```

**Payload contiene:**
- `sub`: UID del usuario
- `email`: Email del usuario
- `role`: Rol (user, admin, etc.)
- `exp`: Timestamp de expiración
- `iss`: Issuer (CinemaApi)
- `aud`: Audience (CinemaClient)

**Expiración:**
- Default: 60 minutos
- Configurable en `appsettings.json` → `Jwt:ExpiresMinutes`

---

## 📝 Variables de Entorno (Postman)

Si usas Postman, puedes crear estas variables:

```json
{
  "baseUrl": "http://localhost:5000",
  "authToken": "tu-token-aqui"
}
```

Luego úsalas en requests:
```
GET {{baseUrl}}/api/movies
Authorization: Bearer {{authToken}}
```

---

## 🧪 Flujo de Prueba Recomendado

1. **Health Check** (Request #1)
2. **Registrar Usuario** (Request #2)
3. **Login** (Request #3) → Guardar token
4. **Listar Movies** (Request #5)
5. **Crear Movie** (Request #7) → Usar token
6. **Actualizar Movie** (Request #8) → Usar token
7. **Eliminar Movie** (Request #9) → Usar token
8. **Listar Screenings** (Request #10)
9. **Crear Screening** (Request #13) → Usar token

---

## 🐛 Troubleshooting

### Error 401 Unauthorized
- Verifica que el token sea válido
- Verifica que el token no haya expirado (60 min default)
- Verifica el formato: `Authorization: Bearer {token}`

### Error 404 Not Found
- Verifica que la ruta sea correcta
- Verifica que el backend esté corriendo
- Verifica el método HTTP (GET, POST, PUT, DELETE)

### Error 500 Internal Server Error
- Revisa los logs del backend
- Verifica que todos los campos requeridos estén presentes
- Verifica el formato del JSON

---

**Última actualización:** 04 Nov 2025
**Versión API:** 1.0
