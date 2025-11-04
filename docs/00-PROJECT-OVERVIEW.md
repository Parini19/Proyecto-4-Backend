# Cinema Management System - Project Overview

**Última actualización:** 2025-11-03

## Descripción del Sistema

Sistema integral de gestión de cines que abarca dos áreas principales:

### 1. **Customer Area (Área de Clientes)**
- Consulta de cartelera de películas
- Compra de boletos en línea
- Selección de asientos
- Compra de alimentos y combos
- Gestión de perfil de usuario

### 2. **Admin Area (Área de Administración)**
- Gestión de películas (CRUD)
- Gestión de salas de cine (CRUD)
- Gestión de horarios y proyecciones (CRUD)
- Gestión de usuarios
- Gestión de combos de alimentos (CRUD)
- Gestión de órdenes de comida
- Reportes y estadísticas

---

## Stack Tecnológico

### Backend
- **.NET 9.0** con ASP.NET Core Web API
- **Clean Architecture** (4 capas: API, Application, Domain, Infrastructure)
- **Firebase Authentication** para autenticación de usuarios
- **Cloud Firestore** como base de datos NoSQL
- **JWT Tokens** para autorización
- **Serilog** para logging estructurado
- **Swagger/OpenAPI** para documentación de API
- **Feature Flags** con Microsoft.FeatureManagement

### Frontend
- **Flutter 3.35.4** con Dart 3.9.2+
- **Clean Architecture** con separación de capas (Core, Features)
- **Riverpod** para state management (preparado para expansión)
- **GoRouter** para navegación (preparado, parcialmente implementado)
- **Dio** como HTTP client
- **Material Design 3** con tema oscuro
- **Soporte Multi-plataforma:** Web (primario), Android, iOS

### Firebase Services
- **Firebase Authentication** - Gestión de usuarios
- **Cloud Firestore** - Base de datos en tiempo real
- **Firebase Admin SDK** - Operaciones administrativas

---

## Arquitectura del Sistema

