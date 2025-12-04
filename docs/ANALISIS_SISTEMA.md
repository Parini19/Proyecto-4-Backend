# ANÁLISIS COMPLETO DEL SISTEMA - DUDAS Y RESPUESTAS

## 1. COLOR MORADO EN SELECTOR DE ASIENTOS

### Respuesta:
**El color morado/púrpura representa asientos VIP**

### Código Fuente:
Archivo: `lib/features/booking/widgets/seat_widget.dart` - Línea 135-143

```dart
Color _getColorForType(SeatType type) {
  switch (type) {
    case SeatType.regular:
      return AppColors.seatAvailable; // Verde
    case SeatType.vip:
      return AppColors.vip; // Morado/Oro (Purple/Gold)
    case SeatType.wheelchair:
      return AppColors.info; // Azul
  }
}
```

---

## 2. SISTEMA COMPLETO DE COLORES EN SELECTOR DE ASIENTOS

### Colores y Significados:

| Color | Significado | Código | Interactivo |
|-------|------------|--------|-------------|
| **Verde** | Asiento regular disponible | `AppColors.seatAvailable` | ✅ Sí |
| **Morado/Oro** | Asiento VIP disponible | `AppColors.vip` | ✅ Sí |
| **Azul** | Asiento para discapacitados | `AppColors.info` | ✅ Sí |
| **Celeste/Primary** | Asiento SELECCIONADO (tu selección actual) | `AppColors.primary` | ✅ Sí |
| **Gris claro** | Asiento OCUPADO o RESERVADO | `AppColors.surfaceVariant` | ❌ No |

### Iconos:
- **X blanca**: Asiento ocupado/reservado (línea 63: `Icons.close`)
- **♿ Silla de ruedas**: Asiento para discapacitados (línea 76: `Icons.accessible`)

### Estado Visual:
- **Glow/Brillo**: Los asientos seleccionados tienen sombra neón (`AppColors.glowShadow`)
- **Animación**: Al presionar un asiento hay animación de escala (se achica a 0.9x)

---

## 3. SELECCIÓN DE CINE POR USUARIO - ¿CÓMO FUNCIONA?

### Estado Actual:
⚠️ **PROBLEMA IDENTIFICADO: No existe un flujo completo de selección de cine para el usuario final**

### Lo que EXISTE:
1. El backend tiene cines en Firestore (6 cines creados)
2. Cada screening tiene `cinemaId` asociado
3. Cada theater room tiene `cinemaId` asociado

### Lo que NO EXISTE:
1. **No hay página de usuario** donde pueda ver lista de cines disponibles
2. **No hay filtro por cine** en la página de películas del usuario
3. **No hay mapa** o ubicación de cines
4. **El usuario NO puede escoger cine** - solo ve funciones globales

### Solución Propuesta:
**Se necesita implementar:**
1. Página "Seleccionar Cine" para usuarios
2. Mostrar ubicación/ciudad de cada cine
3. Filtrar películas y funciones por cine seleccionado
4. Guardar cine preferido del usuario en sesión

---

## 4. CONFIGURADOR VISUAL DE ASIENTOS PARA ADMIN

### Estado Actual:
⚠️ **NO IMPLEMENTADO - Solo existe configuración numérica**

### Lo que existe:
- Admin puede configurar `capacity` (número total de asientos)
- Se genera `seatConfiguration` como JSON string: `{"rows": 8, "seatsPerRow": 12}`
- El layout es AUTOMÁTICO basado en filas × columnas

### Lo que NO existe:
- ❌ Editor visual de drag & drop de asientos
- ❌ Colocar asientos personalizados (forma de L, espacios vacíos, etc.)
- ❌ Marcar asientos específicos como VIP o discapacitados
- ❌ Vista previa del layout

### Solución Propuesta:
**Implementar Editor Visual de Asientos:**
1. Grid interactivo donde puedas:
   - Arrastrar asientos
   - Hacer clic para cambiar tipo (Regular/VIP/Wheelchair)
   - Dejar espacios vacíos (pasillos)
   - Numerar asientos personalizadamente
2. Guardar layout como JSON detallado en `seatConfiguration`
3. Vista previa del layout guardado

---

## 5. DASHBOARD - CARDS DE ESTADÍSTICAS

### Respuesta:
❌ **TODOS LOS DATOS ESTÁN HARDCODEADOS (QUEMADOS)**

### Prueba:
Archivo: `lib/features/admin/pages/admin_dashboard.dart` - Líneas 393-446

```dart
_buildStatCard(
  icon: Icons.movie,
  title: 'Películas',
  value: '24',  // ← HARDCODED
  change: '+3', // ← HARDCODED
  ...
),
_buildStatCard(
  icon: Icons.event,
  title: 'Funciones Hoy',
  value: '48',  // ← HARDCODED
  change: '+8', // ← HARDCODED
  ...
),
_buildStatCard(
  icon: Icons.confirmation_number,
  title: 'Boletos Vendidos',
  value: '342', // ← HARDCODED
  ...
),
_buildStatCard(
  icon: Icons.attach_money,
  title: 'Ingresos Hoy',
  value: '\$4,280', // ← HARDCODED
  ...
),
```

### Otros datos quemados:
- "Ocupación Promedio: 78%" (línea 493)
- "Película Más Vista: Avatar 2" (línea 502)
- "Sala Más Usada: Sala 3" (línea 511)
- Actividad reciente (líneas 460-485)

