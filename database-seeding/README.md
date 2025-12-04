# Database Seeding Scripts

Esta carpeta contiene scripts y documentación para poblar la base de datos de Firestore con datos iniciales.

## ⚠️ Importante

**Estos scripts ya fueron ejecutados el 26 de noviembre de 2025.**

La base de datos ya está poblada con:
- 20 salas de cine (15 normales + 5 VIP)
- 267 funciones para los próximos 7 días
- Solo películas "En Cartelera" y "Más Populares"

**El sistema está en producción. Usa el Panel de Admin para gestionar datos.**

---

## Archivos en esta Carpeta

### Scripts de Ejecución
- **seed-database.ps1** - Script PowerShell para Windows
- **seed-database.sh** - Script Bash para Linux/Mac

### Documentación
- **EJECUTAR-SEEDING.md** - Instrucciones detalladas en español
- **README-SEEDING.md** - Guía rápida de uso
- **SCREENINGS_SETUP.md** - Documentación completa del sistema

---

## ¿Cuándo Usar Estos Scripts?

**Solo usa estos scripts si necesitas:**
1. Resetear completamente la base de datos
2. Recrear datos de prueba desde cero
3. Configurar un nuevo ambiente de desarrollo

**NO uses estos scripts en producción normal.** El administrador debe gestionar datos desde:
- Panel de Admin: `http://localhost:5173/admin`

---

## Cómo Ejecutar (Solo si es necesario)

### Requisito: API corriendo
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
dotnet run --urls="https://localhost:7238"
```

### Opción 1: Script Automático (Windows)
```powershell
cd "C:\Users\Guillermo Parini\Documents\Cinema\database-seeding"
.\seed-database.ps1
```

### Opción 2: Script Automático (Linux/Mac)
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema\database-seeding"
chmod +x seed-database.sh
./seed-database.sh
```

### Opción 3: Manual (PowerShell)
```powershell
# Crear salas
Invoke-WebRequest -Uri "https://localhost:7238/api/theaterrooms/seed?clearExisting=true" `
    -Method POST -SkipCertificateCheck

# Crear funciones
Invoke-WebRequest -Uri "https://localhost:7238/api/screenings/seed?clearExisting=true" `
    -Method POST -SkipCertificateCheck
```

---

## Endpoints del Backend

Los controllers tienen estos endpoints de seeding:

### Theater Rooms
- `POST /api/theaterrooms/seed?clearExisting=true` - Crear salas
- `DELETE /api/theaterrooms/clear-all` - Eliminar todas las salas

### Screenings
- `POST /api/screenings/seed?clearExisting=true` - Crear funciones
- `DELETE /api/screenings/clear-all` - Eliminar todas las funciones

---

## Resultado Esperado

Después de ejecutar el seeding:

```
========================================
  Cinema Database Seeder
========================================

PASO 1: Theater Rooms (Salas de Cine)
--------------------------------------
→ Creando salas de cine... ✓

  Salas creadas:
    • Salas normales: 15
    • Salas VIP: 5
    • Total: 20 salas

PASO 2: Screenings (Funciones)
--------------------------------------
→ Creando funciones... ✓

  Funciones creadas:
    • Total: ~267 funciones
    • Películas 'En Cartelera': 8
    • Películas 'Más Populares': 8
    • Películas con funciones: 15

========================================
  ✓ Base de datos poblada exitosamente
========================================
```

---

## Notas

- Las películas "Próximamente" NO obtienen funciones automáticas
- El admin debe agregarlas manualmente desde el Panel de Admin
- Los scripts usan `clearExisting=true` para evitar duplicados
- Las funciones se crean para 7 días (hoy + 6 días)
- Horarios: 14:00, 17:30, 21:00, 23:30

---

Para más información, consulta `SCREENINGS_SETUP.md`.
