# Cinema Management System - Plan de Trabajo

**Última actualización:** 2025-11-03

## Metodología de Trabajo

- **Enfoque:** Desarrollo iterativo por fases
- **Arquitectura:** Clean Architecture en Backend y Frontend
- **Prioridad:** Funcionalidades core primero, luego features avanzadas
- **Testing:** Implementación paralela de tests unitarios
- **Documentación:** Actualización continua durante el desarrollo

---

## FASE 1: FOUNDATION & CORE ENTITIES (2-3 semanas)

### Objetivo
Completar las operaciones CRUD para todas las entidades base y establecer la infraestructura sólida.

---

### 1.1 Backend - Movies Management (Películas) 🎬

**Prioridad:** ALTA

#### Tareas Backend:
- [ ] **Implementar MovieRepository completo**
  - [ ] `GetByIdAsync(string id)` - Obtener película por ID
  - [ ] `AddAsync(Movie movie)` - Agregar nueva película
  - [ ] `UpdateAsync(Movie movie)` - Actualizar película existente
  - [ ] `DeleteAsync(string id)` - Eliminar película
  - [ ] `ListAsync()` - Listar todas (ya existe)

- [ ] **Actualizar entidad Movie**
  - [ ] Agregar `ReleaseDate` (DateTime)
  - [ ] Agregar `Rating` (decimal, ej: 4.5/5.0)
  - [ ] Agregar `PosterUrl` (string, URL de imagen)
  - [ ] Agregar `TrailerUrl` (string, URL de YouTube)
  - [ ] Agregar `Classification` (string, ej: PG-13, R)
  - [ ] Agregar `IsActive` (bool, película activa en cartelera)
  - [ ] Agregar `Language` (string, idioma original)

- [ ] **Implementar MoviesController completo**
  - [ ] `POST /api/movies/add-movie` - Agregar película (con validación)
  - [ ] `GET /api/movies/get-movie/{id}` - Obtener película
  - [ ] `PUT /api/movies/edit-movie/{id}` - Editar película
  - [ ] `DELETE /api/movies/delete-movie/{id}` - Eliminar película
  - [ ] `GET /api/movies/get-all-movies` - Listar todas
  - [ ] `GET /api/movies/active` - Listar películas activas en cartelera

- [ ] **Agregar validaciones**
  - [ ] Validar campos requeridos (Title, Description, Duration)
  - [ ] Validar formato de URLs (Poster, Trailer)
  - [ ] Validar rangos (Rating 0-5, Duration > 0)

- [ ] **Tests Unitarios**
  - [ ] Test para cada método del repository
  - [ ] Test para cada endpoint del controller
  - [ ] Test de validaciones

#### Tareas Frontend:
- [ ] **Actualizar entidad Movie en Flutter**
  - [ ] Agregar todos los campos nuevos
  - [ ] Crear `fromJson` y `toJson` métodos
  - [ ] Mapear con backend

- [ ] **Crear MovieService**
  - [ ] `getMovies()` - Obtener películas activas
  - [ ] `getAllMovies()` - Obtener todas (admin)
  - [ ] `getMovieById(String id)` - Obtener por ID
  - [ ] `addMovie(Movie movie)` - Agregar (admin)
  - [ ] `updateMovie(Movie movie)` - Actualizar (admin)
  - [ ] `deleteMovie(String id)` - Eliminar (admin)

- [ ] **Actualizar MoviesPage (Customer)**
  - [ ] Conectar con API real (reemplazar datos estáticos)
  - [ ] Mostrar poster desde URL
  - [ ] Mostrar rating con estrellas
  - [ ] Filtrar solo películas activas
  - [ ] Agregar pull-to-refresh

- [ ] **Crear AdminMoviesPage**
  - [ ] Listar todas las películas
  - [ ] CRUD completo con formularios
  - [ ] Upload de poster URL
  - [ ] Activar/desactivar películas

---

### 1.2 Backend - Theater Rooms Management (Salas) 🏛️

**Prioridad:** ALTA

#### Tareas Backend:
- [ ] **Crear ITheaterRoomRepository interface**
  - [ ] `GetByIdAsync(string id)`
  - [ ] `ListAsync()`
  - [ ] `AddAsync(TheaterRoom room)`
  - [ ] `UpdateAsync(TheaterRoom room)`
  - [ ] `DeleteAsync(string id)`