```
┌─────────────────────────────────────────────────────────────┐
│                     FLUTTER FRONTEND                         │
│  (Web, Android, iOS)                                        │
│                                                             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  Login   │  │  Movies  │  │  Seats   │  │   Food   │  │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘  │
│                                                             │
│  ┌──────────────────────────────────────────────────┐     │
│  │          Core Services Layer                      │     │
│  │  (UserService, ApiClient, Entities)              │     │
│  └──────────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ HTTPS/REST API
                            │
┌─────────────────────────────────────────────────────────────┐
│                   .NET BACKEND API                           │
│                                                             │
│  ┌──────────────────────────────────────────────────┐     │
│  │         Controllers Layer                         │     │
│  │  (Auth, Movies, Screenings, Food, Users)         │     │
│  └──────────────────────────────────────────────────┘     │
│                            │                               │
│  ┌──────────────────────────────────────────────────┐     │
│  │      Application Layer (Interfaces)               │     │
│  └──────────────────────────────────────────────────┘     │
│                            │                               │
│  ┌──────────────────────────────────────────────────┐     │
│  │         Domain Layer (Entities)                   │     │
│  └──────────────────────────────────────────────────┘     │
│                            │                               │
│  ┌──────────────────────────────────────────────────┐     │
│  │    Infrastructure Layer (Repositories)            │     │
│  └──────────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ Firebase SDK
                            │
┌─────────────────────────────────────────────────────────────┐
│                   FIREBASE SERVICES                          │
│                                                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐    │
│  │ Authentication│  │  Firestore   │  │   Storage    │    │
│  │     Users     │  │  Database    │  │   (Future)   │    │
│  └──────────────┘  └──────────────┘  └──────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

---

## Estado Actual del Proyecto

### ✅ Backend - Implementado
- [x] Estructura Clean Architecture completa
- [x] Integración con Firebase Authentication
- [x] Integración con Cloud Firestore
- [x] JWT Token generation
- [x] User management (CRUD completo)
- [x] Movie retrieval (GET)
- [x] Logging con Serilog
- [x] Swagger documentation
- [x] CORS configurado para Flutter
- [x] Feature Flags
- [x] Audit Middleware

### ⚠️ Backend - Parcialmente Implementado
- [ ] Movies CRUD (solo GET implementado)
- [ ] Screenings CRUD (endpoints creados, lógica pendiente)
- [ ] Theater Rooms CRUD (endpoints creados, lógica pendiente)
- [ ] Food Combos CRUD (endpoints creados, lógica pendiente)
- [ ] Food Orders CRUD (endpoints creados, lógica pendiente)

### ✅ Frontend - Implementado
- [x] Clean Architecture base
- [x] Login page con validación
- [x] Admin dashboard con protección de rutas
- [x] Movie picker/cartelera
- [x] Seat selection con grid interactivo
- [x] Food ordering page
- [x] About Us page
- [x] UserService para autenticación
- [x] ApiClient con Dio
- [x] Sesión de usuario global (Singleton)
- [x] Routing básico con MaterialApp
- [x] CI/CD con GitHub Actions

### ⚠️ Frontend - Preparado pero no Activo
- [ ] Riverpod para state management global
- [ ] GoRouter para navegación avanzada
- [ ] Integración directa con Firebase SDK
- [ ] Persistencia de sesión (token storage)
- [ ] Integración con API para datos dinámicos

---

## Colecciones de Firestore

| Colección | Propósito | Estado |
|-----------|-----------|--------|
| `users` | Perfiles de usuario | ✅ Implementado |
| `movies` | Catálogo de películas | ⚠️ Parcial |
| `screenings` | Horarios de proyecciones | ❌ Pendiente |
| `theaterRooms` | Salas de cine | ❌ Pendiente |
| `foodCombos` | Combos de alimentos | ❌ Pendiente |
| `foodOrders` | Órdenes de comida | ❌ Pendiente |
| `bookings` | Reservas de boletos | ❌ Pendiente |
| `seats` | Asientos por sala | ❌ Pendiente |

---

## Roles de Usuario

| Rol | Permisos | Acceso |
|-----|----------|--------|
| **Admin** | Gestión completa del sistema | Admin Dashboard |
| **User** | Compra de boletos, alimentos | Customer Area |
| **Guest** | Solo visualización de cartelera | Limitado |

---

## Endpoints Principales

### Autenticación
- `POST /api/FirebaseTest/login` - Login con email/password
- `GET /api/me` - Obtener usuario autenticado

### Películas
- `GET /api/movies` - Listar películas
- `POST /api/movies/add-movie` - Agregar película (TODO)
- `PUT /api/movies/edit-movie/{id}` - Editar película (TODO)
- `DELETE /api/movies/delete-movie/{id}` - Eliminar película (TODO)

### Usuarios
- `GET /api/FirebaseTest/get-all-users` - Listar usuarios
- `POST /api/FirebaseTest/add-user` - Agregar usuario
- `PUT /api/FirebaseTest/edit-user/{uid}` - Editar usuario
- `DELETE /api/FirebaseTest/delete-user/{uid}` - Eliminar usuario

### Proyecciones (Screenings)
- Endpoints definidos pero sin implementación (TODO)

### Salas de Cine (Theater Rooms)
- Endpoints definidos pero sin implementación (TODO)

### Combos de Alimentos
- Endpoints definidos pero sin implementación (TODO)

### Órdenes de Comida
- Endpoints definidos pero sin implementación (TODO)

---

## Configuración de Desarrollo

### Backend
```
URL: https://localhost:7238
Swagger: https://localhost:7238/swagger
Health Check: https://localhost:7238/health
```

### Frontend
```
Web Port: 5173 (Chrome)
API Base URL: https://localhost:7238
Platform: Web (primario), Android, iOS
```

---

## Próximos Pasos

Ver `01-WORK-PLAN.md` para el plan de trabajo detallado.

---

## Estructura de Documentación

```
docs/
├── 00-PROJECT-OVERVIEW.md           # Este documento
├── 01-WORK-PLAN.md                  # Plan de trabajo por fases
├── 02-BACKEND-ARCHITECTURE.md       # Arquitectura del backend
├── 03-FRONTEND-ARCHITECTURE.md      # Arquitectura del frontend
├── 04-API-DOCUMENTATION.md          # Documentación de APIs
├── 05-DATABASE-SCHEMA.md            # Esquema de Firestore
├── 06-AUTHENTICATION-FLOW.md        # Flujo de autenticación
└── 07-DEPLOYMENT-GUIDE.md           # Guía de despliegue
```

---

## Contacto y Recursos

- **Backend Repository:** `C:\Users\Guillermo Parini\Documents\Cinema`
- **Frontend Repository:** `C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend`
- **Firebase Console:** (Configurar URL)
- **Swagger Docs:** https://localhost:7238/swagger

---

**Notas:**
- El sistema está en fase de desarrollo activo
- Se sigue Clean Architecture en ambos proyectos
- Firebase es la fuente de verdad para autenticación y datos
- El backend actúa como intermediario entre Flutter y Firebase
