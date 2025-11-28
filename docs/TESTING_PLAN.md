# 🧪 Plan de Pruebas Completo - Sistema de Cine

**Proyecto**: MagiaCinema
**Versión**: 1.0
**Fecha**: 28 de Noviembre, 2025
**Última Actualización**: Post-optimización Firestore

---

## 📋 Tabla de Contenidos

1. [Configuración Inicial](#configuración-inicial)
2. [Fase 1: Infraestructura Base](#fase-1-infraestructura-base)
3. [Fase 2: Contenido (Películas)](#fase-2-contenido-películas)
4. [Fase 3: Programación (Screenings)](#fase-3-programación-screenings)
5. [Fase 4: Sistema de Reservas](#fase-4-sistema-de-reservas)
6. [Fase 5: Pagos y Facturación](#fase-5-pagos-y-facturación)
7. [Fase 6: Comida y Bebidas](#fase-6-comida-y-bebidas)
8. [Fase 7: Reportes y Auditoría](#fase-7-reportes-y-auditoría)
9. [Fase 8: Chatbot IA](#fase-8-chatbot-ia)
10. [Casos de Prueba Integrados](#casos-de-prueba-integrados)
11. [Preparación para Demo](#preparación-para-demo)

---

## 🚀 Configuración Inicial

### Pre-requisitos
- ✅ Backend compilado: `dotnet build`
- ✅ Frontend compilado: `flutter pub get && flutter run`
- ✅ Firebase configurado (Plan Blaze activado post-incidente)
- ✅ Base de datos LIMPIA (ejecutar cleanup si es necesario)

### Activar Audit Logs para Pruebas (Opcional)
```json
// appsettings.Development.json
"FeatureManagement": {
  "AuditLogging": true  // Cambiar a true para ver logs
}
```

⚠️ **Nota**: Dejar en `false` para uso diario, activar solo para demos o cuando necesites auditar acciones.

---

## 🏗️ Fase 1: Infraestructura Base

### 1.1 Crear Ubicaciones de Cine (Cinema Locations)

#### Endpoint
```http
POST /api/cinemalocations/create-cinema-location
Content-Type: application/json

{
  "name": "MagiaCinema San José Centro",
  "address": "Avenida Central, San José, Costa Rica",
  "city": "San José",
  "state": "San José",
  "zipCode": "10101",
  "phone": "+506 2222-3333",
  "email": "sanjose@magiacinema.com",
  "amenities": ["Estacionamiento", "Cafetería", "3D", "IMAX"]
}
```

#### Datos de Prueba Sugeridos
| Nombre | Ciudad | Características |
|--------|--------|-----------------|
| MagiaCinema San José Centro | San José | Estacionamiento, Cafetería, 3D |
| MagiaCinema Escazú | Escazú | IMAX, 4DX, VIP |
| MagiaCinema Heredia | Heredia | 3D, Cafetería |
| MagiaCinema Cartago | Cartago | Estacionamiento, 3D |
| MagiaCinema Alajuela | Alajuela | 3D, Cafetería, Estacionamiento |

#### Verificación
```http
GET /api/cinemalocations/get-all-cinema-locations
```
**Resultado Esperado**: Lista con 5 cines creados

---

### 1.2 Crear Salas de Teatro (Theater Rooms)

#### Endpoint Manual (Por Sala)
```http
POST /api/theaterrooms/create-theater-room
Content-Type: application/json

{
  "cinemaId": "{ID_del_cine}",
  "name": "Sala 1 - VIP",
  "capacity": 80,
  "roomType": "VIP",
  "has3D": true,
  "hasIMAX": false,
  "seatConfiguration": {
    "rows": 8,
    "columns": 10,
    "seats": [
      {"row": 0, "col": 0, "type": "vip"},
      {"row": 0, "col": 1, "type": "vip"},
      ...
    ]
  }
}
```

#### Endpoint Automático (Seed 20 Salas)
```http
POST /api/theaterrooms/seed
```
**Resultado**:
- 15 salas normales (capacidad 96, configuración 8x12)
- 5 salas VIP (capacidad 72, configuración 6x12, asientos premium)

**Distribución**:
- Cada cine recibe 3-5 salas aleatorias
- Salas numeradas del 1-20
- Configuración realista con asientos empty, regular, vip, wheelchair

#### Verificación
```http
GET /api/theaterrooms/get-all-theater-rooms
```
**Resultado Esperado**: 20 salas distribuidas entre los 5 cines

---

## 🎬 Fase 2: Contenido (Películas)

### 2.1 Cargar Películas desde TMDB

#### Endpoint de Seed
```http
POST /api/movies-seeder/seed-from-tmdb?page=1
```

**Parámetros**:
- `page`: Número de página de TMDB (1-10 recomendado)
- Cada página trae ~20 películas

**Proceso**:
1. Llama a TMDB API
2. Descarga metadata (título, descripción, rating, duración)
3. Descarga imágenes (poster, backdrop)
4. Sube imágenes a Cloudinary
5. Guarda película en Firestore

**Resultado**: ~20 películas por página

#### Películas Mínimas Recomendadas
```http
POST /api/movies-seeder/seed-from-tmdb?page=1
POST /api/movies-seeder/seed-from-tmdb?page=2
```
**Total**: ~40 películas (suficiente para demo)

#### Verificación
```http
GET /api/movies/get-all-movies
```

**Revisar**:
- ✅ Películas tienen `posterUrl` (Cloudinary)
- ✅ Películas tienen `backdropUrl` (Cloudinary)
- ✅ Algunas tienen `isNew: true` ("En Cartelera")
- ✅ Ratings variados (0-10)

---

### 2.2 Categorización de Películas

Las películas se auto-categorizan en 3 grupos para el frontend:

| Categoría | Criterio |
|-----------|----------|
| **En Cartelera** | `isNew === true` |
| **Más Populares** | Top 8 por `rating` (descendente) |
| **Próximamente** | `isNew === false` y no en Top 8 |

**Nota**: Solo "En Cartelera" y "Más Populares" tendrán screenings automáticos.

---

## 📅 Fase 3: Programación (Screenings)

### 3.1 Seed Mínimo para Pruebas Diarias

```http
POST /api/minimal-seed/create-today-screenings
```

**Características**:
- ✅ 2 screenings por cine (5 cines × 2 = 10 screenings)
- ✅ Solo del día actual
- ✅ Horarios: 5:30 PM y 9:00 PM
- ✅ Usa películas "En Cartelera" o top rated
- ✅ Distribuye películas equitativamente entre cines

**Lecturas Firestore**: ~50 (vs 2,500 del seed antiguo)

#### Verificación
```http
GET /api/screenings/future?limit=50
```
**Resultado Esperado**: 10 screenings del día actual

---

### 3.2 Seed para Demo (Datos Realistas)

```http
POST /api/minimal-seed/create-demo-screenings
```

**Características**:
- ✅ 6 screenings por cine (5 cines × 6 = 30 screenings)
- ✅ 2 pasadas (ayer - completadas)
- ✅ 1 actual (en progreso AHORA)
- ✅ 3 futuras (hoy noche + mañana)

**Beneficio**: Permite probar bookings de funciones pasadas, actuales y futuras

---

### 3.3 Consultar Screenings (Paginados)

#### Todos los Screenings (Paginado)
```http
GET /api/screenings/paginated?pageNumber=1&pageSize=50
```

#### Solo Funciones Futuras (Recomendado)
```http
GET /api/screenings/future?limit=20
```

#### Por Película
```http
GET /api/screenings/by-movie/{movieId}?limit=10
```

#### Por Cine
```http
GET /api/screenings/by-cinema/{cinemaId}?limit=10
```

---

## 🎟️ Fase 4: Sistema de Reservas

### 4.1 Flujo Completo de Reserva

#### Paso 1: Usuario Selecciona Screening
**Frontend**: Usuario ve lista de películas → Selecciona horario → Ve asientos disponibles

**Backend Query**:
```http
GET /api/screenings/by-movie/{movieId}?limit=5
```

---

#### Paso 2: Ver Asientos Disponibles
**Frontend**: Muestra configuración de sala con asientos ocupados

**Backend Query**:
```http
GET /api/screenings/get-screening/{screeningId}
```

**Respuesta incluye**:
- `theaterRoomId`: ID de la sala
- `startTime`, `endTime`: Horario
- `price`: Precio por boleto

**Luego**:
```http
GET /api/theaterrooms/get-theater-room/{theaterRoomId}
```

**Respuesta incluye**:
- `seatConfiguration.seats[]`: Configuración completa de asientos
  - `row`, `col`, `type` (normal/vip/wheelchair/empty)

**Y consultar asientos ocupados**:
```http
GET /api/bookings/occupied-seats/{screeningId}
```

**Respuesta**:
```json
["R1S1", "R1S2", "R2S5", "R3S10"]  // IDs de asientos ocupados
```

---

#### Paso 3: Crear Pre-Reserva (Booking)
**Frontend**: Usuario selecciona asientos (máx 8) → Click "Continuar"

**Backend**:
```http
POST /api/bookings/create-booking
Content-Type: application/json

{
  "userId": "{firebase_uid}",
  "screeningId": "{screening_id}",
  "seatNumbers": ["R3S5", "R3S6", "R3S7"],
  "ticketPrice": 4500.0,
  "foodOrderId": null  // Opcional, se agrega después si compra comida
}
```

**Importante**: El backend calcula automáticamente:
- `ticketQuantity`: Largo del array `seatNumbers`
- `subtotalTickets`: `ticketQuantity × ticketPrice`
- `subtotalFood`: Si hay `foodOrderId`
- `tax`: 13% del subtotal
- `total`: Subtotal + tax

**Respuesta**:
```json
{
  "success": true,
  "booking": {
    "id": "booking_123",
    "userId": "user_456",
    "screeningId": "screening_789",
    "seatNumbers": ["R3S5", "R3S6", "R3S7"],
    "ticketQuantity": 3,
    "ticketPrice": 4500.0,
    "subtotalTickets": 13500.0,
    "subtotalFood": 0.0,
    "tax": 1755.0,
    "total": 15255.0,  // ← ESTE ES EL TOTAL PARA EL PAGO
    "status": "pending",
    "createdAt": "2025-11-28T..."
  }
}
```

**Estado**: `pending` (no confirmado hasta pagar)

---

## 🍿 Fase 5: Comida y Bebidas

### 5.1 Seed de Combos de Comida

```http
POST /api/foodcombos/seed
```

**Resultado**: 17 combos predefinidos:
- 7 Combos de Palomitas (Individual/Grande/Premium/Familiar)
- 3 Combos de Nachos
- 4 Combos de Hot Dogs
- 3 Combos de Bebidas

Precios en colones: ₡1,200 - ₡9,500

---

### 5.2 Crear Orden de Comida (Antes de Pago)

**Frontend**: Usuario agrega combos al carrito

**Backend**:
```http
POST /api/foodorders/create-food-order
Content-Type: application/json

{
  "userId": "{firebase_uid}",
  "items": [
    {
      "foodComboId": "{combo_id_1}",
      "quantity": 2,
      "price": 3500.0
    },
    {
      "foodComboId": "{combo_id_2}",
      "quantity": 1,
      "price": 5500.0
    }
  ],
  "status": "pending"
}
```

**Respuesta**:
```json
{
  "success": true,
  "foodOrder": {
    "id": "food_order_123",
    "userId": "user_456",
    "items": [...],
    "totalPrice": 12500.0,  // (3500×2) + (5500×1)
    "status": "pending",
    "createdAt": "..."
  }
}
```

---

### 5.3 Actualizar Booking con Comida

```http
PUT /api/bookings/edit-booking/{bookingId}
Content-Type: application/json

{
  "id": "{booking_id}",
  "userId": "{user_id}",
  "screeningId": "{screening_id}",
  "seatNumbers": ["R3S5", "R3S6", "R3S7"],
  "ticketPrice": 4500.0,
  "foodOrderId": "food_order_123"  // ← Agregar food order
}
```

**Backend recalcula**:
- `subtotalFood`: 12500.0
- `tax`: 13% de (13500 + 12500) = 3380.0
- `total`: 29380.0

---

## 💳 Fase 6: Pagos y Facturación

### 6.1 Procesar Pago Simulado

**Frontend**: Usuario confirma compra → Ingresa tarjeta (simulada)

**Backend**:
```http
POST /api/payments/process
Content-Type: application/json

{
  "bookingId": "{booking_id}",
  "amount": 29380.0,  // ← DEBE COINCIDIR con booking.total
  "paymentMethod": "credit_card",
  "cardNumber": "4111111111111111",
  "cardHolderName": "Juan Pérez",
  "expirationDate": "12/25",
  "cvv": "123"
}
```

**Proceso Backend**:
1. ✅ Valida que `amount === booking.total`
2. ✅ Simula procesamiento de pago (90% éxito, 10% fallo)
3. ✅ Si éxito:
   - Crea `Payment` (status: completed)
   - Actualiza `Booking` (status: confirmed)
   - Genera `Tickets` (1 por asiento con QR único)
   - Genera `Invoice` (con número secuencial INV-0001)
   - Envía 3 emails:
     - Confirmación de reserva
     - Boletos con QR codes
     - Factura (PDF adjunto)

**Respuesta Exitosa**:
```json
{
  "success": true,
  "payment": {
    "id": "payment_123",
    "bookingId": "booking_456",
    "amount": 29380.0,
    "status": "completed",
    "transactionId": "TXN-20251128-123456",
    "createdAt": "..."
  },
  "tickets": [
    {
      "id": "ticket_1",
      "bookingId": "booking_456",
      "seatNumber": "R3S5",
      "qrCode": "base64_encoded_qr_image...",
      "status": "valid"
    },
    // ... más tickets
  ],
  "invoice": {
    "id": "invoice_1",
    "invoiceNumber": "INV-0001",
    "totalAmount": 29380.0,
    "createdAt": "..."
  }
}
```

---

### 6.2 Verificar Emails Enviados (SendGrid)

**Emails que se envían**:

1. **Confirmación de Reserva**
   - Para: `{user_email}`
   - Asunto: "✅ Reserva Confirmada - MagiaCinema"
   - Contenido: Detalles de película, horario, asientos

2. **Boletos Electrónicos**
   - Para: `{user_email}`
   - Asunto: "🎟️ Tus Boletos - {Nombre Película}"
   - Contenido: QR codes (1 por asiento)

3. **Factura**
   - Para: `{user_email}`
   - Asunto: "🧾 Tu Factura - MagiaCinema"
   - Contenido: Desglose detallado + total

**Verificar en**:
- SendGrid Dashboard: https://app.sendgrid.com/
- O revisar logs del backend para confirmación

---

## 📊 Fase 7: Reportes y Auditoría

### 7.1 Dashboard de Reportes

```http
GET /api/reports/dashboard-summary
```

**Respuesta**:
```json
{
  "totalMovies": 40,
  "totalScreenings": 30,
  "todayScreenings": 10,
  "totalBookings": 15,
  "todayBookings": 5,
  "totalUsers": 8,
  "todayRevenue": 125000.0,
  "totalFoodCombos": 17
}
```

---

### 7.2 Reporte de Ventas

```http
GET /api/reports/sales?startDate=2025-11-27&endDate=2025-11-28
```

**Respuesta**:
```json
{
  "totalSales": 250000.0,
  "totalBookings": 20,
  "averageTicketPrice": 4500.0,
  "salesByDay": [
    {"date": "2025-11-27", "sales": 120000.0, "bookings": 10},
    {"date": "2025-11-28", "sales": 130000.0, "bookings": 10}
  ]
}
```

---

### 7.3 Reporte de Ingresos

```http
GET /api/reports/revenue?startDate=2025-11-01&endDate=2025-11-30
```

**Respuesta**:
```json
{
  "totalRevenue": 1500000.0,
  "ticketRevenue": 1100000.0,
  "foodRevenue": 400000.0,
  "revenueByDay": [...]
}
```

---

### 7.4 Audit Logs (Si Feature Flag Activo)

```http
GET /api/auditlog/get-all-audit-logs?limit=50
```

**Respuesta**: Logs de todas las acciones (CREATE/UPDATE/DELETE) en el sistema

---

## 🤖 Fase 8: Chatbot IA

### 8.1 Consulta Simple

```http
POST /api/chat/ask
Content-Type: application/json

{
  "message": "¿Qué películas están en cartelera?"
}
```

**Respuesta**:
```json
{
  "response": "Actualmente tenemos las siguientes películas en cartelera:\n\n1. Dune: Part Two (Sci-Fi) - Rating: 8.5\n2. Poor Things (Drama) - Rating: 8.2\n..."
}
```

---

### 8.2 Consultas Soportadas

| Pregunta | Backend Action |
|----------|----------------|
| "¿Qué películas hay?" | Lista películas "En Cartelera" |
| "Películas de acción" | Filtra por género |
| "¿Qué horarios hay?" | Lista screenings futuros |
| "Recomiéndame una película" | Ordena por rating |
| "¿Dónde están los cines?" | Lista cinema locations |

---

## 🎭 Fase 9: Casos de Prueba Integrados

### Caso 1: Reserva Simple (Sin Comida)

**Flujo**:
1. Usuario ve películas → `GET /api/screenings/by-movie/{id}`
2. Usuario selecciona screening → `GET /api/screenings/get-screening/{id}`
3. Usuario ve asientos → `GET /api/bookings/occupied-seats/{id}`
4. Usuario selecciona 2 asientos → `POST /api/bookings/create-booking`
5. Usuario paga → `POST /api/payments/process`

**Resultado**:
- ✅ Booking confirmado
- ✅ 2 tickets generados con QR
- ✅ Invoice generada
- ✅ 3 emails enviados

---

### Caso 2: Reserva con Comida

**Flujo**:
1-4. Igual que Caso 1
5. Usuario agrega combos → `POST /api/foodorders/create-food-order`
6. Actualizar booking → `PUT /api/bookings/edit-booking/{id}` (agregar foodOrderId)
7. Usuario paga → `POST /api/payments/process`

**Resultado**:
- ✅ Booking con comida confirmado
- ✅ Tickets + Invoice incluyen comida
- ✅ Emails con desglose completo

---

### Caso 3: Pago Fallido

**Flujo**:
1-4. Igual que Caso 1
5. Usuario paga con monto incorrecto → `POST /api/payments/process` (amount ≠ booking.total)

**Resultado**:
- ❌ Error 400: "Payment amount does not match booking total"
- ❌ Booking permanece en status `pending`
- ❌ No se generan tickets ni invoice

---

### Caso 4: Flujo Completo Admin

**Flujo**:
1. Admin crea cine → `POST /api/cinemalocations/create-cinema-location`
2. Admin crea salas → `POST /api/theaterrooms/seed`
3. Admin carga películas → `POST /api/movies-seeder/seed-from-tmdb?page=1`
4. Admin crea screenings → `POST /api/minimal-seed/create-today-screenings`
5. Admin revisa reportes → `GET /api/reports/dashboard-summary`

---

## 🎬 Fase 10: Preparación para Demo

### Pre-Demo Checklist

#### 1. Limpiar Base de Datos
```http
POST /api/cleanup/clear-all-data
```

#### 2. Verificar Infraestructura
```http
GET /api/cinemalocations/get-all-cinema-locations  // Debe haber 5 cines
GET /api/theaterrooms/get-all-theater-rooms        // Debe haber 20 salas
```

Si faltan:
```http
POST /api/theaterrooms/seed  // Solo si no hay salas
```

#### 3. Cargar Películas
```http
POST /api/movies-seeder/seed-from-tmdb?page=1
POST /api/movies-seeder/seed-from-tmdb?page=2
```

#### 4. Cargar Combos de Comida
```http
POST /api/foodcombos/seed
```

#### 5. Crear Screenings para Demo
```http
POST /api/minimal-seed/create-demo-screenings
```

**Resultado**: 30 screenings (6 por cine) con datos realistas:
- Pasadas (completadas)
- Actual (en progreso)
- Futuras (programadas)

#### 6. Activar Audit Logs
```json
// appsettings.Development.json
"FeatureManagement": {
  "AuditLogging": true  // ← Activar para demo
}
```

#### 7. Verificar Emails (SendGrid)
- ✅ API Key configurado
- ✅ Sender verificado
- ✅ No hay errores en logs

---

### Durante el Demo

#### Demostración de Features

1. **Admin Panel**:
   - Crear nuevo cine
   - Crear sala con configuración de asientos
   - Ver reportes en vivo

2. **Usuario - Exploración**:
   - Ver películas en cartelera
   - Chatbot: "¿Qué películas recomiendas?"
   - Ver horarios disponibles

3. **Usuario - Reserva**:
   - Seleccionar película → Horario → Asientos
   - Agregar combos de comida
   - Pagar (tarjeta simulada)
   - Mostrar emails con QR codes

4. **Validación**:
   - Verificar asientos ocupados en siguiente booking
   - Mostrar invoice generada
   - Revisar audit logs de acciones

---

## 📈 Métricas de Éxito

### Performance (Post-Optimización)

| Métrica | Antes | Después | ✅ |
|---------|-------|---------|---|
| Lecturas/día | 27,000 | 1,050 | **-97%** |
| Seed screenings | 2,500 lecturas | 50 lecturas | **-98%** |
| Get all endpoints | Sin límite | Paginado (50 max) | ✅ |
| Audit logs | Siempre ON | Feature flag | ✅ |

### Funcionalidad

- ✅ 5 Cinema Locations
- ✅ 20 Theater Rooms (15 normales + 5 VIP)
- ✅ 40+ Películas (TMDB + Cloudinary)
- ✅ 30 Screenings (realistas para demo)
- ✅ 17 Food Combos
- ✅ Sistema de pagos funcionando
- ✅ Emails automáticos (3 tipos)
- ✅ QR codes generados
- ✅ Invoices numeradas
- ✅ Reportes completos
- ✅ Chatbot IA integrado

---

## 🚨 Troubleshooting

### Error: "Firebase quota exceeded"
**Solución**:
1. Actualizar a Plan Blaze
2. Esperar reset diario (~2 AM Costa Rica)
3. Usar endpoints paginados

### Error: "Payment amount does not match booking total"
**Causa**: Frontend calculó total localmente, difiere del backend
**Solución**: Usar `booking.total` del backend response para pago

### Error: "No se envían emails"
**Verificar**:
1. SendGrid API Key configurado
2. Sender verificado en SendGrid
3. Logs del backend para errores SMTP

### Error: "Asientos no se marcan como ocupados"
**Causa**: Booking no confirmado (status pending)
**Solución**: Completar pago para confirmar booking

---

## 📞 Soporte

**Documentación Relacionada**:
- [FIRESTORE_QUOTA_INCIDENT.md](./FIRESTORE_QUOTA_INCIDENT.md) - Análisis del incidente
- [API Documentation](./API_DOCS.md) - Todos los endpoints
- [Frontend Testing Guide](./FRONTEND_TESTING.md) - Pruebas de UI

**Contacto**:
- Desarrollado por: Claude Code
- Fecha: 28 de Noviembre, 2025

---

**🎯 Listo para Demo**: Sigue este plan paso a paso para asegurar un demo exitoso sin exceders cuotas de Firestore.
