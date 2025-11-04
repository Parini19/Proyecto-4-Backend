# Cinema Management System - Documentación Completa

**Última actualización:** 2025-11-03

---

## 📚 Índice de Documentación

Bienvenido a la documentación completa del Cinema Management System. Esta carpeta contiene toda la información necesaria para entender, desarrollar y mantener el sistema.

---

### 📖 Documentos Principales

| Documento | Descripción | Audiencia |
|-----------|-------------|-----------|
| **[00-PROJECT-OVERVIEW.md](./00-PROJECT-OVERVIEW.md)** | Visión general del proyecto, stack tecnológico, arquitectura general | Todos |
| **[01-WORK-PLAN.md](./01-WORK-PLAN.md)** | Plan de trabajo detallado por fases, tareas y prioridades | Equipo de desarrollo, PM |
| **[02-BACKEND-ARCHITECTURE.md](./02-BACKEND-ARCHITECTURE.md)** | Arquitectura del backend (.NET), entidades, repositorios, servicios | Backend developers |
| **[03-FRONTEND-ARCHITECTURE.md](../Cinema%20Frontend/Proyecto-4-Frontend/docs/03-FRONTEND-ARCHITECTURE.md)** | Arquitectura del frontend (Flutter), state management, routing | Frontend developers |
| **[04-API-DOCUMENTATION.md](./04-API-DOCUMENTATION.md)** | Documentación completa de endpoints, modelos de datos, ejemplos | Todos los developers |

---

## 🚀 Quick Start

### Para Nuevos Desarrolladores

1. **Leer primero:** [00-PROJECT-OVERVIEW.md](./00-PROJECT-OVERVIEW.md)
   - Entiende qué es el proyecto y su arquitectura general

2. **Configurar entorno:**
   - Backend: Ver sección de configuración en [02-BACKEND-ARCHITECTURE.md](./02-BACKEND-ARCHITECTURE.md)
   - Frontend: Ver sección de plataformas en [03-FRONTEND-ARCHITECTURE.md](../Cinema%20Frontend/Proyecto-4-Frontend/docs/03-FRONTEND-ARCHITECTURE.md)

3. **Revisar plan de trabajo:** [01-WORK-PLAN.md](./01-WORK-PLAN.md)
   - Identifica en qué fase estamos
   - Revisa las tareas pendientes

4. **Consultar API:** [04-API-DOCUMENTATION.md](./04-API-DOCUMENTATION.md)
   - Referencia de todos los endpoints disponibles

---

## 🏗️ Estructura del Proyecto

### Backend (.NET 9.0)
```
C:\Users\Guillermo Parini\Documents\Cinema/
├── src/
│   ├── Cinema.Api/              # Capa de presentación (Controllers)
│   ├── Cinema.Application/      # Capa de aplicación (Interfaces)
│   ├── Cinema.Domain/           # Capa de dominio (Entidades)
│   └── Cinema.Infrastructure/   # Capa de infraestructura (Repositories)
├── docs/                        # Esta carpeta
└── Cinema.sln
```

### Frontend (Flutter 3.35.4)
```
C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend/
├── lib/
│   ├── core/                    # Dominio y servicios
│   │   ├── entities/            # Modelos de datos
│   │   └── services/            # Lógica de negocio
│   ├── features/                # UI por feature
│   ├── main.dart                # Entry point
│   └── app.dart                 # Main app widget
├── docs/                        # Documentación frontend
└── pubspec.yaml
```

---

## 📋 Estado Actual del Proyecto

### ✅ Completado

#### Backend
- [x] Estructura Clean Architecture
- [x] Integración con Firebase Auth
- [x] Integración con Firestore
- [x] User management (CRUD completo)
- [x] Movie retrieval (GET)
- [x] JWT authentication
- [x] Logging con Serilog
- [x] Swagger documentation
- [x] CORS para Flutter

#### Frontend
- [x] Estructura Clean Architecture
- [x] Login page con validación
- [x] Admin dashboard con route guards
- [x] Movie picker/cartelera
- [x] Seat selection
- [x] Food ordering page
- [x] UserService para autenticación
- [x] ApiClient con Dio
- [x] Session management

### ⚠️ En Progreso

#### Backend
- [ ] Movies CRUD completo (solo GET implementado)
- [ ] Screenings CRUD (endpoints definidos, lógica pendiente)
- [ ] Theater Rooms CRUD (endpoints definidos, lógica pendiente)
- [ ] Food Combos CRUD (endpoints definidos, lógica pendiente)
- [ ] Food Orders CRUD (endpoints definidos, lógica pendiente)

#### Frontend
- [ ] Migración completa a Riverpod
- [ ] Migración a GoRouter
- [ ] Integración con API para datos dinámicos
- [ ] Persistencia de sesión (secure storage)

### 🔜 Próximos Pasos

Ver [01-WORK-PLAN.md](./01-WORK-PLAN.md) para el plan detallado por fases.