- [ ] **Implementar FirestoreTheaterRoomRepository**
  - [ ] Colección Firestore: `theaterRooms`
  - [ ] Implementar todos los métodos CRUD
  - [ ] Manejo de errores y logging

- [ ] **Actualizar entidad TheaterRoom**
  - [ ] Agregar `Rows` (int, cantidad de filas)
  - [ ] Agregar `Columns` (int, cantidad de columnas)
  - [ ] Agregar `ScreenType` (string, ej: IMAX, 3D, Standard)
  - [ ] Agregar `Features` (List<string>, ej: ["Dolby Atmos", "Reclining Seats"])
  - [ ] Agregar `IsActive` (bool)

- [ ] **Implementar TheaterRoomsController**
  - [ ] `POST /api/theaterrooms/add-theater-room`
  - [ ] `GET /api/theaterrooms/get-theater-room/{id}`
  - [ ] `PUT /api/theaterrooms/edit-theater-room/{id}`
  - [ ] `DELETE /api/theaterrooms/delete-theater-room/{id}`
  - [ ] `GET /api/theaterrooms/get-all-theater-rooms`

- [ ] **Validaciones**
  - [ ] Validar capacidad = rows × columns
  - [ ] Validar nombre único
  - [ ] Validar rangos (capacity > 0, rows/columns > 0)

- [ ] **Tests Unitarios**
  - [ ] Repository tests
  - [ ] Controller tests
  - [ ] Validation tests

#### Tareas Frontend:
- [ ] **Crear entidad TheaterRoom**
  - [ ] Definir modelo con todos los campos
  - [ ] JSON serialization

- [ ] **Crear TheaterRoomService**
  - [ ] Métodos CRUD completos

- [ ] **Crear AdminTheaterRoomsPage**
  - [ ] Listar salas
  - [ ] CRUD con formularios
  - [ ] Visualizar capacidad y características

---

### 1.3 Backend - Screenings Management (Proyecciones) 📅

**Prioridad:** ALTA

#### Tareas Backend:
- [ ] **Crear IScreeningRepository interface**
  - [ ] `GetByIdAsync(string id)`
  - [ ] `ListAsync()`
  - [ ] `GetByMovieIdAsync(string movieId)` - Proyecciones de una película
  - [ ] `GetByTheaterRoomIdAsync(string roomId)` - Proyecciones de una sala
  - [ ] `GetByDateRangeAsync(DateTime start, DateTime end)` - Proyecciones en rango de fechas
  - [ ] `AddAsync(Screening screening)`
  - [ ] `UpdateAsync(Screening screening)`
  - [ ] `DeleteAsync(string id)`

- [ ] **Actualizar entidad Screening**
  - [ ] Agregar `Price` (decimal, precio del boleto)
  - [ ] Agregar `AvailableSeats` (int, asientos disponibles)
  - [ ] Agregar `IsActive` (bool)
  - [ ] Validar que EndTime > StartTime
  - [ ] Calcular EndTime automáticamente desde Movie.DurationMinutes

- [ ] **Implementar FirestoreScreeningRepository**
  - [ ] Colección: `screenings`
  - [ ] Queries complejas (por película, sala, fecha)
  - [ ] Índices compuestos en Firestore si es necesario

- [ ] **Implementar ScreeningsController**
  - [ ] `POST /api/screenings/add-screening`
  - [ ] `GET /api/screenings/get-screening/{id}`
  - [ ] `PUT /api/screenings/edit-screening/{id}`
  - [ ] `DELETE /api/screenings/delete-screening/{id}`
  - [ ] `GET /api/screenings/get-all-screenings`
  - [ ] `GET /api/screenings/by-movie/{movieId}`
  - [ ] `GET /api/screenings/by-room/{roomId}`
  - [ ] `GET /api/screenings/by-date?start={start}&end={end}`

- [ ] **Lógica de Negocio**
  - [ ] Validar que no haya solapamiento de horarios en la misma sala
  - [ ] Calcular AvailableSeats desde TheaterRoom.Capacity
  - [ ] Validar que MovieId y TheaterRoomId existan

