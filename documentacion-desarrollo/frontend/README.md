# Documentación del Frontend - Cinema App

Documentación específica del frontend del proyecto Cinema (Flutter/Dart).

---

## 📚 Documentos Disponibles

### 🔗 Integración con Backend

**[FRONTEND_INTEGRATION_STATUS.md](./FRONTEND_INTEGRATION_STATUS.md)**
- Estado de integración frontend-backend
- Implementaciones completadas
- Features del sistema de reservas y pagos
- Providers de Riverpod
- Modelos y servicios
- Flujo de usuario completo
- Checklist de testing

---

## 🏗️ Arquitectura del Frontend

### Stack Tecnológico
- **Flutter 3.x** - Framework UI multiplataforma
- **Dart** - Lenguaje de programación
- **Riverpod** - State management
- **Go Router** - Navegación
- **HTTP** - Cliente API REST
- **QR Flutter** - Lectura/generación de QR
- **Cloudinary SDK** - Carga de imágenes

### Estructura del Proyecto

```
lib/
├── core/              ← Configuración y utilidades
│   ├── config/       ← Configuración (theme, routes)
│   ├── constants/    ← Constantes (colors, strings)
│   └── utils/        ← Utilidades helpers
├── features/          ← Módulos por feature
│   ├── movies/       ← Películas
│   │   ├── models/
│   │   ├── providers/
│   │   ├── services/
│   │   └── pages/
│   ├── booking/      ← Reservas
│   │   ├── models/
│   │   ├── providers/
│   │   └── pages/
│   ├── payment/      ← Pagos
│   ├── auth/         ← Autenticación
│   └── ...
└── main.dart          ← Punto de entrada
```

### Features Principales

#### 🎬 Movies (Películas)
- Carrusel de películas
- Filtros: En Cartelera, Próximamente, Más Populares
- Detalles de película
- Integración con Cloudinary

#### 🎟️ Booking (Reservas)
- Selección de horarios
- Selección de asientos
- Agregar comida y bebidas
- Resumen de reserva

#### 💳 Payment (Pagos)
- Simulación de pago con tarjeta
- Validación Luhn
- Confirmación de pago

#### 🎫 Tickets (Boletos)
- Visualización de boletos
- Códigos QR
- Descarga de facturas

#### 👤 Profile (Perfil)
- Historial de reservas
- Mis boletos
- Configuración

---

## 🚀 Inicio Rápido

### 1. Configurar el Proyecto
Ver [../general/SETUP.md](../general/SETUP.md)

### 2. Instalar Dependencias
```bash
flutter pub get
```

### 3. Ejecutar la App
```bash
# Web
flutter run -d chrome

# Android
flutter run -d android

# iOS
flutter run -d ios
```

### 4. Build para Producción
```bash
# Web
flutter build web

# Android APK
flutter build apk

# iOS
flutter build ios
```

---

## 🎨 Diseño y UI/UX

### Tema de Colores
- **Primary**: Azul oscuro (#1A1D2E)
- **Secondary**: Rojo (#E94560)
- **Background**: Negro (#0F0F0F)
- **Cards**: Gris oscuro (#1F1F1F)

### Componentes Personalizados
- `MovieCard` - Tarjeta de película
- `SeatWidget` - Asiento de cine
- `FoodItemCard` - Item de comida
- `TicketWidget` - Boleto digital
- `CustomAppBar` - App bar personalizada

### Responsive Design
- **Mobile**: < 600px
- **Tablet**: 600px - 1024px
- **Desktop**: > 1024px

---

## 🔌 Integración con API

### Base URL
```dart
const String baseUrl = 'https://localhost:7238/api';
```

### Servicios HTTP

```dart
// Ejemplo: MovieService
class MovieService {
  Future<List<Movie>> getAllMovies() async {
    final response = await http.get(Uri.parse('$baseUrl/movies'));
    // ...
  }
}
```

### State Management con Riverpod

```dart
// Provider para películas
final moviesProvider = FutureProvider<List<Movie>>((ref) async {
  final movieService = ref.watch(movieServiceProvider);
  return await movieService.getAllMovies();
});

// Uso en widget
Consumer(
  builder: (context, ref, child) {
    final moviesAsync = ref.watch(moviesProvider);

    return moviesAsync.when(
      data: (movies) => MoviesList(movies: movies),
      loading: () => CircularProgressIndicator(),
      error: (err, stack) => ErrorWidget(error: err),
    );
  },
)
```

---

## 🧪 Testing

### Ejecutar Tests
```bash
# Todos los tests
flutter test

# Tests específicos
flutter test test/features/movies/
```

### Testing Manual
Ver [../general/TESTING_GUIDE.md](../general/TESTING_GUIDE.md)

---

## 📝 Flujo de Usuario Completo

### 1. Navegación
```
Home → Movies → Movie Details → Select Showtime →
Seat Selection → Food Selection → Payment → Confirmation → Tickets
```

### 2. Autenticación
```
Login → Register → Forgot Password → Reset Password
```

### 3. Perfil
```
Profile → My Bookings → My Tickets → Settings → Logout
```

---

## 🔒 Configuración

### API Keys (lib/core/config/env.dart)

```dart
class Environment {
  static const String apiBaseUrl = 'https://localhost:7238/api';
  static const String cloudinaryCloudName = 'tu-cloud-name';
  static const String cloudinaryUploadPreset = 'tu-preset';
}
```

### Firebase (opcional)
```dart
// lib/firebase_options.dart
static const FirebaseOptions web = FirebaseOptions(
  apiKey: "tu-api-key",
  projectId: "tu-project-id",
  // ...
);
```

---

## 📱 Plataformas Soportadas

- ✅ **Web** (Chrome, Firefox, Safari)
- ✅ **Android** (API 21+)
- ✅ **iOS** (iOS 12+)
- ⚠️ **Desktop** (experimental)

---

## 📖 Documentación Adicional

- **Backend API**: Ver [../backend/API_COLLECTION.md](../backend/API_COLLECTION.md)
- **Setup General**: Ver [../general/SETUP.md](../general/SETUP.md)
- **Testing**: Ver [../general/TESTING_GUIDE.md](../general/TESTING_GUIDE.md)

---

## 🐛 Troubleshooting

### Error: "Connection refused"
- Verifica que el backend esté corriendo en `https://localhost:7238`
- Revisa la configuración de CORS en el backend

### Error: "SSL certificate error"
- En desarrollo, el backend usa certificados autofirmados
- Configura tu navegador/dispositivo para aceptar estos certificados

### Error: "Provider not found"
- Asegúrate de que `ProviderScope` esté en la raíz de tu app
- Verifica que estés usando `ConsumerWidget` o `Consumer`

---

**Última actualización**: Noviembre 26, 2025