**Prioridad Inmediata:**
1. Completar Movies CRUD (Backend + Frontend)
2. Implementar Screenings management
3. Implementar Theater Rooms management
4. Implementar Bookings & Seats

---

## 🔧 Tecnologías Principales

### Backend
- **Framework:** ASP.NET Core 9.0
- **Lenguaje:** C# 12
- **Base de Datos:** Cloud Firestore
- **Autenticación:** Firebase Authentication + JWT
- **Logging:** Serilog
- **Documentation:** Swagger/OpenAPI

### Frontend
- **Framework:** Flutter 3.35.4
- **Lenguaje:** Dart 3.9.2+
- **State Management:** Riverpod (preparado)
- **Routing:** GoRouter (preparado), MaterialApp (actual)
- **HTTP Client:** Dio
- **Plataformas:** Web, Android, iOS

### Firebase Services
- **Authentication:** Gestión de usuarios
- **Firestore:** Base de datos NoSQL
- **Hosting:** Web deployment (futuro)

---

## 📊 Diagramas y Arquitectura

### Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│                     FLUTTER FRONTEND                         │
│  (Web, Android, iOS)                                        │
│                                                             │
│  Features: Auth, Movies, Bookings, Food Orders, Admin      │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ HTTPS/REST API
                            │
┌─────────────────────────────────────────────────────────────┐
│                   .NET BACKEND API                           │
│                                                             │
│  Clean Architecture: API → Application → Domain             │
│                          ↓                                  │
│                   Infrastructure (Firestore)                │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ Firebase SDK
                            │
┌─────────────────────────────────────────────────────────────┐
│                   FIREBASE SERVICES                          │
│                                                             │
│  Authentication | Firestore | Storage (futuro)             │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔐 Seguridad

### Autenticación
- JWT tokens con expiración de 60 minutos
- Firebase Authentication como fuente de verdad
- Roles: `admin`, `user`

### Autorización
- Endpoints protegidos con `[Authorize]` attribute
- Role-based access control (RBAC)
- Route guards en frontend para admin

### Mejores Prácticas
- **NUNCA** commitear credenciales (usar `.gitignore`)
- Hashear passwords (⚠️ TODO: implementar bcrypt)
- HTTPS en producción
- Validar todos los inputs
- Sanitizar datos antes de guardar

---

## 🧪 Testing

### Backend
- **Unit Tests:** Repositories y Controllers
- **Integration Tests:** Con Firestore Emulator
- **Load Tests:** Reservas concurrentes

### Frontend
- **Widget Tests:** Componentes UI
- **Integration Tests:** Flujos completos
- **E2E Tests:** Flutter Driver

**Estado Actual:** Framework configurado, tests pendientes de implementar

---

## 🚀 Deployment

### Backend
- **Desarrollo:** `https://localhost:7238`
- **Producción:** TBD (Azure, AWS, Google Cloud Run)
- **Dockerización:** Pendiente

### Frontend
- **Web:** Firebase Hosting (recomendado)
- **Android:** Google Play Store
- **iOS:** Apple App Store (opcional)

---

## 📝 Convenciones de Código

### Backend (C#)
- StyleCop enforced
- PascalCase para métodos y propiedades públicas
- camelCase para variables locales
- Async/await para operaciones I/O
- Dependency Injection para servicios

### Frontend (Dart)
- flutter_lints enforced
- camelCase para variables y métodos
- PascalCase para clases
- Widgets en archivos separados
- Usar `const` constructors donde sea posible

---

## 🔄 Git Workflow

### Branches
- `main`: Producción
- `develop`: Desarrollo (próxima release)
- `feature/*`: Nuevas features
- `bugfix/*`: Correcciones de bugs
- `hotfix/*`: Correcciones urgentes en producción

### Commits
Usar Conventional Commits:
```
feat: Add movie CRUD endpoints
fix: Correct login validation
docs: Update API documentation
refactor: Improve repository pattern
test: Add unit tests for UserService
```

### Pull Requests
- Requieren code review
- Deben pasar CI/CD (tests y linting)
- Descripción clara de cambios
- Referenciar issue relacionado

---

## 📞 Contacto y Soporte

### Recursos
- **Swagger API:** https://localhost:7238/swagger
- **Backend Repo:** `C:\Users\Guillermo Parini\Documents\Cinema`
- **Frontend Repo:** `C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend`

### Para Reportar Issues
1. Verificar si ya existe el issue
2. Proveer pasos para reproducir
3. Incluir logs relevantes
4. Especificar entorno (dev, prod, web, mobile)

---

## 📚 Recursos Adicionales

