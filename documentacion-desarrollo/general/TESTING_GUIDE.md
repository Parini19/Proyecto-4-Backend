# 🧪 Guía Completa de Pruebas - Sistema Cinema

**Fecha de Creación:** 04 de Noviembre, 2025
**Versión:** 1.0
**Estado:** Primera prueba del sistema integrado

---

## 📋 Índice

1. [Pre-requisitos](#pre-requisitos)
2. [Configuración Inicial](#configuración-inicial)
3. [Pruebas del Backend](#pruebas-del-backend)
4. [Pruebas del Frontend](#pruebas-del-frontend)
5. [Pruebas de Integración](#pruebas-de-integración)
6. [Pruebas de Seguridad](#pruebas-de-seguridad)
7. [Resolución de Problemas](#resolución-de-problemas)
8. [Checklist Final](#checklist-final)

---

## Pre-requisitos

### Software Necesario
- [ ] .NET 9.0 SDK instalado
- [ ] Flutter SDK instalado (versión 3.35.4 o superior)
- [ ] Chrome o Edge (para Flutter Web)
- [ ] Postman o herramienta similar (opcional pero recomendado)
- [ ] Visual Studio Code o IDE de tu preferencia

### Verificar Versiones
```bash
# Verificar .NET
dotnet --version
# Debería mostrar: 9.0.x

# Verificar Flutter
flutter --version
# Debería mostrar: Flutter 3.35.4 o superior
```

---

## Configuración Inicial

### 1. Configurar Backend

**A. Navegar al proyecto:**
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
```

**B. Verificar configuración de appsettings.json:**
```json
{
  "Firebase": {
    "Enabled": false,  // DEBE estar en false para usar InMemory
    "ProjectId": "tu-proyecto",
    "ConfigPath": "./Config/firebase-credentials.json"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5200",
      "http://localhost:5201",
      "http://localhost:5202",
      "http://localhost:5203",
      "http://localhost:5204",
      "http://localhost:5205"
    ]
  },
  "Jwt": {
    "Key": "tu-super-secret-key-de-minimo-32-caracteres",
    "Issuer": "CinemaApi",
    "Audience": "CinemaClient",
    "ExpiresMinutes": "60"
  }
}
```

**C. Restaurar dependencias:**
```bash
dotnet restore
```

**D. Iniciar el backend:**
```bash
dotnet run
```

**✅ Resultado esperado:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

**⚠️ IMPORTANTE:** Deja esta terminal abierta. El backend debe estar corriendo durante todas las pruebas.

---

### 2. Configurar Frontend

**A. Abrir nueva terminal y navegar al proyecto:**
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
```

**B. Verificar configuración de ApiService:**

Archivo: `lib/core/services/api_service.dart`
```dart
static const String _baseUrl = 'http://localhost:5000/api';
```

**C. Instalar dependencias (si es necesario):**
```bash
flutter pub get
```

**D. Iniciar el frontend:**
```bash
flutter run -d chrome --web-port 5210
```

**✅ Resultado esperado:**
```
Launching lib/main.dart on Chrome in debug mode...
Building application for the web...
...
Application finished.
```

---

## Pruebas del Backend

### Prueba 1: Health Check

**Objetivo:** Verificar que el backend está corriendo correctamente.

**Método:** GET
**URL:** `http://localhost:5000/health`
**Headers:** Ninguno

**Pasos:**
1. Abre tu navegador
2. Navega a: `http://localhost:5000/health`

**✅ Resultado esperado:**
```json
{
  "status": "ok",
  "time": "2025-11-04T..."
}
```

**Status Code:** 200 OK

---

### Prueba 2: Listar Películas (GET)

**Objetivo:** Verificar que el endpoint de películas retorna los datos InMemory.

**Método:** GET
**URL:** `http://localhost:5000/api/movies`
**Headers:** Ninguno

**Pasos con Postman:**
1. Crear nueva request GET
2. URL: `http://localhost:5000/api/movies`
3. Click en "Send"

**Pasos con navegador:**
1. Navega a: `http://localhost:5000/api/movies`

**✅ Resultado esperado:**
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

**Status Code:** 200 OK

---

### Prueba 3: Obtener Película por ID

**Objetivo:** Verificar que se puede obtener una película específica.

**Método:** GET
**URL:** `http://localhost:5000/api/movies/1`
**Headers:** Ninguno

**✅ Resultado esperado:**
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

**Status Code:** 200 OK

**Prueba negativa - ID inexistente:**
- URL: `http://localhost:5000/api/movies/999`
- **✅ Resultado esperado:** 404 Not Found
```json
{
  "message": "Movie with id '999' not found"
}
```

---

### Prueba 4: Listar Screenings

**Objetivo:** Verificar que el endpoint de funciones retorna datos.

**Método:** GET
**URL:** `http://localhost:5000/api/screenings`
**Headers:** Ninguno

**✅ Resultado esperado:**
```json
[
  {
    "id": "1",
    "movieId": "1",
    "theaterRoomId": "1",
    "startTime": "2025-11-04T...",
    "endTime": "2025-11-04T..."
  },
  {
    "id": "2",
    "movieId": "2",
    "theaterRoomId": "1",
    "startTime": "2025-11-04T...",
    "endTime": "2025-11-04T..."
  }
]
```

**Status Code:** 200 OK

---

### Prueba 5: Obtener Screenings por Película

**Objetivo:** Verificar el endpoint especial de screenings por película.

**Método:** GET
**URL:** `http://localhost:5000/api/screenings/by-movie/1`
**Headers:** Ninguno

**✅ Resultado esperado:**
```json
[
  {
    "id": "1",
    "movieId": "1",
    "theaterRoomId": "1",
    "startTime": "...",
    "endTime": "..."
  }
]
```

**Status Code:** 200 OK

---

### Prueba 6: Crear Película SIN Token (Debe Fallar)

**Objetivo:** Verificar que los endpoints protegidos requieren autenticación.

**Método:** POST
**URL:** `http://localhost:5000/api/movies`
**Headers:**
```
Content-Type: application/json
```
**Body:**
```json
{
  "title": "Test Movie",
  "description": "This is a test movie",
  "durationMinutes": 120,
  "genre": "Action",
  "director": "Test Director",
  "year": 2024
}
```

**✅ Resultado esperado:**
- **Status Code:** 401 Unauthorized

**⚠️ Nota:** Este es el comportamiento esperado. Necesitas un token JWT para crear películas.

---

## Pruebas del Frontend

### Prueba 7: Registro de Usuario

**Objetivo:** Crear un nuevo usuario y verificar que la contraseña se hashea con BCrypt.

**Pasos:**
1. Abre el frontend en Chrome: `http://localhost:5210`
2. Deberías ver la página de Login
3. Click en el botón **"Regístrate"** (abajo del formulario)
4. Completa el formulario de registro:
   - **Nombre completo:** Juan Pérez
   - **Correo electrónico:** juan@test.com
   - **Contraseña:** 123456
   - **Confirmar contraseña:** 123456
5. Click en **"Crear Cuenta"**

**✅ Resultado esperado:**
- Mensaje de éxito: "Cuenta creada exitosamente" o similar
- Deberías ver los datos del usuario creado
- La app debería permitirte continuar

**Verificación en Backend:**
- En la terminal del backend, deberías ver logs de la request POST a `/api/FirebaseTest/add-user`
- La contraseña NO debe ser "123456" en la base de datos
- Debe ser un hash BCrypt que comienza con "$2a$" o "$2b$"

**📝 Anota el UID generado:** ___________________________

---

### Prueba 8: Login con Credenciales Correctas

**Objetivo:** Iniciar sesión y obtener un JWT token.

**Pre-requisito:** Debes haber completado la Prueba 7 (Registro).

**Pasos:**
1. Si no estás en la página de Login, navega a ella
2. Ingresa las credenciales:
   - **Correo electrónico:** juan@test.com
   - **Contraseña:** 123456
3. Click en **"Iniciar Sesión"**

**✅ Resultado esperado:**
- Mensaje de éxito: "¡Bienvenido Juan Pérez!" (con el nombre que registraste)
- Redirección automática a la página Home
- En los logs del backend deberías ver: POST `/api/FirebaseTest/login` con status 200

**Verificar Token Guardado:**
1. Abre Chrome DevTools (F12)
2. Ve a la pestaña **"Application"** (o "Aplicación")
3. En el menú izquierdo: **Local Storage** > `http://localhost:5210`
4. Busca la key: `auth_token`
5. Deberías ver un JWT token (un string largo que comienza con "eyJ...")

**📝 Copia el token para la siguiente prueba:**
```
eyJ...
```

---

### Prueba 9: Login con Contraseña Incorrecta (Debe Fallar)

**Objetivo:** Verificar que BCrypt valida correctamente las contraseñas.

**Pasos:**
1. Si estás logueado, haz logout (o usa una ventana de incógnito)
2. En la página de Login, ingresa:
   - **Correo electrónico:** juan@test.com
   - **Contraseña:** wrongpassword
3. Click en **"Iniciar Sesión"**

**✅ Resultado esperado:**
- ❌ Error: "Invalid credentials" o "Contraseña incorrecta"
- NO se debe guardar token en Local Storage
- NO se debe redirigir a Home
- Status 401 Unauthorized en el backend

---

### Prueba 10: Login con Email Inexistente (Debe Fallar)

**Objetivo:** Verificar validación de usuarios.

**Pasos:**
1. En la página de Login, ingresa:
   - **Correo electrónico:** noexiste@test.com
   - **Contraseña:** 123456
2. Click en **"Iniciar Sesión"**

**✅ Resultado esperado:**
- ❌ Error: "User not found" o "Usuario no encontrado"
- Status 401 Unauthorized en el backend

---

### Prueba 11: Navegación como Invitado

**Objetivo:** Verificar que se puede acceder sin login.

**Pasos:**
1. En la página de Login
2. Click en **"Continuar como invitado"** (abajo del formulario)

**✅ Resultado esperado:**
- Redirección a HomePage
- Puedes ver la cartelera de películas
- NO hay token en Local Storage
- Algunas funciones pueden estar limitadas

---

## Pruebas de Integración

### Prueba 12: Crear Película con Token Válido

**Objetivo:** Verificar que los endpoints protegidos funcionan con autenticación.

**Pre-requisito:** Debes tener un token JWT válido de la Prueba 8.

**Usando Postman:**

**Método:** POST
**URL:** `http://localhost:5000/api/movies`
**Headers:**
```
Content-Type: application/json
Authorization: Bearer eyJ... (tu token aquí)
```
**Body:**
```json
{
  "title": "Matrix Resurrections",
  "description": "Return to the world of two realities",
  "durationMinutes": 148,
  "genre": "Science Fiction",
  "director": "Lana Wachowski",
  "year": 2021
}
```

**✅ Resultado esperado:**
- **Status Code:** 201 Created
- Respuesta con la película creada incluyendo un ID generado
```json
{
  "id": "4",
  "title": "Matrix Resurrections",
  ...
}
```

**Verificación:**
1. Ahora haz GET a `http://localhost:5000/api/movies`
2. Deberías ver 4 películas (las 3 originales + la nueva)

---

### Prueba 13: Actualizar Película

**Objetivo:** Verificar el endpoint PUT.

**Pre-requisito:** Token válido y la película creada en Prueba 12.

**Método:** PUT
**URL:** `http://localhost:5000/api/movies/4`
**Headers:**
```
Content-Type: application/json
Authorization: Bearer eyJ... (tu token aquí)
```
**Body:**
```json
{
  "title": "The Matrix Resurrections",
  "description": "Updated description - Return to the world of two realities",
  "durationMinutes": 148,
  "genre": "Science Fiction / Action",
  "director": "Lana Wachowski",
  "year": 2021
}
```

**✅ Resultado esperado:**
- **Status Code:** 200 OK
- Respuesta con la película actualizada

**Verificación:**
1. GET a `http://localhost:5000/api/movies/4`
2. Deberías ver los cambios reflejados

---

### Prueba 14: Eliminar Película

**Objetivo:** Verificar el endpoint DELETE.

**Pre-requisito:** Token válido.

**Método:** DELETE
**URL:** `http://localhost:5000/api/movies/4`
**Headers:**
```
Authorization: Bearer eyJ... (tu token aquí)
```

**✅ Resultado esperado:**
- **Status Code:** 204 No Content

**Verificación:**
1. GET a `http://localhost:5000/api/movies/4`
2. Debería retornar 404 Not Found
3. GET a `http://localhost:5000/api/movies` debería mostrar solo 3 películas

---

### Prueba 15: Crear Screening

**Objetivo:** Verificar CRUD de screenings.

**Pre-requisito:** Token válido.

**Método:** POST
**URL:** `http://localhost:5000/api/screenings`
**Headers:**
```
Content-Type: application/json
Authorization: Bearer eyJ... (tu token aquí)
```
**Body:**
```json
{
  "movieId": "1",
  "theaterRoomId": "2",
  "startTime": "2025-11-05T18:00:00Z",
  "endTime": "2025-11-05T20:30:00Z"
}
```

**✅ Resultado esperado:**
- **Status Code:** 201 Created
- Respuesta con el screening creado y un ID

---

### Prueba 16: Listar Todos los Usuarios (Requiere Feature Flag)

**Objetivo:** Verificar endpoint con feature flag.

**Pre-requisito:** Token válido.

**⚠️ Nota:** Este endpoint puede estar deshabilitado por feature flag.

**Método:** GET
**URL:** `http://localhost:5000/api/FirebaseTest/get-all-users`
**Headers:**
```
Authorization: Bearer eyJ... (tu token aquí)
```

**✅ Resultado esperado:**
- **Status Code:** 200 OK
- Lista de usuarios (sin contraseñas expuestas)
```json
{
  "success": true,
  "users": [
    {
      "uid": "...",
      "email": "juan@test.com",
      "displayName": "Juan Pérez",
      "emailVerified": false,
      "disabled": false,
      "role": "user"
    }
  ]
}
```

---

## Pruebas de Seguridad

### Prueba 17: Verificar Hashing de Contraseñas

**Objetivo:** Confirmar que BCrypt está funcionando correctamente.

**Método Manual:**

1. Crea un segundo usuario desde el frontend:
   - Email: test2@test.com
   - Password: password123
   - Nombre: Test User 2

2. **En el código del backend**, agrega un breakpoint o log temporal en:
   - Archivo: `FirestoreUserService.cs`
   - Línea 34 (después del hash): `user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);`

3. Ejecuta el registro en modo debug

**✅ Verificaciones:**
- La contraseña original "password123" debe transformarse
- El hash debe comenzar con "$2a$" o "$2b$"
- El hash debe tener longitud de ~60 caracteres
- Dos usuarios con la misma contraseña deben tener hashes DIFERENTES (por el salt aleatorio)

**Ejemplo de hash válido:**
```
$2a$11$xYz9Abc...defGhi (60 caracteres aprox)
```

---

### Prueba 18: Verificar Token JWT

**Objetivo:** Validar la estructura del JWT token.

**Pasos:**
1. Obtén un token de la Prueba 8
2. Ve a: https://jwt.io/
3. Pega tu token en el campo "Encoded"

**✅ Verificaciones en el Payload:**
```json
{
  "sub": "usuario-uid-aqui",
  "email": "juan@test.com",
  "role": "user",
  "jti": "guid-único",
  "exp": ...,
  "iss": "CinemaApi",
  "aud": "CinemaClient"
}
```

- **sub:** UID del usuario
- **email:** Email del usuario
- **role:** Rol asignado
- **exp:** Timestamp de expiración (debe ser 60 minutos en el futuro)
- **iss:** Debe ser "CinemaApi"
- **aud:** Debe ser "CinemaClient"

---

### Prueba 19: Token Expirado

**Objetivo:** Verificar que tokens expirados son rechazados.

**⚠️ Nota:** Esta prueba requiere esperar 60 minutos o modificar temporalmente la configuración.

**Opción A - Modificar configuración (más rápido):**
1. En `appsettings.json`, cambia:
```json
"Jwt": {
  "ExpiresMinutes": "1"  // 1 minuto
}
```
2. Reinicia el backend
3. Haz login y obtén un token
4. Espera 2 minutos
5. Intenta usar el token para crear una película

**✅ Resultado esperado:**
- **Status Code:** 401 Unauthorized
- Error relacionado con token expirado

---

### Prueba 20: Inyección SQL en Login

**Objetivo:** Verificar que no hay vulnerabilidades de SQL injection.

**⚠️ Nota:** Esta es una prueba de seguridad. El sistema debe rechazar estos intentos.

**Método:** POST
**URL:** `http://localhost:5000/api/FirebaseTest/login`
**Headers:**
```
Content-Type: application/json
```
**Body (intento de inyección):**
```json
{
  "email": "' OR '1'='1",
  "password": "' OR '1'='1"
}
```

**✅ Resultado esperado:**
- ❌ Login debe fallar
- **Status Code:** 401 Unauthorized
- Mensaje: "User not found" o "Invalid credentials"
- NO debe permitir acceso sin credenciales válidas

---

## Resolución de Problemas

### Problema 1: Backend no inicia

**Error:** `Unable to bind to http://localhost:5000`

**Solución:**
```bash
# Verificar qué proceso está usando el puerto 5000
netstat -ano | findstr :5000

# Si hay algo, detenerlo o cambiar el puerto en launchSettings.json
```

---

### Problema 2: Error de CORS en Frontend

**Error en console:** `Access to XMLHttpRequest blocked by CORS policy`

**Solución:**
1. Verifica que el puerto del frontend esté en `appsettings.json`:
```json
"Cors": {
  "AllowedOrigins": [
    "http://localhost:5210"  // Tu puerto aquí
  ]
}
```
2. Reinicia el backend

---

### Problema 3: Token no se guarda en Frontend

**Síntoma:** Login exitoso pero token no aparece en Local Storage

**Solución:**
1. Abre DevTools > Console
2. Busca errores de SharedPreferences
3. Verifica que `api_service.dart` tiene:
```dart
await prefs.setString('auth_token', token);
```
4. En Web, los datos van a Local Storage, no SharedPreferences

---

### Problema 4: Firebase:ProjectId missing

**Error:** `Firebase:ProjectId missing`

**Solución:**
Asegúrate que `appsettings.json` tiene:
```json
"Firebase": {
  "Enabled": false,  // IMPORTANTE: false para InMemory
  "ProjectId": "dummy-project",
  "ConfigPath": "./Config/dummy.json"
}
```

---

### Problema 5: Películas no se cargan en Frontend

**Síntoma:** HomePage vacía o error al cargar

**Solución:**
1. Verifica que el backend está corriendo
2. En DevTools Console, verifica requests a `/api/movies`
3. Verifica que `_baseUrl` en `api_service.dart` es `http://localhost:5000/api`

---

### Problema 6: Error 500 al crear usuario

**Síntoma:** POST a `/api/FirebaseTest/add-user` retorna 500

**Posibles causas:**
1. BCrypt.Net no instalado: `dotnet add package BCrypt.Net-Next`
2. Firestore mal configurado (si Firebase:Enabled = true)
3. Campos requeridos faltantes en el modelo User

**Solución:**
- Revisa los logs del backend para el stack trace completo
- Verifica que `Firebase:Enabled` esté en `false`

---

## Checklist Final

### Backend ✅

- [ ] Backend inicia sin errores
- [ ] GET /health retorna 200
- [ ] GET /api/movies retorna 3 películas
- [ ] GET /api/movies/1 retorna Inception
- [ ] GET /api/movies/999 retorna 404
- [ ] GET /api/screenings retorna 2 screenings
- [ ] GET /api/screenings/by-movie/1 funciona
- [ ] POST /api/movies sin token retorna 401
- [ ] POST /api/movies con token retorna 201
- [ ] PUT /api/movies/{id} con token retorna 200
- [ ] DELETE /api/movies/{id} con token retorna 204
- [ ] POST /api/screenings con token retorna 201

### Frontend ✅

- [ ] Frontend inicia sin errores en Chrome
- [ ] Página de Login se muestra correctamente
- [ ] Botón "Regístrate" navega a página de registro
- [ ] Registro crea usuario exitosamente
- [ ] Login con credenciales correctas funciona
- [ ] Login muestra mensaje de bienvenida con displayName
- [ ] Login redirige a HomePage
- [ ] Login con contraseña incorrecta falla apropiadamente
- [ ] Login con email inexistente falla apropiadamente
- [ ] Token se guarda en Local Storage
- [ ] "Continuar como invitado" funciona

### Seguridad ✅

- [ ] Contraseñas se guardan hasheadas con BCrypt
- [ ] Hashes comienzan con $2a$ o $2b$
- [ ] Login con password incorrecta retorna 401
- [ ] Endpoints protegidos requieren token
- [ ] Token incluye claims correctos (sub, email, role)
- [ ] Inyección SQL en login es rechazada
- [ ] Token expirado es rechazado (si se probó)

### Integración ✅

- [ ] Frontend puede llamar a backend sin CORS errors
- [ ] Login desde frontend guarda token
- [ ] Token se envía en requests subsecuentes
- [ ] Crear película desde Postman con token funciona
- [ ] Actualizar película desde Postman funciona
- [ ] Eliminar película desde Postman funciona
- [ ] Crear screening desde Postman funciona

---

## 📊 Registro de Pruebas

**Tester:** ___________________________
**Fecha:** ___________________________
**Hora de inicio:** ___________________
**Hora de fin:** ______________________

### Resumen de Resultados

| Prueba | Resultado | Notas |
|--------|-----------|-------|
| 1. Health Check | ⬜ | |
| 2. Listar Películas | ⬜ | |
| 3. Obtener Película por ID | ⬜ | |
| 4. Listar Screenings | ⬜ | |
| 5. Screenings por Película | ⬜ | |
| 6. Crear Película sin Token | ⬜ | |
| 7. Registro de Usuario | ⬜ | |
| 8. Login Correcto | ⬜ | |
| 9. Login Password Incorrecta | ⬜ | |
| 10. Login Email Inexistente | ⬜ | |
| 11. Navegación Invitado | ⬜ | |
| 12. Crear Película con Token | ⬜ | |
| 13. Actualizar Película | ⬜ | |
| 14. Eliminar Película | ⬜ | |
| 15. Crear Screening | ⬜ | |
| 16. Listar Usuarios | ⬜ | |
| 17. Hashing BCrypt | ⬜ | |
| 18. Verificar JWT | ⬜ | |
| 19. Token Expirado | ⬜ | |
| 20. Inyección SQL | ⬜ | |

**Leyenda:** ✅ Pasó | ❌ Falló | ⚠️ Con observaciones | ⬜ No probado

---

## 📝 Notas Adicionales

### Problemas Encontrados
```
(Anota aquí cualquier problema que encuentres)




```

### Mejoras Sugeridas
```
(Anota aquí sugerencias de mejora)




```

### Preguntas para el Equipo
```
(Anota aquí preguntas o dudas)




```

---

## 🎯 Próximos Pasos

Después de completar estas pruebas:

1. **Si todas las pruebas pasan:**
   - Documentar cualquier comportamiento inesperado pero funcional
   - Proceder con pruebas de carga (opcional)
   - Preparar para pruebas de usuario

2. **Si hay pruebas fallidas:**
   - Documentar el error exacto
   - Incluir screenshots si es posible
   - Reportar al equipo de desarrollo
   - Re-ejecutar después de correcciones

3. **Configurar Firestore (opcional):**
   - Si quieres usar Firestore en lugar de InMemory
   - Seguir documentación en `FIRESTORE_SETUP.md`

---

**Fin del documento de pruebas**
**Versión 1.0 - Sistema Cinema**