- [ ] **Tests Unitarios**
  - [ ] Tests de solapamiento de horarios
  - [ ] Tests de queries por fecha/película/sala
  - [ ] Validation tests

#### Tareas Frontend:
- [ ] **Crear entidad Screening**
  - [ ] Modelo completo con relaciones

- [ ] **Crear ScreeningService**
  - [ ] Métodos para obtener proyecciones por película
  - [ ] Métodos CRUD (admin)

- [ ] **Actualizar MoviePickerPage**
  - [ ] Mostrar horarios reales desde API
  - [ ] Filtrar por fecha actual/próximos días

- [ ] **Crear AdminScreeningsPage**
  - [ ] CRUD de proyecciones
  - [ ] Calendario/vista de horarios
  - [ ] Validación de conflictos

---

### 1.4 Backend - Food Combos Management (Combos de Alimentos) 🍿

**Prioridad:** MEDIA

#### Tareas Backend:
- [ ] **Crear IFoodComboRepository interface**
  - [ ] CRUD completo

- [ ] **Actualizar entidad FoodCombo**
  - [ ] Agregar `ImageUrl` (string, URL de imagen del combo)
  - [ ] Agregar `IsAvailable` (bool, disponibilidad)
  - [ ] Agregar `Category` (string, ej: "Snacks", "Drinks", "Combos")

- [ ] **Implementar FirestoreFoodComboRepository**
  - [ ] Colección: `foodCombos`

- [ ] **Implementar FoodCombosController**
  - [ ] CRUD completo
  - [ ] `GET /api/foodcombos/available` - Solo combos disponibles

- [ ] **Tests Unitarios**

#### Tareas Frontend:
- [ ] **Crear entidad FoodCombo**

- [ ] **Crear FoodComboService**

- [ ] **Actualizar FoodPage**
  - [ ] Conectar con API real
  - [ ] Mostrar imágenes desde URL
  - [ ] Filtrar solo disponibles

- [ ] **Crear AdminFoodCombosPage**
  - [ ] CRUD completo

---

### 1.5 Backend - Bookings & Seat Management (Reservas y Asientos) 🎟️

**Prioridad:** ALTA

#### Tareas Backend:
- [ ] **Crear entidad Booking**
  ```csharp
  - Id (string)
  - UserId (string)
  - ScreeningId (string)
  - SeatIds (List<string>)
  - TotalPrice (decimal)
  - Status (string: "pending", "confirmed", "cancelled")
  - BookingDate (DateTime)
  - PaymentMethod (string)
  ```

- [ ] **Crear entidad Seat**
  ```csharp
  - Id (string)
  - TheaterRoomId (string)
  - Row (string, ej: "A", "B")
  - Number (int, ej: 1, 2, 3)
  - Type (string: "standard", "vip", "disabled")
  - IsAvailable (bool) - por proyección específica
  ```

- [ ] **Crear IBookingRepository**
  - [ ] CRUD completo
  - [ ] `GetByUserIdAsync(string userId)`
  - [ ] `GetByScreeningIdAsync(string screeningId)`

- [ ] **Crear ISeatRepository**
  - [ ] `GetByTheaterRoomIdAsync(string roomId)`
  - [ ] `GetAvailableSeatsForScreeningAsync(string screeningId)`
  - [ ] `ReserveSeatsAsync(List<string> seatIds, string bookingId)`
  - [ ] `ReleaseSeatsAsync(List<string> seatIds)`

- [ ] **Implementar Repositories en Firestore**
  - [ ] Colección `bookings`
  - [ ] Colección `seats`
  - [ ] Transacciones para reservas atómicas

- [ ] **Crear BookingsController**
  - [ ] `POST /api/bookings/create` - Crear reserva
  - [ ] `GET /api/bookings/{id}` - Obtener reserva
  - [ ] `GET /api/bookings/user/{userId}` - Reservas del usuario
  - [ ] `PUT /api/bookings/{id}/confirm` - Confirmar pago
  - [ ] `PUT /api/bookings/{id}/cancel` - Cancelar reserva
  - [ ] `GET /api/bookings/screening/{screeningId}/seats` - Asientos ocupados

