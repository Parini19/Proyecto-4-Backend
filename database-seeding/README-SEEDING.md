# 🎬 Guía Rápida: Poblar Base de Datos

## ✅ Todo Listo - Ejecutar Script

He creado un sistema completo de seeding que limpia y puebla tu base de datos correctamente.

### 1. Asegúrate que el API esté corriendo

```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
dotnet run --urls="https://localhost:7238"
```

### 2. Ejecuta el Script de Seeding (en otra terminal)

```powershell
cd "C:\Users\Guillermo Parini\Documents\Cinema"
.\seed-database.ps1
```

### 3. ¡Listo!

El script hará todo automáticamente:
- ✅ Eliminará salas y funciones duplicadas
- ✅ Creará 20 salas limpias (15 normales + 5 VIP)
- ✅ Creará funciones solo para películas "En Cartelera" y "Más Populares"
- ✅ Mostrará un resumen detallado

---

## 📊 ¿Qué se Crea?

### Salas de Cine
- **15 salas normales**: "Sala 1" a "Sala 15" (96 asientos c/u)
- **5 salas VIP**: "Sala VIP 1" a "Sala VIP 5" (60 asientos c/u)
- **IDs**: SALA-01, SALA-02, ..., SALA-VIP-01, etc.

### Funciones (Screenings)
- Solo para películas con `isNew = true` (En Cartelera)
- Solo para películas top 8 por rating (Más Populares)
- **NO** para películas "Próximamente"
- 7 días de funciones (hoy + 6 días)
- Horarios: 14:00, 17:30, 21:00, 23:30
- Distribuidas aleatoriamente en las 20 salas

---

## 🔧 Endpoints Creados

### Theater Rooms
```bash
# Crear salas (limpiando existentes)
POST https://localhost:7238/api/theaterrooms/seed?clearExisting=true

# Limpiar todas las salas
DELETE https://localhost:7238/api/theaterrooms/clear-all

# Ver todas las salas
GET https://localhost:7238/api/theaterrooms/get-all-theater-rooms
```

### Screenings
```bash
# Crear funciones (limpiando existentes)
POST https://localhost:7238/api/screenings/seed?clearExisting=true

# Limpiar todas las funciones
DELETE https://localhost:7238/api/screenings/clear-all

# Ver todas las funciones
GET https://localhost:7238/api/screenings/get-all-screenings
```

---

## 📝 Notas Importantes

1. **Orden correcto**: El script ejecuta primero Theater Rooms, luego Screenings
2. **Sin duplicados**: Usa `clearExisting=true` para limpiar antes de crear
3. **Películas "Próximamente"**: NO tendrán funciones automáticas
4. **Admin Panel**: El administrador puede agregar funciones manualmente

---

## 📖 Documentación Completa

Para más detalles, consulta:
- `SCREENINGS_SETUP.md` - Documentación completa del sistema

---

## ✨ Resultado Esperado

Después de ejecutar el script verás:

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
    • Total: 174 funciones
    • Películas 'En Cartelera': 5
    • Películas 'Más Populares': 8
    • Películas con funciones: 10

  Películas incluidas:
    • [En Cartelera] Dune: Part Two (Rating: 8.8)
    • [Popular] Oppenheimer (Rating: 9.2)
    ... y más

========================================
  ✓ Base de datos poblada exitosamente
========================================
```

¡Todo listo para probar el sistema de reservas! 🎉
