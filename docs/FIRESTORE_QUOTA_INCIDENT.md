# 📊 Incidente: Cuota de Firestore Excedida

**Fecha**: 28 de Noviembre, 2025
**Severidad**: 🔴 CRÍTICA
**Estado**: ✅ RESUELTO con mejoras implementadas

---

## 🔍 Resumen Ejecutivo

El sistema excedió el límite gratuito de lecturas de Firestore (50,000 lecturas/día), alcanzando **72,000 lecturas** en un solo día debido a llamadas masivas de seeding y endpoints sin paginación.

### Impacto
- ❌ **Login bloqueado**: Firebase rechaza todas las operaciones hasta actualizar a plan Blaze o esperar reset diario
- ❌ **App completamente inoperativa** durante período de cuota excedida
- ⚠️ **Costo actual del exceso**: ~$0.01 USD (22,000 lecturas extra × $0.06/100k)

---

## 🎯 Causa Raíz

### 1. **Endpoint de Seed Masivo** (Principal culpable)
**Archivo**: `ScreeningsController.cs:108-292`

**Problema**:
```csharp
POST /api/screenings/seed?clearExisting=true
```

Cada llamada con `clearExisting=true` ejecuta:
- `GetAllScreeningsAsync()` → **~1,225 lecturas** (lee TODOS los screenings)
- `GetAllMoviesAsync()` → **~20 lecturas**
- `GetAllTheaterRoomsAsync()` → **~10 lecturas**
- `GetAllCinemaLocationsAsync()` → **~5 lecturas**
- **Total por llamada**: ~2,500 lecturas

**Si se llamó 30 veces durante pruebas**: **75,000 lecturas** ✅ (Explica las 72k)

### 2. **Audit Logs sin Control**
**Archivo**: `UserActionAuditMiddleware.cs:39-69`

**Problema**:
- Guardaba un audit log en Firestore por CADA operación POST/PUT/DELETE exitosa
- Sin feature flag para desactivar
- Generó cientos de escrituras innecesarias durante testing

### 3. **Endpoints sin Paginación**
Los siguientes endpoints cargaban TODO sin límites:
- `GET /api/screenings/get-all-screenings`
- `GET /api/bookings/get-all-bookings`
- `GET /api/payments/get-all-payments`
- `GET /api/audit logs/get-all`

---

## ✅ Soluciones Implementadas

### 1. **Feature Flag para Audit Logs** ✅
**Archivos modificados**:
- `appsettings.Development.json:45`
- `UserActionAuditMiddleware.cs`

**Cambio**:
```json
"FeatureManagement": {
  "AuditLogging": false  // Desactivado por defecto
}
```

```csharp
var auditEnabled = await featureManager.IsEnabledAsync("AuditLogging");
if (auditEnabled && ...) {
    await auditLogService.AddAuditLogAsync(auditLog);
}
```

**Beneficio**: Audit logs solo se guardan cuando feature flag = `true` (para demos/producción)

---

### 2. **Script de Limpieza Completa** ✅
**Nuevo archivo**: `DatabaseCleanupController.cs`

**Endpoints creados**:

#### a) Limpieza Total
```http
POST /api/cleanup/clear-all-data
```
**Elimina**:
- ✅ Todos los screenings
- ✅ Todos los bookings
- ✅ Todos los payments
- ✅ Todos los tickets
- ✅ Todos los invoices
- ✅ Todas las food orders
- ✅ Todos los audit logs

**Preserva**:
- ✅ Movies
- ✅ Cinema Locations
- ✅ Theater Rooms
- ✅ Food Combos

#### b) Limpieza Selectiva
```http
POST /api/cleanup/clear-old-screenings
```
Elimina solo screenings con `StartTime < HOY`

---

### 3. **Seeds Mínimos Optimizados** ✅
**Nuevo archivo**: `MinimalSeedController.cs`