- [ ] **Lógica de Negocio**
  - [ ] Validar disponibilidad de asientos antes de reservar
  - [ ] Actualizar AvailableSeats en Screening al confirmar
  - [ ] Liberar asientos al cancelar reserva
  - [ ] Timeout de reservas pendientes (15 minutos)

- [ ] **Tests Unitarios**
  - [ ] Tests de concurrencia (múltiples usuarios reservando mismos asientos)
  - [ ] Tests de transacciones

#### Tareas Frontend:
- [ ] **Crear entidades Booking y Seat**

- [ ] **Crear BookingService**
  - [ ] `getAvailableSeats(String screeningId)`
  - [ ] `createBooking(Booking booking)`
  - [ ] `confirmBooking(String bookingId)`
  - [ ] `getUserBookings(String userId)`

- [ ] **Actualizar SeatSelectionPage**
  - [ ] Cargar asientos ocupados desde API
  - [ ] Marcar asientos no disponibles
  - [ ] Reservar asientos al confirmar
  - [ ] Timer de 15 minutos para completar compra

- [ ] **Crear MyBookingsPage**
  - [ ] Ver historial de reservas del usuario
  - [ ] Cancelar reservas futuras
  - [ ] Ver detalles de cada reserva

---

## FASE 2: FOOD ORDERS & PAYMENT INTEGRATION (1-2 semanas)

### Objetivo
Implementar el sistema de órdenes de comida y preparar integración de pagos.

---

### 2.1 Backend - Food Orders Management

**Prioridad:** MEDIA

#### Tareas Backend:
- [ ] **Actualizar entidad FoodOrder**
  - [ ] Agregar `BookingId` (string, opcional, asociar con reserva)
  - [ ] Agregar `CreatedAt` (DateTime)
  - [ ] Agregar `UpdatedAt` (DateTime)
  - [ ] Agregar `PaymentMethod` (string)

- [ ] **Crear IFoodOrderRepository**
  - [ ] CRUD completo
  - [ ] `GetByUserIdAsync(string userId)`
  - [ ] `GetPendingOrdersAsync()`

- [ ] **Implementar FirestoreFoodOrderRepository**
  - [ ] Colección: `foodOrders`

- [ ] **Implementar FoodOrdersController**
  - [ ] CRUD completo
  - [ ] `GET /api/foodorders/user/{userId}`
  - [ ] `PUT /api/foodorders/{id}/complete`
  - [ ] `GET /api/foodorders/pending` (admin)

- [ ] **Lógica de Negocio**
  - [ ] Calcular TotalPrice desde FoodCombos
  - [ ] Validar que FoodComboIds existan
  - [ ] Actualizar Status (pending → completed → delivered)

- [ ] **Tests Unitarios**

#### Tareas Frontend:
- [ ] **Actualizar FoodPage**
  - [ ] Agregar carrito de compras
  - [ ] Calcular total dinámicamente
  - [ ] Botón "Confirmar Orden"

- [ ] **Crear FoodOrderService**
  - [ ] `createOrder(FoodOrder order)`
  - [ ] `getUserOrders(String userId)`

- [ ] **Crear MyOrdersPage**
  - [ ] Ver historial de órdenes de comida
  - [ ] Ver estado de orden

---

### 2.2 Payment Integration (Preparación)

**Prioridad:** BAJA (Fase inicial - Mock)

- [ ] **Definir estrategia de pago**
  - [ ] Investigar opciones: Stripe, PayPal, pasarelas locales
  - [ ] Decidir entre pago real o mock para MVP

- [ ] **Backend**
  - [ ] Crear `IPaymentService` interface
  - [ ] Implementar `MockPaymentService` para desarrollo
  - [ ] Endpoints: `/api/payments/process`, `/api/payments/verify`

- [ ] **Frontend**
  - [ ] Crear PaymentPage con formulario mock
  - [ ] Simular proceso de pago
  - [ ] Confirmar Booking tras pago exitoso

---

## FASE 3: ADVANCED FEATURES & UX IMPROVEMENTS (2-3 semanas)

### Objetivo
Mejorar la experiencia de usuario y agregar funcionalidades avanzadas.

---

