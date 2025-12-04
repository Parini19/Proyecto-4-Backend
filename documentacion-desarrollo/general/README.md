# Documentación General - Cinema Project

Documentación que aplica a todo el proyecto (Backend + Frontend).

---

## 📚 Documentos Disponibles

### 🚀 Inicio Rápido

**[SETUP.md](./SETUP.md)**
- Configuración inicial del proyecto
- Requisitos del sistema
- Instalación de dependencias
- Configuración de variables de entorno

**[INSTRUCCIONES-CORRER-PROYECTO.md](./INSTRUCCIONES-CORRER-PROYECTO.md)**
- Cómo ejecutar el proyecto completo
- Backend + Frontend simultáneamente
- Orden de ejecución
- Verificación de funcionamiento

**[INSTRUCCIONES_EJECUCION.md](./INSTRUCCIONES_EJECUCION.md)**
- Instrucciones detalladas de ejecución
- Troubleshooting común
- Tips de desarrollo

**[README-DEVELOPERS.md](./README-DEVELOPERS.md)**
- Guía para nuevos desarrolladores
- Arquitectura general del proyecto
- Convenciones de código
- Mejores prácticas

---

### ⚙️ Configuración

**[CONFIGURACION-PUERTOS.md](./CONFIGURACION-PUERTOS.md)**
- Configuración de puertos del sistema
- Backend: 7238 (HTTPS)
- Frontend: 5173 (Vite/Web)
- Conflictos de puertos

**[PUERTOS-Y-CONFIGURACION.md](./PUERTOS-Y-CONFIGURACION.md)**
- Detalles adicionales de puertos
- Variables de entorno
- Configuración de Firestore
- Configuración de Cloudinary

---

### 🧪 Testing

**[TESTING_GUIDE.md](./TESTING_GUIDE.md)**
- Guía completa de testing
- Tests unitarios (Backend)
- Tests de integración
- Tests de UI (Frontend)
- Tests end-to-end
- Colecciones de Postman/Insomnia
- Checklist de testing manual

**[QUICK_TEST_CHECKLIST.md](./QUICK_TEST_CHECKLIST.md)**
- Checklist rápido de pruebas
- Smoke tests
- Regression tests
- Verificación antes de commits

---

### 📝 Historial y Cambios

**[CAMBIOS-REALIZADOS.md](./CAMBIOS-REALIZADOS.md)**
- Registro histórico de cambios
- Features implementadas
- Bugs corregidos
- Refactorings

**[COMO-APLICAR-CAMBIOS.md](./COMO-APLICAR-CAMBIOS.md)**
- Cómo aplicar cambios al proyecto
- Proceso de desarrollo
- Git workflow
- Pull requests

**[RESUMEN_IMPLEMENTACIONES.md](./RESUMEN_IMPLEMENTACIONES.md)**
- Resumen de todas las implementaciones
- Estado actual del proyecto
- Roadmap

**[ULTIMA-TAREA.md](./ULTIMA-TAREA.md)**
- Última tarea realizada
- Contexto de trabajo reciente
- Próximos pasos

---

## 🏗️ Arquitectura General

### Stack Completo

```
┌─────────────────────────────────────────┐
│         Frontend (Flutter/Dart)         │
│     - Riverpod (State Management)       │
│     - HTTP Client                       │
│     - Material Design                   │
└──────────────┬──────────────────────────┘
               │ REST API (HTTPS)
               │ Port: 7238
┌──────────────▼──────────────────────────┐
│        Backend (.NET 9 API)             │
│     - ASP.NET Core                      │
│     - Controllers + Services            │
└──────────────┬──────────────────────────┘
               │
        ┌──────┴──────┐
        ▼             ▼
┌──────────────┐ ┌──────────────┐
│   Firestore  │ │  Cloudinary  │
│   (NoSQL DB) │ │   (Images)   │
└──────────────┘ └──────────────┘
```

### Flujo de Datos

1. **Usuario** interactúa con **Frontend Flutter**
2. **Frontend** hace request HTTP a **Backend API**
3. **Backend** procesa lógica de negocio
4. **Backend** consulta/escribe en **Firestore**
5. **Backend** gestiona imágenes en **Cloudinary**
6. **Backend** retorna respuesta JSON
7. **Frontend** actualiza UI con datos