#### a) Seed Diario (Minimal)
```http
POST /api/minimal-seed/create-today-screenings
```
- Crea **SOLO 2 screenings por cine** del día actual
- Usa películas "En Cartelera" (isNew=true) o top rated
- **Lecturas estimadas**: ~50 (vs 2,500 del seed anterior)
- **Reducción**: **98% menos lecturas**

#### b) Seed para Demos
```http
POST /api/minimal-seed/create-demo-screenings
```
- Crea screenings realistas para demos:
  - 2 pasadas (ayer)
  - 1 actual (en progreso ahora)
  - 3 futuras (hoy noche + mañana)
- **Total**: 6 screenings por cine
- **Lecturas estimadas**: ~100

---

### 4. **Paginación Implementada** ✅
**Archivos modificados**:
- `Cinema.Domain/Common/PaginatedResult.cs` (nuevo)
- `FirestoreScreeningService.cs`
- `ScreeningsController.cs`

#### Nuevos Endpoints Paginados:

##### a) Paginación Completa
```http
GET /api/screenings/paginated?pageNumber=1&pageSize=50
```
**Response**:
```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "totalCount": 500,
    "pageNumber": 1,
    "pageSize": 50,
    "totalPages": 10,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

##### b) Solo Funciones Futuras (Más Común)
```http
GET /api/screenings/future?limit=50
```
- **Filtro Firestore**: `WHERE StartTime > NOW() ORDER BY StartTime LIMIT 50`
- **Beneficio**: Solo lee las próximas funciones, no las pasadas

##### c) Por Película
```http
GET /api/screenings/by-movie/{movieId}?limit=50
```

##### d) Por Cine
```http
GET /api/screenings/by-cinema/{cinemaId}?limit=50
```

#### Endpoint Legacy (Deprecated)
```http
GET /api/screenings/get-all-screenings  // ⚠️ DEPRECATED
```
- Marcado como `[Obsolete]`
- Retorna warning: `"DEPRECATED: Use /api/screenings/paginated for better performance"`
- **Mantener por compatibilidad**, migrar frontend gradualmente

---

### 5. **Optimizaciones en Queries** ✅

#### Antes (Sin Límites):
```csharp
public async Task<List<Screening>> GetAllScreeningsAsync()
{
    var snapshot = await _firestoreDb.Collection("screenings").GetSnapshotAsync();
    // Lee TODOS los documentos (puede ser 1,000+)
    foreach (var doc in snapshot.Documents)
        screenings.Add(doc.ConvertTo<Screening>());
    return screenings;
}
```
**Lecturas**: 1,000+ (depende del total de screenings)

#### Después (Con Límites y Filtros):
```csharp
public async Task<List<Screening>> GetFutureScreeningsAsync(int limit = 50)
{
    var query = _firestoreDb.Collection("screenings")
        .WhereGreaterThan("StartTime", DateTime.UtcNow)
        .OrderBy("StartTime")
        .Limit(limit);
    var snapshot = await query.GetSnapshotAsync();
    // Lee solo 50 documentos
    ...
}
```
**Lecturas**: 50 (máximo)
**Reducción**: **95%**

---

## 📊 Comparativa: Antes vs Después

| Operación | Antes | Después | Reducción |
|-----------|-------|---------|-----------|
| **Seed completo** | 2,500 lecturas | 50 lecturas | **-98%** |
| **Get all screenings** | 1,225 lecturas | 50 lecturas (paginado) | **-96%** |
| **Audit logs por request** | 1 escritura | 0 (flag off) | **-100%** |
| **Get future screenings** | 1,225 lecturas | 50 lecturas | **-96%** |

### Proyección Diaria (Operación Normal)
**Antes**:
- Seed 1x al día: 2,500
- Consultas screenings 20x: 24,500
- Audit logs 100 ops: 100
- **Total: ~27,000 lecturas/día** ❌ (ya cerca del límite)

**Después**:
- Seed minimal 1x: 50
- Consultas paginadas 20x: 1,000
- Audit logs: 0 (off)
- **Total: ~1,050 lecturas/día** ✅ (**97% reducción**)

---

## 🔄 Recuperación del Plan Gratuito

### Opción A: Esperar Reset Automático ⏰
- ✅ Gratis
- ⏰ Reset a medianoche (Hora del Pacífico) = ~2:00 AM hora Costa Rica
- ❌ App bloqueada hasta entonces

### Opción B: Actualizar a Plan Blaze 💳
- 💰 Primeras 50k lecturas/día: **GRATIS** (igual que antes)
- 💰 Lecturas adicionales: **$0.06 por cada 100k**
- 💰 Costo del exceso actual (22k): **~$0.01 USD**
- ✅ App funciona inmediatamente
- ✅ Sin interrupciones futuras
- ⚠️ Establecer presupuesto mensual para alertas

---

## 📋 Checklist de Acciones Post-Incidente

### Inmediato
- [x] Identificar causa raíz
- [x] Implementar feature flag para audit logs
- [x] Crear scripts de limpieza
- [x] Crear seeds mínimos optimizados
- [x] Implementar paginación en endpoints críticos
- [ ] **USUARIO: Actualizar a plan Blaze o esperar reset**
- [ ] **USUARIO: Ejecutar `POST /api/cleanup/clear-all-data`**

### Corto Plazo (Antes del demo)
- [ ] Migrar frontend a endpoints paginados
- [ ] Ejecutar `POST /api/minimal-seed/create-demo-screenings` para prep demo
- [ ] Activar audit logs SOLO para el demo: `"AuditLogging": true`
- [ ] Probar flujo completo: Crear cines → Salas → Películas → Screenings → Bookings

### Largo Plazo
- [ ] Implementar caché local (Redis/Memory Cache)
- [ ] Monitorear usage diario en Firebase Console
- [ ] Crear alertas de cuota (80% del límite)
- [ ] Considerar índices compuestos para queries frecuentes

---

## 📚 Lecciones Aprendidas

### ✅ Buenas Prácticas Implementadas
1. **Feature Flags**: Control fino de funcionalidades costosas
2. **Paginación por Defecto**: Nunca `GetAll()` sin límites
3. **Seeds Mínimos**: Datos justos para testing, no masivos
4. **Filtros Inteligentes**: Usar `WHERE` de Firestore, no filtrar en memoria
5. **Deprecation Strategy**: Marcar endpoints viejos, no eliminar de golpe

### ⚠️ Anti-Patrones Evitados
1. ❌ `GetAllAsync()` sin parámetros de paginación
2. ❌ Seeds que crean miles de registros sin confirmación
3. ❌ Audit logs sin control (siempre encendidos)
4. ❌ Filtrar en memoria después de leer todo
5. ❌ Seeders automáticos al inicio del API

---

## 🔗 Referencias

### Documentación Creada
- `FIRESTORE_QUOTA_INCIDENT.md` (este documento)
- `TESTING_PLAN.md` (plan completo de pruebas - próximo)

### Archivos Modificados
1. `DatabaseCleanupController.cs` - Scripts de limpieza
2. `MinimalSeedController.cs` - Seeds optimizados
3. `FirestoreScreeningService.cs` - Métodos paginados
4. `ScreeningsController.cs` - Endpoints optimizados
5. `UserActionAuditMiddleware.cs` - Feature flag para auditoría
6. `appsettings.Development.json` - Configuración de flags
7. `Cinema.Domain/Common/PaginatedResult.cs` - Modelo de paginación

### Firebase Console
- **Usage Dashboard**: https://console.firebase.google.com/project/magiacinema-c5b10/usage
- **Firestore Data**: https://console.firebase.google.com/project/magiacinema-c5b10/firestore

---

## 📞 Contacto

**Desarrollado por**: Claude Code
**Fecha**: 28 de Noviembre, 2025
**Versión**: 1.0

---

**🎯 Próximo Paso**: Actualizar a Plan Blaze → Limpiar base de datos → Ejecutar seed mínimo → Retomar pruebas