### 3.1 Frontend - State Management Refactoring

**Prioridad:** MEDIA

- [ ] **Migrar a Riverpod completo**
  - [ ] Crear providers para UserSession (reemplazar Singleton)
  - [ ] Crear providers para cada servicio
  - [ ] StateNotifier para carrito de comida
  - [ ] StateNotifier para selección de asientos
  - [ ] AsyncNotifierProvider para datos de API

- [ ] **Migrar a GoRouter**
  - [ ] Definir rutas completas
  - [ ] Implementar route guards para admin
  - [ ] Deep linking support
  - [ ] Navegación declarativa

---

### 3.2 Frontend - Enhanced UI/UX

**Prioridad:** MEDIA

- [ ] **Movie Details Page**
  - [ ] Vista detallada de película
  - [ ] Reproducir trailer (YouTube embed)
  - [ ] Ver reseñas/ratings
  - [ ] Botón "Ver Horarios"

- [ ] **Search & Filters**
  - [ ] Búsqueda de películas por título
  - [ ] Filtros por género
  - [ ] Filtros por clasificación
  - [ ] Ordenar por rating/fecha

- [ ] **Responsive Design**
  - [ ] Adaptar UI para tablet
  - [ ] Menú lateral para desktop
  - [ ] Grid responsive para películas

- [ ] **Dark/Light Theme**
  - [ ] Implementar theme switcher
  - [ ] Persistir preferencia

- [ ] **Animations**
  - [ ] Transiciones entre páginas
  - [ ] Loading skeletons
  - [ ] Hero animations para posters

---

### 3.3 Backend - Advanced Features

**Prioridad:** MEDIA

- [ ] **Search & Filtering**
  - [ ] `GET /api/movies/search?q={query}`
  - [ ] `GET /api/movies/filter?genre={genre}&rating={min}`
  - [ ] Índices en Firestore para queries eficientes

- [ ] **Statistics & Reports (Admin)**
  - [ ] `GET /api/reports/revenue?start={date}&end={date}`
  - [ ] `GET /api/reports/top-movies`
  - [ ] `GET /api/reports/occupancy-rate`
  - [ ] Dashboard data aggregation

- [ ] **Notifications (Preparación)**
  - [ ] Firebase Cloud Messaging setup
  - [ ] Notificar confirmación de reserva
  - [ ] Recordatorios de proyecciones

---

## FASE 4: ADMIN DASHBOARD & ANALYTICS (1-2 semanas)

### Objetivo
Crear un dashboard administrativo completo con reportes y análisis.

---

### 4.1 Admin Dashboard - Frontend

**Prioridad:** MEDIA

- [ ] **Dashboard Home**
  - [ ] Métricas clave (ventas hoy, reservas activas, ocupación)
  - [ ] Gráficos de tendencias (Chart.js o similar)
  - [ ] Alerts y notificaciones

- [ ] **Gestión Completa**
  - [ ] AdminMoviesPage (CRUD películas)
  - [ ] AdminScreeningsPage (CRUD proyecciones con calendario)
  - [ ] AdminTheaterRoomsPage (CRUD salas)
  - [ ] AdminFoodCombosPage (CRUD combos)
  - [ ] AdminUsersPage (gestión de usuarios)
  - [ ] AdminBookingsPage (ver todas las reservas)
  - [ ] AdminOrdersPage (ver todas las órdenes de comida)

- [ ] **Reports Page**
  - [ ] Reportes de ventas
  - [ ] Películas más vistas
  - [ ] Tasa de ocupación por sala
  - [ ] Exportar a PDF/Excel

---

### 4.2 Admin Dashboard - Backend

**Prioridad:** MEDIA

- [ ] **ReportsController**
  - [ ] Endpoints de estadísticas
  - [ ] Aggregation queries en Firestore
  - [ ] Caching de reportes

- [ ] **Admin Authorization**
  - [ ] Policy-based authorization
  - [ ] Verificar rol admin en todos los endpoints críticos

---

## FASE 5: TESTING, OPTIMIZATION & DEPLOYMENT (1-2 semanas)

### Objetivo
Asegurar calidad, performance y preparar para producción.

---

### 5.1 Testing

**Prioridad:** ALTA