### Documentación Externa
- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [Flutter Docs](https://docs.flutter.dev)
- [Firebase Docs](https://firebase.google.com/docs)
- [Firestore Docs](https://firebase.google.com/docs/firestore)
- [Clean Architecture (Uncle Bob)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

### Tutoriales Recomendados
- [.NET Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [Flutter Clean Architecture](https://resocoder.com/flutter-clean-architecture-tdd/)
- [Riverpod Documentation](https://riverpod.dev)

---

## 🎯 Objetivos del Proyecto

### Objetivos de Negocio
- Modernizar la experiencia de compra de boletos de cine
- Reducir tiempos de espera en taquilla
- Incrementar ventas de alimentos con pre-orders
- Proveer dashboard administrativo eficiente

### Objetivos Técnicos
- Implementar arquitectura escalable y mantenible
- Soporte multi-plataforma (Web, Android, iOS)
- Performance óptimo (< 2s load time)
- Alta disponibilidad (99.9% uptime)
- Seguridad robusta

---

## 📅 Timeline

| Fase | Duración | Hitos |
|------|----------|-------|
| **Fase 1** | 2-3 semanas | CRUD de entidades core |
| **Fase 2** | 1-2 semanas | Food Orders + Mock Payments |
| **Fase 3** | 2-3 semanas | UX improvements + Advanced features |
| **Fase 4** | 1-2 semanas | Admin Dashboard completo |
| **Fase 5** | 1-2 semanas | Testing, Optimization, Deployment |
| **TOTAL** | **7-12 semanas** | MVP listo para producción |

---

## ✅ Checklist de Configuración Inicial

### Backend
- [ ] Instalar .NET 9.0 SDK
- [ ] Clonar repositorio
- [ ] Configurar Firebase Service Account JSON
- [ ] Actualizar `appsettings.json` con credenciales
- [ ] Ejecutar `dotnet restore`
- [ ] Ejecutar `dotnet run --project src/Cinema.Api`
- [ ] Verificar Swagger en https://localhost:7238/swagger

### Frontend
- [ ] Instalar Flutter 3.35.4+
- [ ] Clonar repositorio
- [ ] Ejecutar `flutter pub get`
- [ ] Configurar API_BASE_URL
- [ ] Ejecutar `flutter run -d chrome --web-port=5173`
- [ ] Verificar que conecta con backend

### Firebase
- [ ] Crear proyecto en Firebase Console
- [ ] Habilitar Authentication (Email/Password)
- [ ] Crear base de datos Firestore
- [ ] Descargar Service Account JSON
- [ ] Configurar reglas de seguridad Firestore

---

## 🔍 Troubleshooting Común

### Backend no conecta con Firestore
1. Verificar que `GOOGLE_APPLICATION_CREDENTIALS` está configurado
2. Verificar que el Service Account JSON es válido
3. Verificar `Firebase:Enabled = true` en appsettings

### Frontend no puede hacer login
1. Verificar que backend está corriendo
2. Verificar CORS configurado correctamente
3. Verificar API_BASE_URL apunta al backend correcto
4. Revisar consola del navegador para errores

### Tests fallan
1. Verificar que todas las dependencias están instaladas
2. Ejecutar `dotnet restore` / `flutter pub get`
3. Verificar que no hay puertos en uso
4. Revisar logs para errores específicos

---

## 📈 Métricas y Monitoreo

### Backend
- **Logging:** Serilog → archivos en `logs/`
- **Metrics:** Application Insights (futuro)
- **Monitoring:** (TBD)

### Frontend
- **Crash Reporting:** Firebase Crashlytics (futuro)
- **Analytics:** Firebase Analytics (futuro)

---

## 🎓 Glosario

| Término | Definición |
|---------|------------|
| **CRUD** | Create, Read, Update, Delete |
| **JWT** | JSON Web Token (autenticación) |
| **Firestore** | Base de datos NoSQL de Firebase |
| **Clean Architecture** | Patrón arquitectónico en capas |
| **Repository Pattern** | Abstracción de acceso a datos |
| **Riverpod** | State management para Flutter |
| **GoRouter** | Routing package para Flutter |
| **Dio** | HTTP client para Dart/Flutter |
| **DTO** | Data Transfer Object |

---

## 📄 Licencia

(Especificar licencia del proyecto)

---

## 👥 Contribuidores

- **Equipo de Desarrollo Cinema System**
- Última documentación generada por: Claude Code

---

**Nota:** Esta documentación está en constante actualización. Mantener sincronizada con los cambios del proyecto.

---

## 🔗 Links Rápidos

- [Project Overview](./00-PROJECT-OVERVIEW.md)
- [Work Plan](./01-WORK-PLAN.md)
- [Backend Architecture](./02-BACKEND-ARCHITECTURE.md)
- [Frontend Architecture](../Cinema%20Frontend/Proyecto-4-Frontend/docs/03-FRONTEND-ARCHITECTURE.md)
- [API Documentation](./04-API-DOCUMENTATION.md)
- [Swagger UI](https://localhost:7238/swagger)

---

**¿Necesitas ayuda?** Consulta la documentación específica según tu rol o área de trabajo.
