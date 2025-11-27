# Documentación para Desarrollo - Cinema

Esta carpeta contiene toda la documentación técnica y guías de desarrollo del proyecto Cinema, organizada por área.

---

## 📁 Estructura de Documentación

```
documentacion-desarrollo/
├── backend/              ← Documentación específica del Backend (.NET API)
├── frontend/             ← Documentación específica del Frontend (Flutter)
└── general/              ← Documentación general del proyecto
```

---

## 🔧 Backend (.NET API)

Documentación del backend en **[backend/](./backend/)**:

- **[API_COLLECTION.md](./backend/API_COLLECTION.md)** - Colección completa de endpoints del API REST
- **[PAYMENT_BILLING_SYSTEM_DOCS.md](./backend/PAYMENT_BILLING_SYSTEM_DOCS.md)** - Sistema de pagos y facturación
- **[CLOUDINARY_UPLOAD_GUIDE.md](./backend/CLOUDINARY_UPLOAD_GUIDE.md)** - Guía para cargar imágenes a Cloudinary
- **[get_cloudinary_urls.md](./backend/get_cloudinary_urls.md)** - Script para obtener URLs de Cloudinary

---

## 🎨 Frontend (Flutter)

Documentación del frontend en **[frontend/](./frontend/)**:

- **[FRONTEND_INTEGRATION_STATUS.md](./frontend/FRONTEND_INTEGRATION_STATUS.md)** - Estado de integración frontend-backend

---

## 📋 Documentación General

Documentación que aplica a todo el proyecto en **[general/](./general/)**:

### 🚀 Inicio Rápido
- **[SETUP.md](./general/SETUP.md)** - Configuración inicial del proyecto
- **[INSTRUCCIONES-CORRER-PROYECTO.md](./general/INSTRUCCIONES-CORRER-PROYECTO.md)** - Cómo ejecutar el proyecto completo
- **[INSTRUCCIONES_EJECUCION.md](./general/INSTRUCCIONES_EJECUCION.md)** - Instrucciones detalladas de ejecución
- **[README-DEVELOPERS.md](./general/README-DEVELOPERS.md)** - Guía para desarrolladores

### ⚙️ Configuración
- **[CONFIGURACION-PUERTOS.md](./general/CONFIGURACION-PUERTOS.md)** - Configuración de puertos del sistema
- **[PUERTOS-Y-CONFIGURACION.md](./general/PUERTOS-Y-CONFIGURACION.md)** - Detalles de puertos y configuración

### 🧪 Testing
- **[TESTING_GUIDE.md](./general/TESTING_GUIDE.md)** - Guía completa de testing
- **[QUICK_TEST_CHECKLIST.md](./general/QUICK_TEST_CHECKLIST.md)** - Checklist rápido de pruebas

### 📝 Historial y Cambios
- **[CAMBIOS-REALIZADOS.md](./general/CAMBIOS-REALIZADOS.md)** - Registro de cambios realizados
- **[COMO-APLICAR-CAMBIOS.md](./general/COMO-APLICAR-CAMBIOS.md)** - Cómo aplicar cambios al proyecto
- **[RESUMEN_IMPLEMENTACIONES.md](./general/RESUMEN_IMPLEMENTACIONES.md)** - Resumen de implementaciones
- **[ULTIMA-TAREA.md](./general/ULTIMA-TAREA.md)** - Última tarea realizada

---

## 📂 Otras Carpetas de Documentación

### `/docs/`
Documentación técnica y arquitectónica del proyecto:
- 00-PROJECT-OVERVIEW.md - Visión general
- 01-WORK-PLAN.md - Plan de trabajo
- 02-BACKEND-ARCHITECTURE.md - Arquitectura del backend
- 04-API-DOCUMENTATION.md - Documentación del API
- BACKEND-STATUS-REVIEW.md - Revisión del backend
- RESUMEN-EJECUTIVO.md - Resumen ejecutivo
- STRATEGIC-RECOMMENDATIONS.md - Recomendaciones estratégicas

### `/database-seeding/`
Scripts y documentación para inicializar la base de datos:
- seed-database.ps1 - Script PowerShell
- seed-database.sh - Script Bash
- SCREENINGS_SETUP.md - Sistema de funciones y salas
- README-SEEDING.md - Guía de seeding
- EJECUTAR-SEEDING.md - Instrucciones de ejecución

---

## 🎯 Guía de Inicio Rápido

Para comenzar a trabajar en el proyecto, sigue este orden:

### 1. Backend Developer
1. [general/SETUP.md](./general/SETUP.md) - Configuración inicial
2. [backend/API_COLLECTION.md](./backend/API_COLLECTION.md) - Endpoints del API
3. [backend/PAYMENT_BILLING_SYSTEM_DOCS.md](./backend/PAYMENT_BILLING_SYSTEM_DOCS.md) - Sistema de pagos
4. [general/TESTING_GUIDE.md](./general/TESTING_GUIDE.md) - Testing

### 2. Frontend Developer
1. [general/SETUP.md](./general/SETUP.md) - Configuración inicial
2. [frontend/FRONTEND_INTEGRATION_STATUS.md](./frontend/FRONTEND_INTEGRATION_STATUS.md) - Estado de integración
3. [general/INSTRUCCIONES-CORRER-PROYECTO.md](./general/INSTRUCCIONES-CORRER-PROYECTO.md) - Ejecutar el proyecto
4. [general/TESTING_GUIDE.md](./general/TESTING_GUIDE.md) - Testing

### 3. Full Stack Developer
1. [general/README-DEVELOPERS.md](./general/README-DEVELOPERS.md) - Guía general
2. [general/INSTRUCCIONES-CORRER-PROYECTO.md](./general/INSTRUCCIONES-CORRER-PROYECTO.md) - Ejecutar todo
3. [backend/API_COLLECTION.md](./backend/API_COLLECTION.md) - Entender el API
4. [frontend/FRONTEND_INTEGRATION_STATUS.md](./frontend/FRONTEND_INTEGRATION_STATUS.md) - Integración

---

## 📌 Notas

- **Backend**: Documentación en [backend/](./backend/) - C#/.NET 9, Firestore, REST API
- **Frontend**: Documentación en [frontend/](./frontend/) - Flutter/Dart, Riverpod, Material Design
- **General**: Documentación en [general/](./general/) - Setup, testing, configuración
- Algunos documentos pueden estar desactualizados - verifica la fecha de última modificación
- Para poblar la base de datos, ver carpeta `/database-seeding/` en la raíz del proyecto

---

**Última actualización**: Noviembre 26, 2025