#### Backend:
- [ ] **Unit Tests**
  - [ ] Todos los repositories (coverage > 80%)
  - [ ] Todos los controllers (coverage > 80%)
  - [ ] Servicios de negocio

- [ ] **Integration Tests**
  - [ ] Tests con Firestore Emulator
  - [ ] Tests de endpoints completos
  - [ ] Tests de autenticación

- [ ] **Load Tests**
  - [ ] Simular múltiples usuarios concurrentes
  - [ ] Tests de stress en reservas de asientos

#### Frontend:
- [ ] **Widget Tests**
  - [ ] Tests de componentes clave
  - [ ] Tests de formularios

- [ ] **Integration Tests**
  - [ ] Flujo completo de compra de boletos
  - [ ] Flujo de login y navegación

- [ ] **E2E Tests**
  - [ ] Tests automatizados con Flutter Driver

---

### 5.2 Performance Optimization

**Prioridad:** MEDIA

#### Backend:
- [ ] **Caching**
  - [ ] Implementar Redis/Memory Cache para datos frecuentes
  - [ ] Cache de películas activas
  - [ ] Cache de proyecciones del día

- [ ] **Database Optimization**
  - [ ] Crear índices compuestos en Firestore
  - [ ] Optimizar queries costosas
  - [ ] Paginación en listas largas

- [ ] **API Optimization**
  - [ ] Compression (gzip)
  - [ ] Rate limiting
  - [ ] CDN para imágenes (Cloudinary/Firebase Storage)

#### Frontend:
- [ ] **Image Optimization**
  - [ ] Lazy loading de imágenes
  - [ ] Cached network images
  - [ ] Resize de imágenes grandes

- [ ] **Code Splitting**
  - [ ] Lazy loading de rutas
  - [ ] Tree shaking

- [ ] **Performance Profiling**
  - [ ] Identificar cuellos de botella
  - [ ] Optimizar rebuilds innecesarios

---

### 5.3 Security Hardening

**Prioridad:** ALTA

#### Backend:
- [ ] **Autenticación Robusta**
  - [ ] Implementar refresh tokens
  - [ ] Token expiration handling
  - [ ] Logout functionality

- [ ] **Autorización**
  - [ ] Verificar permisos en TODOS los endpoints
  - [ ] Prevenir acceso no autorizado a recursos

- [ ] **Input Validation**
  - [ ] Validar TODOS los inputs
  - [ ] Sanitizar datos antes de guardar
  - [ ] Prevenir SQL Injection (no aplica, pero sí NoSQL injection)

- [ ] **HTTPS**
  - [ ] Forzar HTTPS en producción
  - [ ] Certificado SSL válido

- [ ] **Secrets Management**
  - [ ] Mover Firebase key a variables de entorno
  - [ ] NO commitear credenciales
  - [ ] Usar Azure Key Vault / AWS Secrets Manager

#### Frontend:
- [ ] **Token Storage**
  - [ ] Secure storage con flutter_secure_storage
  - [ ] No almacenar en localStorage plano

- [ ] **HTTPS Only**
  - [ ] Forzar conexiones HTTPS

---

### 5.4 Deployment Preparation

**Prioridad:** ALTA

#### Backend:
- [ ] **Dockerize Backend**
  - [ ] Crear Dockerfile
  - [ ] Docker Compose para desarrollo local
  - [ ] Multi-stage build para producción

- [ ] **CI/CD Pipeline**
  - [ ] GitHub Actions para build y tests
  - [ ] Deploy automático a staging
  - [ ] Deploy manual a producción

- [ ] **Hosting**
  - [ ] Decidir plataforma (Azure, AWS, Google Cloud Run)
  - [ ] Configurar dominio y DNS
  - [ ] Configurar SSL

#### Frontend:
- [ ] **Flutter Web Build**
  - [ ] Optimizar build de producción
  - [ ] Configurar base href correcto

- [ ] **Android Build**
  - [ ] Generar APK/AAB firmado
  - [ ] Configurar Play Store metadata

- [ ] **iOS Build (opcional)**
  - [ ] Configurar provisioning profiles
  - [ ] Build para App Store

