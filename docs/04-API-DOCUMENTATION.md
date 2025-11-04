# API Documentation - Cinema Management System

**Base URL:** `https://localhost:7238`
**Environment:** Development
**Última actualización:** 2025-11-03

---

## Tabla de Contenidos

1. [Autenticación](#autenticación)
2. [Endpoints](#endpoints)
   - [Health Check](#health-check)
   - [Authentication](#authentication-endpoints)
   - [Users](#users-endpoints)
   - [Movies](#movies-endpoints)
   - [Screenings](#screenings-endpoints)
   - [Theater Rooms](#theater-rooms-endpoints)
   - [Food Combos](#food-combos-endpoints)
   - [Food Orders](#food-orders-endpoints)
3. [Modelos de Datos](#modelos-de-datos)
4. [Códigos de Error](#códigos-de-error)
5. [Ejemplos de Uso](#ejemplos-de-uso)

---

## Autenticación

### JWT Bearer Token

Todos los endpoints protegidos requieren un token JWT en el header:

```
Authorization: Bearer <token>
```

### Obtener Token

Usar el endpoint `/api/FirebaseTest/login` para obtener un token.

**Ejemplo:**
```bash
curl -X POST https://localhost:7238/api/FirebaseTest/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Login successful",
  "uid": "abc123xyz",
  "email": "user@example.com",
  "displayName": "John Doe",
  "role": "user",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Roles

| Rol | Descripción | Permisos |
|-----|-------------|----------|
| `admin` | Administrador | Acceso completo a todos los endpoints |
| `user` | Usuario regular | Acceso limitado (solo lectura y sus propias operaciones) |

---

## Endpoints

### Health Check

#### GET `/health`

Verifica que el API esté funcionando.

**Autenticación:** No requerida

**Respuesta:**
```json
{
  "status": "Healthy",
  "timestamp": "2025-11-03T10:30:00Z"
}
```

---

## Authentication Endpoints

### POST `/api/FirebaseTest/login`

Autenticar usuario y obtener token JWT.

**Autenticación:** No requerida

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "uid": "abc123xyz",
  "email": "user@example.com",
  "displayName": "John Doe",
  "role": "user",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Invalid email or password"
}
```

---

### GET `/api/me`

Obtener información del usuario autenticado.

**Autenticación:** Requerida

**Headers:**
```
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
{
  "userId": "abc123xyz",
  "email": "user@example.com",
  "role": "user"
}
```

---

## Users Endpoints

### GET `/api/FirebaseTest/get-all-users`

Listar todos los usuarios (Admin only).

**Autenticación:** Requerida (Feature Flag: `DatabaseReadAll`)

**Response (200 OK):**
```json
[
  {
    "uid": "user1",
    "email": "user1@example.com",
    "displayName": "User One",
    "emailVerified": true,
    "disabled": false,
    "role": "user"
  },
  {
    "uid": "admin1",
    "email": "admin@example.com",
    "displayName": "Admin",
    "emailVerified": true,
    "disabled": false,
    "role": "admin"
  }
]
```

---

### POST `/api/FirebaseTest/add-user`

Crear nuevo usuario.

**Autenticación:** Opcional (Admin recomendado)

**Request Body:**
```json
{
  "email": "newuser@example.com",
  "displayName": "New User",
  "role": "user",
  "password": "securePassword123"
}
```

**Response (201 Created):**
```json
{
  "uid": "newUserUid",
  "message": "User created successfully"
}
```

---

### GET `/api/FirebaseTest/get-user/{uid}`

Obtener usuario por UID.

**Autenticación:** Opcional

**Response (200 OK):**
```json
{
  "uid": "user1",
  "email": "user1@example.com",
  "displayName": "User One",
  "emailVerified": true,
  "disabled": false,
  "role": "user",
  "password": "hashedPassword"
}
```

---

### PUT `/api/FirebaseTest/edit-user/{uid}`

Actualizar información de usuario.

**Autenticación:** Opcional

**Request Body:**
```json
{
  "email": "updated@example.com",
  "displayName": "Updated Name",
  "role": "admin",
  "disabled": false
}
```

**Response (200 OK):**
```json
{
  "message": "User updated successfully"
}
```

---

### DELETE `/api/FirebaseTest/delete-user/{uid}`

Eliminar usuario.

**Autenticación:** Opcional (Admin recomendado)

**Response (200 OK):**
```json
{
  "message": "User deleted successfully"
}
```

---

## Movies Endpoints

### GET `/api/movies`

Obtener todas las películas.

**Autenticación:** No requerida

**Response (200 OK):**
```json
[
  {
    "id": "movie1",
    "title": "Inception",
    "description": "A mind-bending thriller",
    "durationMinutes": 148,
    "genre": "Sci-Fi",
    "director": "Christopher Nolan"
  },
  {
    "id": "movie2",
    "title": "The Dark Knight",
    "description": "Batman faces the Joker",
    "durationMinutes": 152,
    "genre": "Action",
    "director": "Christopher Nolan"
  }
]
```

---

### POST `/api/movies/add-movie` ⚠️ TODO

Agregar nueva película.

**Autenticación:** Requerida (Admin)

**Request Body:**
```json
{
  "title": "New Movie",
  "description": "Description of the movie",
  "durationMinutes": 120,
  "genre": "Action",
  "director": "Director Name",
  "releaseDate": "2025-12-01T00:00:00Z",
  "rating": 4.5,
  "posterUrl": "https://example.com/poster.jpg",
  "trailerUrl": "https://youtube.com/watch?v=xyz",
  "classification": "PG-13",
  "isActive": true,
  "language": "English"
}
```

**Response (201 Created):**
```json
{
  "id": "newMovieId",
  "message": "Movie created successfully"
}
```

---

### GET `/api/movies/get-movie/{id}` ⚠️ TODO

Obtener película por ID.

**Autenticación:** Requerida

**Response (200 OK):**
```json
{
  "id": "movie1",
  "title": "Inception",
  "description": "A mind-bending thriller",
  "durationMinutes": 148,
  "genre": "Sci-Fi",
  "director": "Christopher Nolan",
  "releaseDate": "2010-07-16T00:00:00Z",
  "rating": 4.8,
  "posterUrl": "https://example.com/inception.jpg",
  "trailerUrl": "https://youtube.com/watch?v=abc",
  "classification": "PG-13",
  "isActive": true,
  "language": "English"
}
```

**Response (404 Not Found):**
```json
{
  "message": "Movie not found"
}
```

---

### PUT `/api/movies/edit-movie/{id}` ⚠️ TODO

Editar película existente.

**Autenticación:** Requerida (Admin)

**Request Body:**
```json
{
  "title": "Updated Title",
  "description": "Updated description",
  "durationMinutes": 150,
  "genre": "Thriller",
  "director": "Updated Director",
  "rating": 4.9,
  "isActive": false
}
```

**Response (200 OK):**
```json
{
  "message": "Movie updated successfully"
}
```

---

### DELETE `/api/movies/delete-movie/{id}` ⚠️ TODO

Eliminar película.

**Autenticación:** Requerida (Admin)

**Response (200 OK):**
```json
{
  "message": "Movie deleted successfully"
}
```

---

## Screenings Endpoints

### POST `/api/screenings/add-screening` ⚠️ TODO

Agregar nueva proyección.

**Autenticación:** Requerida (Admin)

**Request Body:**
```json
{
  "movieId": "movie1",
  "theaterRoomId": "room1",
  "startTime": "2025-11-05T19:30:00Z",
  "price": 8.50,
  "isActive": true
}
```

**Response (201 Created):**
```json
{
  "id": "screening1",
  "endTime": "2025-11-05T22:00:00Z",
  "availableSeats": 150,
  "message": "Screening created successfully"
}
```

---

### GET `/api/screenings/get-screening/{id}` ⚠️ TODO

Obtener proyección por ID.

**Autenticación:** Requerida

**Response (200 OK):**
```json
{
  "id": "screening1",
  "movieId": "movie1",
  "theaterRoomId": "room1",
  "startTime": "2025-11-05T19:30:00Z",
  "endTime": "2025-11-05T22:00:00Z",
  "price": 8.50,
  "availableSeats": 120,
  "isActive": true
}
```

---

### GET `/api/screenings/by-movie/{movieId}` ⚠️ TODO

Obtener todas las proyecciones de una película.

**Autenticación:** Requerida

**Response (200 OK):**
```json
[
  {
    "id": "screening1",
    "movieId": "movie1",
    "theaterRoomId": "room1",
    "startTime": "2025-11-05T19:30:00Z",
    "endTime": "2025-11-05T22:00:00Z",
    "price": 8.50,
    "availableSeats": 120
  },
  {
    "id": "screening2",
    "movieId": "movie1",
    "theaterRoomId": "room2",
    "startTime": "2025-11-05T22:00:00Z",
    "endTime": "2025-11-06T00:30:00Z",
    "price": 10.00,
    "availableSeats": 80
  }
]
```

---

### PUT `/api/screenings/edit-screening/{id}` ⚠️ TODO

Editar proyección.

**Autenticación:** Requerida (Admin)

**Request Body:**
```json
{
  "startTime": "2025-11-05T20:00:00Z",
  "price": 9.00,
  "isActive": false
}
```

**Response (200 OK):**
```json
{
  "message": "Screening updated successfully"
}
```

---

### DELETE `/api/screenings/delete-screening/{id}` ⚠️ TODO

Eliminar proyección.

**Autenticación:** Requerida (Admin)

**Response (200 OK):**
```json
{
  "message": "Screening deleted successfully"
}
```

---

## Theater Rooms Endpoints

### GET `/api/theaterrooms/get-all-theater-rooms` ⚠️ TODO

Obtener todas las salas de cine.

**Autenticación:** Requerida

**Response (200 OK):**
```json
[
  {
    "id": "room1",
    "name": "Sala 1 - IMAX",
    "capacity": 150,
    "rows": 10,
    "columns": 15,
    "screenType": "IMAX",
    "features": ["Dolby Atmos", "Reclining Seats"],
    "isActive": true
  },
  {
    "id": "room2",
    "name": "Sala 2 - 3D",
    "capacity": 100,
    "rows": 10,
    "columns": 10,
    "screenType": "3D",
    "features": ["3D Projection"],
    "isActive": true
  }
]
```

---

### POST `/api/theaterrooms/add-theater-room` ⚠️ TODO

Agregar nueva sala.

**Autenticación:** Requerida (Admin)

**Request Body:**
```json
{
  "name": "Sala 3 - VIP",
  "capacity": 50,
  "rows": 5,
  "columns": 10,
  "screenType": "Standard",
  "features": ["VIP Seating", "Bar Service"],
  "isActive": true
}
```

**Response (201 Created):**
```json
{
  "id": "room3",
  "message": "Theater room created successfully"
}
```

---

## Food Combos Endpoints

### GET `/api/foodcombos/get-all-food-combos` ⚠️ TODO

Obtener todos los combos de alimentos.

**Autenticación:** Requerida

**Response (200 OK):**
```json
[
  {
    "id": "combo1",
    "name": "Combo Familiar",
    "description": "2 Palomitas grandes + 4 Refrescos",
    "price": 25.00,
    "items": ["Palomitas Grandes (x2)", "Refresco Grande (x4)"],
    "imageUrl": "https://example.com/combo1.jpg",
    "isAvailable": true,
    "category": "Combos"
  },
  {
    "id": "snack1",
    "name": "Palomitas Medianas",
    "description": "Palomitas de maíz medianas",
    "price": 5.00,
    "items": ["Palomitas Medianas"],
    "imageUrl": "https://example.com/popcorn.jpg",
    "isAvailable": true,
    "category": "Snacks"
  }
]
```

---

### GET `/api/foodcombos/available` ⚠️ TODO

Obtener solo combos disponibles.

**Autenticación:** No requerida

**Response (200 OK):**
```json
[
  {
    "id": "combo1",
    "name": "Combo Familiar",
    "price": 25.00,
    "imageUrl": "https://example.com/combo1.jpg"
  }
]
```

---

### POST `/api/foodcombos/add-food-combo` ⚠️ TODO

Agregar nuevo combo.

**Autenticación:** Requerida (Admin)

**Request Body:**
```json
{
  "name": "Combo Romántico",
  "description": "1 Palomitas grande + 2 Refrescos",
  "price": 15.00,
  "items": ["Palomitas Grandes", "Refresco Grande (x2)"],
  "imageUrl": "https://example.com/combo-romantico.jpg",
  "isAvailable": true,
  "category": "Combos"
}
```

**Response (201 Created):**
```json
{
  "id": "combo2",
  "message": "Food combo created successfully"
}
```

---

## Food Orders Endpoints

### POST `/api/foodorders/add-food-order` ⚠️ TODO

Crear orden de comida.

**Autenticación:** Requerida

**Request Body:**
```json
{
  "userId": "user1",
  "foodComboIds": ["combo1", "snack1"],
  "bookingId": "booking123",
  "status": "pending",
  "paymentMethod": "credit_card"
}
```

**Response (201 Created):**
```json
{
  "id": "order1",
  "totalPrice": 30.00,
  "createdAt": "2025-11-03T10:30:00Z",
  "message": "Food order created successfully"
}
```

---

### GET `/api/foodorders/user/{userId}` ⚠️ TODO

Obtener órdenes de un usuario.

**Autenticación:** Requerida

**Response (200 OK):**
```json
[
  {
    "id": "order1",
    "userId": "user1",
    "foodComboIds": ["combo1", "snack1"],
    "totalPrice": 30.00,
    "status": "completed",
    "createdAt": "2025-11-03T10:30:00Z",
    "updatedAt": "2025-11-03T11:00:00Z"
  }
]
```

---

### PUT `/api/foodorders/{id}/complete` ⚠️ TODO

Marcar orden como completada.

**Autenticación:** Requerida (Admin)

**Response (200 OK):**
```json
{
  "message": "Food order marked as completed"
}
```

---

## Modelos de Datos

### User
```json
{
  "uid": "string",
  "email": "string",
  "displayName": "string",
  "emailVerified": "boolean",
  "disabled": "boolean",
  "role": "string",
  "password": "string"
}
```

### Movie
```json
{
  "id": "string",
  "title": "string",
  "description": "string",
  "durationMinutes": "integer",
  "genre": "string",
  "director": "string",
  "releaseDate": "datetime",
  "rating": "decimal",
  "posterUrl": "string",
  "trailerUrl": "string",
  "classification": "string",
  "isActive": "boolean",
  "language": "string"
}
```

### Screening
```json
{
  "id": "string",
  "movieId": "string",
  "theaterRoomId": "string",
  "startTime": "datetime",
  "endTime": "datetime",
  "price": "decimal",
  "availableSeats": "integer",
  "isActive": "boolean"
}
```

### TheaterRoom
```json
{
  "id": "string",
  "name": "string",
  "capacity": "integer",
  "rows": "integer",
  "columns": "integer",
  "screenType": "string",
  "features": "array<string>",
  "isActive": "boolean"
}
```

### FoodCombo
```json
{
  "id": "string",
  "name": "string",
  "description": "string",
  "price": "decimal",
  "items": "array<string>",
  "imageUrl": "string",
  "isAvailable": "boolean",
  "category": "string"
}
```

### FoodOrder
```json
{
  "id": "string",
  "userId": "string",
  "foodComboIds": "array<string>",
  "totalPrice": "decimal",
  "status": "string",
  "bookingId": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "paymentMethod": "string"
}
```

---

## Códigos de Error

| Código | Descripción | Solución |
|--------|-------------|----------|
| 200 | OK | Solicitud exitosa |
| 201 | Created | Recurso creado exitosamente |
| 400 | Bad Request | Validar datos enviados |
| 401 | Unauthorized | Verificar token de autenticación |
| 403 | Forbidden | Usuario no tiene permisos suficientes |
| 404 | Not Found | Recurso no encontrado |
| 500 | Internal Server Error | Contactar soporte |

---

## Ejemplos de Uso

### Ejemplo 1: Login y obtener películas

```bash
# 1. Login
curl -X POST https://localhost:7238/api/FirebaseTest/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'

# Respuesta:
# {"success":true,"token":"eyJhbGci...","uid":"abc123"}

# 2. Guardar token
TOKEN="eyJhbGci..."

# 3. Obtener películas (público)
curl -X GET https://localhost:7238/api/movies

# 4. Obtener información del usuario actual
curl -X GET https://localhost:7238/api/me \
  -H "Authorization: Bearer $TOKEN"
```

---

### Ejemplo 2: Crear película (Admin)

```bash
TOKEN="admin_token_here"

curl -X POST https://localhost:7238/api/movies/add-movie \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "The Matrix",
    "description": "A computer hacker learns about the true nature of reality",
    "durationMinutes": 136,
    "genre": "Sci-Fi",
    "director": "The Wachowskis",
    "releaseDate": "1999-03-31T00:00:00Z",
    "rating": 4.7,
    "posterUrl": "https://example.com/matrix.jpg",
    "classification": "R",
    "isActive": true,
    "language": "English"
  }'
```

---

### Ejemplo 3: Crear proyección

```bash
TOKEN="admin_token_here"

curl -X POST https://localhost:7238/api/screenings/add-screening \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "movieId": "movie1",
    "theaterRoomId": "room1",
    "startTime": "2025-11-05T19:30:00Z",
    "price": 8.50,
    "isActive": true
  }'
```

---

### Ejemplo 4: Obtener proyecciones de una película

```bash
curl -X GET "https://localhost:7238/api/screenings/by-movie/movie1" \
  -H "Authorization: Bearer $TOKEN"
```

---

## Swagger UI

Para documentación interactiva, visitar:

```
https://localhost:7238/swagger
```

La interfaz de Swagger permite:
- Probar endpoints directamente
- Ver esquemas de modelos
- Generar código cliente
- Exportar especificación OpenAPI

---

## Notas de Versiones

### v1.0.0 (Actual)
- ✅ Autenticación con JWT
- ✅ CRUD de usuarios
- ✅ GET de películas
- ⚠️ Endpoints CRUD de Movies, Screenings, TheaterRooms, FoodCombos pendientes

### v1.1.0 (Próxima)
- CRUD completo de Movies
- CRUD completo de Screenings
- CRUD completo de Theater Rooms
- CRUD completo de Food Combos
- CRUD completo de Food Orders

### v1.2.0 (Futuro)
- Bookings & Seats management
- Payment integration
- Search & filtering
- Reports & analytics

---

**Mantenido por:** Equipo de Desarrollo Cinema System
**Última revisión:** 2025-11-03