### Solución Propuesta:
Necesitas crear servicios/queries que calculen:
1. Total de películas en BD
2. Funciones hoy (filtrar por fecha actual)
3. Boletos vendidos (total de bookings)
4. Ingresos (suma de pagos)
5. Ocupación promedio (asientos ocupados / total)
6. Película más vista (contar bookings por película)
7. Sala más usada (contar screenings por sala)

---

## 6. GESTIÓN DE PELÍCULAS - CARDS

### Respuesta:
✅ **LOS DATOS SON REALES (de Firestore)**

### Prueba:
Archivo: `lib/features/admin/pages/movies_management_page.dart` - Líneas 32-58

```dart
Future<void> _loadMovies() async {
  setState(() {
    _isLoading = true;
    _error = null;
  });

  try {
    print('📽️ Cargando películas desde el backend...');
    final movies = await _moviesService.getAllMovies(); // ← SERVICIO REAL
    print('📽️ Películas cargadas: ${movies.length}');

    setState(() {
      _movies = movies;        // ← DATOS REALES
      _filteredMovies = movies;
      _isLoading = false;
    });
  } catch (e) {
    print('❌ Error cargando películas: $e');
    setState(() {
      _error = 'Error cargando películas del servidor: $e';
      _isLoading = false;
    });
  }
}
```

**Conclusión:** Los datos de gestión de películas SÍ provienen de Firestore a través del servicio `MoviesService`.

---

## 7. FOOD COMBOS - CARDS (Total, Disponibles, No Disponibles)

### Respuesta:
✅ **LOS DATOS SON REALES (de Firestore)**

### Prueba:
Archivo: `lib/features/admin/pages/food_combos_management_page.dart` - Líneas 33-53

```dart
Future<void> _loadFoodCombos() async {
  setState(() {
    _isLoading = true;
    _error = null;
  });

  try {
    final combos = await _foodComboService.getAllFoodCombos(); // ← SERVICIO REAL

    setState(() {
      _combos = combos;           // ← DATOS REALES
      _filteredCombos = combos;
      _isLoading = false;
    });
  } catch (e) {
    setState(() {
      _error = 'Error al cargar los combos de comida: $e';
      _isLoading = false;
    });
  }
}
```

**Nota:** Los contadores (Total, Disponibles, No Disponibles) se calculan dinámicamente filtrando la lista `_combos` por el campo `isAvailable`.

---

## 8. ÓRDENES DE COMIDA - 5 CARDS Y BASE DE DATOS

### Respuesta:
✅ **LOS DATOS SON REALES Y SE GUARDAN EN FIRESTORE**

### Prueba:
Archivo: `lib/features/admin/pages/food_orders_management_page.dart` - Líneas 37-56

```dart
Future<void> _loadFoodOrders() async {
  setState(() {
    _isLoading = true;
    _error = null;
  });

  try {
    final orders = await _foodOrderService.getAllFoodOrders(); // ← SERVICIO REAL
    setState(() {
      _orders = orders;  // ← DATOS REALES DE FIRESTORE
      _filterOrders();
      _isLoading = false;
    });
  } catch (e) {
    setState(() {
      _error = 'Error cargando órdenes: $e';
      _isLoading = false;
    });
  }
}
```

### Funcionalidad de actualización de estado:
```dart
Future<void> _updateOrderStatus(String orderId, String newStatus) async {
  try {
    final success = await _foodOrderService.updateOrderStatus(orderId, newStatus);
    // ← ACTUALIZA EN FIRESTORE
    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Estado actualizado correctamente'),
          backgroundColor: AppColors.success,
        ),
      );
      await _loadFoodOrders();
    }
  } catch (e) {
    _showErrorSnackBar('Error al actualizar el estado');
  }
}
```

**Conclusión:** Las órdenes de comida SÍ se guardan en Firestore y los 5 cards muestran datos reales.

---

## 9. USUARIOS - CARDS

### Respuesta:
✅ **LOS DATOS SON REALES (Confirmado por usuario)**

**Evidencia del usuario:**
> "Pero usuarios si veo que los datos coinciden porque son 5 usuarios y 5 activos y 3 admins"

Esto confirma que los contadores de usuarios provienen de Firestore y reflejan:
- Total de usuarios en BD
- Usuarios activos
- Usuarios con rol de admin

---

## RESUMEN FINAL

### ✅ DATOS REALES (de Firestore):
1. ✅ Gestión de Películas - Cards
2. ✅ Food Combos - Cards (Total, Disponibles, No Disponibles)
3. ✅ Órdenes de Comida - 5 Cards + Se guardan en BD
4. ✅ Usuarios - Cards

### ❌ DATOS HARDCODEADOS (Quemados):
1. ❌ Dashboard - Todos los cards de estadísticas
2. ❌ Dashboard - Actividad reciente
3. ❌ Dashboard - Quick stats (Ocupación, Película más vista, Sala más usada)

### ⚠️ FUNCIONALIDADES FALTANTES:
1. ⚠️ Selección de cine por usuario final
2. ⚠️ Filtro de películas/funciones por cine
3. ⚠️ Configurador visual de asientos (admin)
4. ⚠️ Mapa/ubicación de cines

---

## PRIORIDADES SUGERIDAS

### 🔴 ALTA PRIORIDAD:
1. **Implementar estadísticas reales en Dashboard** (actualmente todo hardcoded)
2. **Sistema de selección de cine para usuarios** (falta completamente)

### 🟡 MEDIA PRIORIDAD:
3. **Configurador visual de asientos** (mejora UX admin)

### 🟢 BAJA PRIORIDAD:
4. Mapa de cines con ubicación geográfica

---

Generado: 2025-11-26