- [ ] **Hosting**
  - [ ] Firebase Hosting para Web
  - [ ] Publicar en Google Play Store
  - [ ] Publicar en Apple App Store (opcional)

---

### 5.5 Monitoring & Logging

**Prioridad:** MEDIA

#### Backend:
- [ ] **Application Insights / Datadog**
  - [ ] Configurar telemetría
  - [ ] Alertas de errores
  - [ ] Performance monitoring

- [ ] **Structured Logging**
  - [ ] Logs centralizados (Serilog ya configurado)
  - [ ] Log levels apropiados
  - [ ] Correlation IDs

#### Frontend:
- [ ] **Crash Reporting**
  - [ ] Firebase Crashlytics
  - [ ] Sentry integration

- [ ] **Analytics**
  - [ ] Firebase Analytics
  - [ ] Track user flows
  - [ ] Conversion funnels

---

## FASE 6: POST-LAUNCH FEATURES (Futuro)

### Funcionalidades Avanzadas (Backlog)

- [ ] **Multi-language Support (i18n)**
  - [ ] Backend: Accept-Language header
  - [ ] Frontend: flutter_localizations

- [ ] **Social Features**
  - [ ] Invitar amigos a proyecciones
  - [ ] Compartir en redes sociales
  - [ ] Reseñas de películas

- [ ] **Loyalty Program**
  - [ ] Sistema de puntos
  - [ ] Descuentos y promociones
  - [ ] Membresías

- [ ] **Advanced Search**
  - [ ] Búsqueda por actor, director
  - [ ] Recomendaciones personalizadas (ML)

- [ ] **Push Notifications**
  - [ ] Firebase Cloud Messaging
  - [ ] Notificaciones de estrenos
  - [ ] Ofertas especiales

- [ ] **QR Code Tickets**
  - [ ] Generar QR al confirmar reserva
  - [ ] Escanear en entrada del cine

- [ ] **Integration with External APIs**
  - [ ] TMDB API para metadata de películas
  - [ ] OMDb API para ratings

---

## Resumen de Prioridades

### 🔴 PRIORIDAD ALTA (MVP Core)
1. Movies CRUD completo (Backend + Frontend)
2. Theater Rooms CRUD completo
3. Screenings CRUD completo
4. Bookings & Seat Management
5. User Authentication & Authorization
6. Security Hardening
7. Deployment Preparation

### 🟡 PRIORIDAD MEDIA (Post-MVP)
1. Food Orders completo
2. Admin Dashboard completo
3. State Management refactoring (Riverpod + GoRouter)
4. Enhanced UI/UX
5. Performance Optimization
6. Reports & Analytics

### 🟢 PRIORIDAD BAJA (Nice to Have)
1. Payment Integration real (Mock primero)
2. Advanced Search & Filters
3. Notifications
4. Multi-language
5. Social Features

---

## Timeline Estimado

| Fase | Duración | Hitos |
|------|----------|-------|
| Fase 1 | 2-3 semanas | CRUD de todas las entidades core |
| Fase 2 | 1-2 semanas | Food Orders + Mock Payments |
| Fase 3 | 2-3 semanas | UX improvements + Advanced features |
| Fase 4 | 1-2 semanas | Admin Dashboard completo |
| Fase 5 | 1-2 semanas | Testing, Optimization, Deployment |
| **TOTAL** | **7-12 semanas** | MVP listo para producción |

---

## Próximos Pasos Inmediatos

1. **Revisar y aprobar este plan** ✅
2. **Configurar entorno de desarrollo** (Firebase keys, etc.)
3. **Comenzar Fase 1.1: Movies CRUD** (Backend primero)
4. **Setup de testing frameworks**
5. **Crear branches de desarrollo** (feature/movies-crud, etc.)

---

## Notas Importantes

- **Clean Architecture:** Mantener separación de capas en todo momento
- **Testing:** Escribir tests en paralelo al desarrollo, no después
- **Documentación:** Actualizar docs al completar cada feature
- **Code Review:** Implementar PR reviews antes de merge a main
- **Versionado Semántico:** Usar SemVer para releases (v1.0.0, v1.1.0, etc.)

---

**Contacto:** Para preguntas o ajustes al plan, consultar con el equipo de desarrollo.