---

## 🚀 Inicio Rápido

### 1. Clonar el Repositorio
```bash
git clone <repo-url>
cd Cinema
```

### 2. Configurar Backend
```bash
cd src/Cinema.Api
dotnet restore
# Configurar appsettings.json con credenciales
dotnet run --urls="https://localhost:7238"
```

### 3. Configurar Frontend
```bash
# En otra terminal
flutter pub get
flutter run -d chrome
```

### 4. Poblar Base de Datos
```bash
cd database-seeding
./seed-database.ps1  # Windows
# o
./seed-database.sh   # Linux/Mac
```

---

## 🔧 Requisitos del Sistema

### Backend
- **.NET 9 SDK** o superior
- **Visual Studio 2022** o **VS Code**
- **Firestore** configurado
- **Cloudinary** account

### Frontend
- **Flutter 3.x** o superior
- **Dart 3.x** o superior
- **Chrome** (para desarrollo web)
- **Android Studio** (para Android)
- **Xcode** (para iOS - solo macOS)

### General
- **Git**
- **Node.js** (opcional, para herramientas)

---

## 🌐 Puertos Utilizados

| Servicio | Puerto | Protocolo | URL |
|----------|--------|-----------|-----|
| Backend API | 7238 | HTTPS | https://localhost:7238 |
| Frontend Web | 5173 | HTTP | http://localhost:5173 |
| Firestore | - | HTTPS | Firebase Cloud |
| Cloudinary | - | HTTPS | Cloudinary Cloud |

---

## 📂 Estructura del Proyecto

```
Cinema/
├── src/                              ← Backend (.NET)
│   └── Cinema.Api/
│       ├── Controllers/
│       ├── Models/
│       ├── Services/
│       └── Program.cs
├── lib/                              ← Frontend (Flutter)
│   ├── core/
│   └── features/
├── docs/                             ← Documentación técnica
├── database-seeding/                 ← Scripts de seeding
├── documentacion-desarrollo/         ← Esta carpeta
│   ├── backend/                      ← Docs del backend
│   ├── frontend/                     ← Docs del frontend
│   └── general/                      ← Docs generales (esta carpeta)
└── README.md                         ← README principal
```

---

## 🔑 Configuración de Credenciales

### Backend: appsettings.json
```json
{
  "FirebaseConfig": {
    "ProjectId": "cinema-project-id",
    "CredentialsPath": "./serviceAccountKey.json"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### Frontend: lib/core/config/env.dart
```dart
class Environment {
  static const String apiBaseUrl = 'https://localhost:7238/api';
  static const String cloudinaryCloudName = 'your-cloud-name';
}
```

---

## 📖 Documentación Adicional

- **Backend**: Ver [../backend/README.md](../backend/README.md)
- **Frontend**: Ver [../frontend/README.md](../frontend/README.md)
- **Arquitectura**: Ver [/docs/02-BACKEND-ARCHITECTURE.md](../../docs/02-BACKEND-ARCHITECTURE.md)
- **Seeding**: Ver [/database-seeding/README.md](../../database-seeding/README.md)

---

## 🛠️ Comandos Útiles

### Backend
```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run --urls="https://localhost:7238"

# Tests
dotnet test
```

### Frontend
```bash
# Instalar dependencias
flutter pub get

# Ejecutar (web)
flutter run -d chrome

# Compilar (web)
flutter build web

# Tests
flutter test

# Análisis de código
flutter analyze
```

### Git
```bash
# Status
git status

# Crear branch
git checkout -b feature/nueva-feature

# Commit
git add .
git commit -m "feat: descripción del cambio"

# Push
git push origin feature/nueva-feature
```

---

## 🐛 Troubleshooting Común

### Puerto 7238 ocupado
```bash
# Windows
netstat -ano | findstr :7238
taskkill /PID <PID> /F

# Linux/Mac
lsof -ti:7238 | xargs kill -9
```

### Firestore connection error
- Verifica que `serviceAccountKey.json` esté en la ubicación correcta
- Verifica que el ProjectId sea correcto
- Verifica permisos en Firebase Console

### Flutter doctor issues
```bash
flutter doctor -v
# Sigue las recomendaciones
```

---

**Última actualización**: Noviembre 26, 2025
