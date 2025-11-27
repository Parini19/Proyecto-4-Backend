# 🎬 Sistema Completo de Cinema (Salas + Funciones)

## 📋 Tabla de Contenidos
- [Salas de Cine (Theater Rooms)](#-salas-de-cine-theater-rooms)
- [Sistema de Funciones (Screenings)](#-sistema-de-funciones-screenings)
- [Script Automático de Seeding](#-script-automático-de-seeding)

---

# 🏛️ Salas de Cine (Theater Rooms)

## Estructura de Salas

El sistema crea **20 salas** automáticamente:

### Salas Normales (15 salas)
- **Nombres**: "Sala 1" hasta "Sala 15"
- **IDs**: SALA-01 hasta SALA-15
- **Capacidad**: 96 asientos cada una (8 filas × 12 asientos)

### Salas VIP (5 salas)
- **Nombres**: "Sala VIP 1" hasta "Sala VIP 5"
- **IDs**: SALA-VIP-01 hasta SALA-VIP-05
- **Capacidad**: 60 asientos cada una (más lujosas, menos asientos)

**Total**: 20 salas (1,590 asientos totales)

## Endpoints de Theater Rooms

### 1. Seed Theater Rooms
```bash
POST https://localhost:7238/api/theaterrooms/seed
POST https://localhost:7238/api/theaterrooms/seed?clearExisting=true
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Created 20 theater rooms (15 normal + 5 VIP)",
  "count": 20,
  "normalRooms": 15,
  "vipRooms": 5,
  "rooms": [
    { "id": "SALA-01", "name": "Sala 1", "capacity": 96 },
    { "id": "SALA-VIP-01", "name": "Sala VIP 1", "capacity": 60 }
  ]
}
```

### 2. Clear All Theater Rooms
```bash
DELETE https://localhost:7238/api/theaterrooms/clear-all
```

### 3. Get All Theater Rooms
```bash
GET https://localhost:7238/api/theaterrooms/get-all-theater-rooms
```

---

# 📅 Sistema de Funciones (Screenings)

## 🎯 Lógica de Negocio

El sistema de funciones funciona de la siguiente manera:

### Películas que OBTIENEN funciones automáticamente:
1. **"En Cartelera"** - Todas las películas con `isNew = true`
2. **"Más Populares"** - Las 8 películas con mejor rating (ordenadas por `rating` descendente)

### Películas que NO obtienen funciones:
- **"Próximamente"** - Películas con `isNew = false` que no están en el top 8 de rating
- Estas deben ser agregadas **manualmente por el administrador** desde el Panel de Admin

---

## 🚀 Endpoints Disponibles

### 1. Seed Screenings (Crear funciones automáticas)
```bash
POST https://localhost:7238/api/screenings/seed
POST https://localhost:7238/api/screenings/seed?clearExisting=true
```

**Descripción:**
- Crea funciones para los próximos 7 días
- Solo para películas "En Cartelera" y "Más Populares"
- Genera múltiples horarios por día (14:00, 17:30, 21:00, 23:30)
- Distribuye en 4 salas (ROOM-1, ROOM-2, ROOM-3, ROOM-4)

**Parámetros:**
- `clearExisting` (opcional): Si es `true`, elimina todas las funciones existentes antes de crear nuevas

**Ejemplo con cURL:**
```bash
# Crear funciones (mantiene las existentes)
curl -X POST https://localhost:7238/api/screenings/seed -k

# Limpiar y volver a crear
curl -X POST "https://localhost:7238/api/screenings/seed?clearExisting=true" -k
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Created 174 screenings for the next 7 days",
  "count": 174,
  "moviesWithScreenings": [
    {
      "id": "1",
      "title": "Dune: Part Two",
      "isNowPlaying": true,
      "rating": 8.8
    }
  ],
  "nowPlayingCount": 5,
  "popularCount": 8,
  "totalMoviesWithScreenings": 10,
  "note": "Solo películas 'En Cartelera' (isNew=true) y 'Más Populares' (top 8 por rating) obtienen funciones automáticas...",
  "sampleScreenings": [...]
}
```

### 2. Clear All Screenings (Limpiar todas las funciones)
```bash
DELETE https://localhost:7238/api/screenings/clear-all
```

**Descripción:**
- Elimina TODAS las funciones de Firestore
- Útil para testing y resetear el sistema

**Ejemplo con cURL:**
```bash
curl -X DELETE https://localhost:7238/api/screenings/clear-all -k
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Deleted 174 screenings",
  "count": 174
}
```

### 3. Get All Screenings
```bash
GET https://localhost:7238/api/screenings/get-all-screenings
```

**Descripción:**
- Obtiene todas las funciones existentes

### 4. Get Screening by ID
```bash
GET https://localhost:7238/api/screenings/get-screening/{id}
```

---

## 🎬 Configuración de Películas

Para que una película aparezca en los diferentes carruseles:

### "En Cartelera" (Now Playing)
```json
{
  "id": "1",
  "title": "Dune: Part Two",
  "isNew": true,  // ✅ DEBE ser true
  "rating": 8.8,
  "..."
}
```
✅ **Obtiene funciones automáticamente**

### "Próximamente" (Upcoming)
```json
{
  "id": "20",
  "title": "Avatar 3",
  "isNew": false,  // ❌ false = próximamente
  "rating": 7.5,
  "..."
}
```
❌ **NO obtiene funciones** (a menos que esté en top 8 por rating)
👤 **El admin debe agregarlas manualmente** cuando la película esté lista para estreno

### "Más Populares" (Popular)
```json
{
  "id": "5",
  "title": "Oppenheimer",
  "isNew": false,
  "rating": 9.2,  // ✅ Rating alto (top 8)
  "..."
}
```
✅ **Obtiene funciones automáticamente** (aunque isNew = false)

---

## 👨‍💼 Panel de Administración

### Agregar Funciones Manualmente

El administrador puede agregar funciones para películas "Próximamente" desde:

**Ruta:** Admin Panel → Funciones → Agregar Nueva Función

**Campos:**
1. **Película** - Seleccionar de la lista
2. **Sala** - ROOM-1, ROOM-2, ROOM-3, ROOM-4
3. **Fecha y Hora de Inicio**
4. **Fecha y Hora de Fin**

**Ejemplo de uso:**
1. Una película "Próximamente" (`isNew = false`) está a punto de estrenarse
2. El admin entra al panel y crea funciones manualmente
3. Los usuarios pueden ver y reservar esas funciones
4. Cuando se quiera que la película tenga funciones automáticas, cambiar `isNew = true`

---

# 🚀 Script Automático de Seeding

## Uso Rápido

### Windows (PowerShell)
```powershell
cd "C:\Users\Guillermo Parini\Documents\Cinema"
.\seed-database.ps1
```

### Linux/Mac (Bash)
```bash
cd "/path/to/Cinema"
chmod +x seed-database.sh
./seed-database.sh
```

El script ejecuta automáticamente:
1. ✅ Limpia y crea 20 salas de cine (15 normales + 5 VIP)
2. ✅ Limpia y crea funciones para los próximos 7 días
3. ✅ Muestra un resumen detallado de lo creado

## 🔄 Flujo Completo

```
┌─────────────────────────────────────────────────────────┐
│ 1. Agregar Películas a Firestore                       │
│    - Películas con isNew=true → "En Cartelera"        │
│    - Películas con isNew=false → "Próximamente"       │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ 2. Ejecutar Script de Seeding                          │
│    .\seed-database.ps1  (Windows)                      │
│    ./seed-database.sh   (Linux/Mac)                    │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ 2a. Crear Salas de Cine (20 salas)                    │
│     - 15 salas normales (96 asientos)                  │
│     - 5 salas VIP (60 asientos)                        │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ 2b. Crear Funciones (Screenings)                       │
│     POST /api/screenings/seed?clearExisting=true       │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ 3. Sistema Crea Funciones Automáticas                  │
│    ✅ Para "En Cartelera" (isNew=true)                │
│    ✅ Para "Más Populares" (top 8 rating)             │
│    ❌ NO para "Próximamente"                          │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ 4. Admin Agrega Funciones Manualmente                  │
│    - Para películas "Próximamente"                     │
│    - Cuando estén listas para estreno                  │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ 5. Usuarios Ven y Reservan                             │
│    - Frontend consulta funciones del backend           │
│    - Muestra horarios disponibles                      │
│    - Permite reservar asientos                         │
└─────────────────────────────────────────────────────────┘
```

---

## 🧪 Testing

### Opción 1: Usar el Script Automático (Recomendado)
```powershell
# Windows
.\seed-database.ps1

# Linux/Mac
./seed-database.sh
```

### Opción 2: Ejecutar Manualmente

#### Paso 1: Crear salas de cine
```bash
curl -X POST "https://localhost:7238/api/theaterrooms/seed?clearExisting=true" -k
```

#### Paso 2: Crear funciones
```bash
curl -X POST "https://localhost:7238/api/screenings/seed?clearExisting=true" -k
```

#### Paso 3: Verificar datos creados
```bash
# Verificar salas
curl https://localhost:7238/api/theaterrooms/get-all-theater-rooms -k

# Verificar funciones
curl https://localhost:7238/api/screenings/get-all-screenings -k
```

### Paso 4: Probar flujo de reserva
1. Abrir frontend
2. Seleccionar película "En Cartelera" o "Popular"
3. Debe mostrar horarios disponibles
4. Seleccionar asientos → Agregar comida → Pagar
5. Verificar que la reserva se crea exitosamente

---

## 📊 Estructura de Datos

### Screening
```json
{
  "id": "SCR-0001",
  "movieId": "1",
  "theaterRoomId": "ROOM-1",
  "startTime": "2025-11-26T14:00:00Z",
  "endTime": "2025-11-26T16:00:00Z"
}
```

### Movie
```json
{
  "id": "1",
  "title": "Dune: Part Two",
  "isNew": true,
  "rating": 8.8,
  "durationMinutes": 166,
  "..."
}
```

---

## ⚠️ Notas Importantes

1. **Orden de ejecución:** Primero crear Theater Rooms, luego Screenings
2. **El script automático lo hace todo:** Usa `seed-database.ps1` para mayor facilidad
3. **Las películas "Próximamente" NO deben tener funciones hasta que estén listas**
4. **El admin debe gestionar manualmente las funciones de películas próximas**
5. **El parámetro `clearExisting=true` elimina TODO** (usar con cuidado)
6. **Los screenings se crean para 7 días** (hoy + 6 días)
7. **Las salas se distribuyen aleatoriamente** entre las 20 salas disponibles

---

## 🐛 Troubleshooting

### Error: "No theater rooms found"
**Causa:** No se han creado las salas de cine
**Solución:** Ejecutar primero: `POST /api/theaterrooms/seed?clearExisting=true`

### Error: "No movies found to create screenings"
**Causa:** No hay películas con `isNew=true` ni películas con rating alto
**Solución:** Agregar películas a Firestore o ajustar el flag `isNew`

### Error: "404 Screening not found" al reservar
**Causa:** El frontend está usando IDs de funciones que no existen
**Solución:** Ejecutar el script de seeding completo

### Las funciones no aparecen en el frontend
**Causa:** El frontend no está consultando el backend correctamente
**Solución:** Verificar que el API esté corriendo y que el frontend use el provider correcto

### Funciones duplicadas en Firestore
**Causa:** Se ejecutó el seeder múltiples veces sin `clearExisting=true`
**Solución:** Usar `.\seed-database.ps1` que siempre limpia antes de crear
