# 📋 DOCUMENTACIÓN DEL SISTEMA DE PAGOS, FACTURACIÓN Y BOLETOS

**Proyecto**: Cinema - Sistema de Gestión de Cine
**Fecha de Implementación**: 25 de Noviembre, 2025
**Versión**: 1.0

---

## 📑 ÍNDICE

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Entidades del Dominio](#entidades-del-dominio)
4. [Servicios Implementados](#servicios-implementados)
5. [Flujo de Compra Completo](#flujo-de-compra-completo)
6. [Configuración Requerida](#configuración-requerida)
7. [APIs y Endpoints](#apis-y-endpoints)
8. [Casos de Uso](#casos-de-uso)
9. [Testing y Validaciones](#testing-y-validaciones)
10. [Troubleshooting](#troubleshooting)

---

## 1. RESUMEN EJECUTIVO

### ¿Qué se implementó?

Sistema completo de gestión de reservas, pagos simulados, generación de boletos digitales con QR, facturación automática y notificaciones por email.

### Componentes Principales

- ✅ **4 Nuevas Entidades** (Booking, Payment, Ticket, Invoice)
- ✅ **4 Servicios Firestore** para persistencia de datos
- ✅ **5 Servicios de Lógica de Negocio**:
  - QRCodeService (generación de códigos QR)
  - PaymentSimulationService (procesamiento de pagos simulados)
  - EmailService (envío de correos con templates HTML)
  - TicketService (gestión de boletos digitales)
  - InvoiceService (generación de facturas y PDFs)

### Tecnologías Utilizadas

- **SendGrid** (v9.29.3) - Servicio de email
- **QRCoder** (v1.6.0) - Generación de códigos QR
- **QuestPDF** (v2024.12.3) - Generación de PDFs
- **Firestore** - Base de datos NoSQL
- **.NET 9.0** - Framework

---

## 2. ARQUITECTURA DEL SISTEMA

### Diagrama de Capas

```
┌─────────────────────────────────────────┐
│          FRONTEND (Flutter)             │
│   - Selección de función y asientos     │
│   - Proceso de pago                     │
│   - Visualización de boletos            │
└─────────────────┬───────────────────────┘
                  │ HTTPS/REST
┌─────────────────▼───────────────────────┐
│      CONTROLLERS (Cinema.Api)           │
│  - BookingsController                   │
│  - PaymentsController                   │
│  - TicketsController                    │
│  - InvoicesController                   │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│     BUSINESS SERVICES (Cinema.Api)      │
│  - TicketService                        │
│  - InvoiceService                       │
│  - PaymentSimulationService             │
│  - QRCodeService                        │
│  - EmailService                         │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│   FIRESTORE SERVICES (Cinema.Api)       │
│  - FirestoreBookingService              │
│  - FirestorePaymentService              │
│  - FirestoreTicketService               │
│  - FirestoreInvoiceService              │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      FIRESTORE DATABASE (Cloud)         │
│  Collections:                           │
│    - bookings/                          │
│    - payments/                          │
│    - tickets/                           │
│    - invoices/                          │
│    - counters/ (para números de factura)│
└─────────────────────────────────────────┘
```

---

## 3. ENTIDADES DEL DOMINIO

### 3.1 Booking (Reserva)

**Ubicación**: `Cinema.Domain/Entities/Booking.cs`

**Propósito**: Representa una reserva de boletos realizada por un usuario.

**Propiedades Clave**:
- `Id`: Identificador único
- `UserId`: Usuario que realiza la reserva
- `ScreeningId`: Función de película
- `SeatNumbers`: Lista de asientos (ej: ["A1", "A2"])
- `Total`: Monto total a pagar
- `Status`: "pending", "confirmed", "cancelled"
- `PaymentId`: Referencia al pago

**Estados del Ciclo de Vida**:
```
pending → (pago exitoso) → confirmed
pending → (usuario cancela) → cancelled
```

### 3.2 Payment (Pago)

**Ubicación**: `Cinema.Domain/Entities/Payment.cs`

**Propósito**: Representa un pago simulado (NO REAL).

**Propiedades Clave**:
- `TransactionId`: ID de transacción generado
- `CardLastFourDigits`: Últimos 4 dígitos (para visualización)
- `CardBrand`: Visa, MasterCard, etc.
- `Status`: "pending", "approved", "rejected"
- `RejectionReason`: Motivo si fue rechazado

**Nota Importante**: Este sistema NO procesa pagos reales. Es puramente educativo.

### 3.3 Ticket (Boleto Digital)

**Ubicación**: `Cinema.Domain/Entities/Ticket.cs`

**Propósito**: Boleto digital con código QR único por asiento.

**Propiedades Clave**:
- `QrCode`: Imagen QR en Base64
- `QrCodeData`: String codificado en el QR
- `IsUsed`: Si ya fue escaneado
- `ExpiresAt`: Fecha de expiración (ShowTime + 30 min)

**Formato del QR**:
```
TICKET:id=ABC123|user=UID456|screening=SCR789|seat=A1|showtime=1737000000
```

### 3.4 Invoice (Factura)

**Ubicación**: `Cinema.Domain/Entities/Invoice.cs`

**Propósito**: Factura generada post-compra.

**Propiedades Clave**:
- `InvoiceNumber`: Número secuencial (ej: INV-2025-0001)
- `Items`: Lista de InvoiceItem (boletos, comida)
- `Subtotal`, `Tax`, `Total`

**Numeración Automática**: Se genera usando un contador en Firestore (`counters/invoice_counter`).

---

## 4. SERVICIOS IMPLEMENTADOS

### 4.1 FirestoreBookingService

**Archivo**: `Cinema.Api/Services/FirestoreBookingService.cs`

**Métodos Principales**:
- `AddBookingAsync(Booking)` - Crear reserva
- `GetBookingAsync(id)` - Obtener por ID
- `GetBookingsByUserIdAsync(userId)` - Reservas del usuario
- `ConfirmBookingAsync(bookingId, paymentId)` - Confirmar después de pago
- `CancelBookingAsync(bookingId)` - Cancelar reserva

**Colección Firestore**: `bookings/`

### 4.2 FirestorePaymentService

**Archivo**: `Cinema.Api/Services/FirestorePaymentService.cs`

**Métodos Principales**:
- `AddPaymentAsync(Payment)` - Crear pago
- `GetPaymentByBookingIdAsync(bookingId)` - Pago de una reserva
- `ApprovePaymentAsync(paymentId, transactionId)` - Aprobar pago
- `RejectPaymentAsync(paymentId, reason)` - Rechazar pago

**Colección Firestore**: `payments/`

### 4.3 FirestoreTicketService

**Archivo**: `Cinema.Api/Services/FirestoreTicketService.cs`

**Métodos Principales**:
- `AddTicketsBatchAsync(List<Ticket>)` - Crear múltiples boletos
- `GetTicketsByUserIdAsync(userId)` - Boletos del usuario
- `GetTicketByQrCodeDataAsync(qrCodeData)` - Buscar por QR
- `UseTicketAsync(ticketId)` - Marcar como usado

**Colección Firestore**: `tickets/`

### 4.4 FirestoreInvoiceService

**Archivo**: `Cinema.Api/Services/FirestoreInvoiceService.cs`

**Métodos Principales**:
- `AddInvoiceAsync(Invoice)` - Crear factura (genera número automático)
- `GetInvoiceByBookingIdAsync(bookingId)` - Factura de una reserva
- `GenerateInvoiceNumberAsync()` - Genera número secuencial único

**Colecciones Firestore**:
- `invoices/` - Facturas
- `counters/invoice_counter` - Contador para numeración

### 4.5 QRCodeService

**Archivo**: `Cinema.Api/Services/QRCodeService.cs`

**Responsabilidades**:
- Generar imágenes QR en Base64
- Codificar/decodificar datos de boletos
- Validar formato de QR

**Ejemplo de Uso**:
```csharp
var qrData = _qrCodeService.EncodeTicketData(ticket);
// Resultado: "TICKET:id=ABC|user=USER123|..."

var qrImage = _qrCodeService.GenerateQrCodeImage(qrData);
// Resultado: "iVBORw0KGgoAAAANS..." (Base64)
```

### 4.6 PaymentSimulationService

**Archivo**: `Cinema.Api/Services/PaymentSimulationService.cs`

**Responsabilidades**:
- Simular procesamiento de pagos
- Validar tarjetas (Algoritmo de Luhn)
- 90% aprobados, 10% rechazados (aleatorio)

**Validaciones Implementadas**:
- ✅ Algoritmo de Luhn para número de tarjeta
- ✅ Validación de fecha de expiración
- ✅ Validación de CVV (3-4 dígitos)
- ✅ Detección automática de marca (Visa, MasterCard, etc.)

**Tarjetas de Prueba**:
```
Visa:       4242424242424242
MasterCard: 5555555555554444
Amex:       378282246310005
```

### 4.7 EmailService

**Archivo**: `Cinema.Api/Services/EmailService.cs`

**Responsabilidades**:
- Enviar emails con SendGrid
- Templates HTML profesionales
- Modo simulado con logs (si no hay API key)

**Emails Implementados**:
1. **Confirmación de Reserva** - Detalles de la compra
2. **Boletos Digitales** - Con códigos QR embebidos
3. **Factura** - Con tabla de items y totales

**Modo Simulado**: Si no se configura SendGrid API Key, los emails se muestran en los logs.

### 4.8 TicketService

**Archivo**: `Cinema.Api/Services/TicketService.cs`

**Responsabilidades**:
- Generar boletos con QR para una reserva
- Validar boletos en la entrada
- Obtener boletos activos del usuario

**Método Clave**: `ValidateAndUseTicketAsync(qrCodeData)`

Validaciones:
- ✅ Formato de QR correcto
- ✅ Boleto existe en la base de datos
- ✅ No fue usado previamente
- ✅ No está expirado

### 4.9 InvoiceService

**Archivo**: `Cinema.Api/Services/InvoiceService.cs`

**Responsabilidades**:
- Generar facturas automáticas
- Crear PDFs con QuestPDF
- Numeración secuencial de facturas

**Características del PDF**:
- ✅ Header con logo/nombre del cine
- ✅ Información del cliente
- ✅ Tabla de items
- ✅ Cálculo de impuestos
- ✅ Total destacado
- ✅ Número de página

---

## 5. FLUJO DE COMPRA COMPLETO

### Paso a Paso

```
┌─────────────────────────────────────────┐
│ 1. USUARIO SELECCIONA FUNCIÓN Y ASIENTOS│
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│ 2. SE CREA BOOKING (status: pending)    │
│    - Calcula totales                    │
│    - Reserva asientos temporalmente     │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│ 3. USUARIO INGRESA DATOS DE PAGO        │
│    - Número de tarjeta                  │
│    - Fecha de expiración                │
│    - CVV                                │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│ 4. PAYMENT SIMULATION SERVICE           │
│    - Valida tarjeta (Algoritmo Luhn)    │
│    - Simula aprobación/rechazo (90/10)  │
│    - Genera TransactionId               │
└──────────────┬──────────────────────────┘
               │
       ┌───────┴───────┐
       │               │
       ▼               ▼
   APROBADO        RECHAZADO
       │               │
       │               └──> Mostrar error al usuario
       │
       ▼
┌─────────────────────────────────────────┐
│ 5. PAYMENT APPROVED                     │
│    - Guardar Payment en Firestore       │
│    - Confirmar Booking (status=confirmed)│
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│ 6. GENERAR BOLETOS                      │
│    - Crear Ticket por cada asiento      │
│    - Generar QR único por boleto        │
│    - Guardar en Firestore (batch)       │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│ 7. GENERAR FACTURA                      │
│    - Crear Invoice con número secuencial│
│    - Agregar items (boletos + comida)   │
│    - Calcular impuestos                 │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│ 8. ENVIAR EMAILS                        │
│    Email 1: Confirmación de reserva     │
│    Email 2: Boletos con QR              │
│    Email 3: Factura PDF                 │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│ 9. USUARIO RECIBE BOLETOS               │
│    - Ve boletos en su cuenta            │
│    - Puede descargar PDF                │
│    - QR listo para escanear             │
└─────────────────────────────────────────┘
```

---

## 6. CONFIGURACIÓN REQUERIDA

### appsettings.Development.json

```json
{
  "SendGrid": {
    "ApiKey": "",
    "FromEmail": "noreply@magiacinema.com",
    "FromName": "Magia Cinema"
  },
  "Payment": {
    "SimulationMode": true,
    "ApprovalRate": 0.9,
    "Currency": "USD"
  },
  "Tickets": {
    "ExpirationMinutes": 30,
    "QrCodeSize": 300
  },
  "Invoice": {
    "NumberPrefix": "INV",
    "TaxRate": 0.13
  }
}
```

### Servicios a Registrar en Program.cs

```csharp
// Firestore Services
builder.Services.AddScoped<FirestoreBookingService>();
builder.Services.AddScoped<FirestorePaymentService>();
builder.Services.AddScoped<FirestoreTicketService>();
builder.Services.AddScoped<FirestoreInvoiceService>();

// Business Services
builder.Services.AddScoped<QRCodeService>();
builder.Services.AddScoped<PaymentSimulationService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<InvoiceService>();
```

---

## 7. APIS Y ENDPOINTS

### 7.1 BookingsController

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/bookings/create` | Crear nueva reserva |
| GET | `/api/bookings/{id}` | Obtener reserva |
| GET | `/api/bookings/user/{userId}` | Reservas del usuario |
| PUT | `/api/bookings/{id}/confirm` | Confirmar reserva |
| DELETE | `/api/bookings/{id}/cancel` | Cancelar reserva |

### 7.2 PaymentsController

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/payments/process` | Procesar pago simulado |
| GET | `/api/payments/{id}` | Obtener pago |
| GET | `/api/payments/booking/{bookingId}` | Pago de una reserva |

### 7.3 TicketsController

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/tickets/{id}` | Obtener boleto |
| GET | `/api/tickets/user/{userId}` | Boletos del usuario |
| GET | `/api/tickets/booking/{bookingId}` | Boletos de una reserva |
| POST | `/api/tickets/validate` | Validar QR (entrada cine) |
| GET | `/api/tickets/{id}/download` | Descargar PDF |

### 7.4 InvoicesController

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/invoices/{id}` | Obtener factura |
| GET | `/api/invoices/booking/{bookingId}` | Factura de una reserva |
| GET | `/api/invoices/user/{userId}` | Facturas del usuario |
| GET | `/api/invoices/{id}/download` | Descargar PDF |

---

## 8. CASOS DE USO

### Caso 1: Compra Exitosa

**Actores**: Usuario, Sistema

**Flujo**:
1. Usuario selecciona película y asientos
2. Sistema crea Booking (pending)
3. Usuario ingresa tarjeta válida
4. Sistema procesa pago → APROBADO
5. Sistema genera boletos con QR
6. Sistema genera factura
7. Sistema envía emails
8. Usuario recibe confirmación

**Resultado**: Boletos disponibles en la cuenta del usuario.

### Caso 2: Pago Rechazado

**Flujo**:
1-3. (igual que caso 1)
4. Sistema procesa pago → RECHAZADO
5. Sistema muestra mensaje de error
6. Booking permanece en estado "pending"

**Resultado**: Usuario debe intentar con otra tarjeta.

### Caso 3: Validación de Boleto en Entrada

**Actores**: Empleado del cine, Sistema

**Flujo**:
1. Cliente llega al cine
2. Muestra QR en su móvil
3. Empleado escanea QR
4. Sistema valida:
   - Formato correcto ✅
   - Boleto existe ✅
   - No usado ✅
   - No expirado ✅
5. Sistema marca como usado
6. Cliente puede entrar

---

## 9. TESTING Y VALIDACIONES

### Validaciones Implementadas

#### Booking
- ✅ Asientos no duplicados en la misma función
- ✅ Máximo 10 boletos por reserva
- ✅ Función debe ser futura

#### Payment
- ✅ Algoritmo de Luhn para tarjeta
- ✅ CVV 3-4 dígitos
- ✅ Fecha de expiración válida
- ✅ Monto coincide con reserva

#### Ticket
- ✅ QR único por boleto
- ✅ No reutilizable
- ✅ Expira después de la función

---

## 10. TROUBLESHOOTING

### Problema: Emails no se envían

**Causa**: API Key de SendGrid no configurada

**Solución**:
1. Ir a https://sendgrid.com
2. Crear cuenta gratuita
3. Generar API Key
4. Agregar en `appsettings.Development.json`:
   ```json
   "SendGrid": {
     "ApiKey": "TU_API_KEY_AQUI"
   }
   ```

**Mientras tanto**: Los emails se mostrarán en los logs.

### Problema: QR no se genera

**Causa**: Paquete QRCoder no instalado

**Solución**:
```bash
dotnet add package QRCoder --version 1.6.0
```

### Problema: Número de factura duplicado

**Causa**: Múltiples servidores sin sincronización

**Solución**: Firestore maneja transacciones. Asegurar que solo una instancia corra en desarrollo.

---

## ✅ ESTADO DE IMPLEMENTACIÓN

| Componente | Estado | Archivo |
|-----------|--------|---------|
| Booking Entity | ✅ Completo | `Cinema.Domain/Entities/Booking.cs` |
| Payment Entity | ✅ Completo | `Cinema.Domain/Entities/Payment.cs` |
| Ticket Entity | ✅ Completo | `Cinema.Domain/Entities/Ticket.cs` |
| Invoice Entity | ✅ Completo | `Cinema.Domain/Entities/Invoice.cs` |
| FirestoreBookingService | ✅ Completo | `Cinema.Api/Services/FirestoreBookingService.cs` |
| FirestorePaymentService | ✅ Completo | `Cinema.Api/Services/FirestorePaymentService.cs` |
| FirestoreTicketService | ✅ Completo | `Cinema.Api/Services/FirestoreTicketService.cs` |
| FirestoreInvoiceService | ✅ Completo | `Cinema.Api/Services/FirestoreInvoiceService.cs` |
| QRCodeService | ✅ Completo | `Cinema.Api/Services/QRCodeService.cs` |
| PaymentSimulationService | ✅ Completo | `Cinema.Api/Services/PaymentSimulationService.cs` |
| EmailService | ✅ Completo | `Cinema.Api/Services/EmailService.cs` |
| TicketService | ✅ Completo | `Cinema.Api/Services/TicketService.cs` |
| InvoiceService | ✅ Completo | `Cinema.Api/Services/InvoiceService.cs` |
| BookingsController | ⏳ Pendiente | - |
| PaymentsController | ⏳ Pendiente | - |
| TicketsController | ⏳ Pendiente | - |
| InvoicesController | ⏳ Pendiente | - |
| Program.cs Registration | ⏳ Pendiente | - |
| appsettings Config | ⏳ Pendiente | - |

---

**Próximos Pasos**: Implementar los 4 controladores REST y actualizar la configuración en Program.cs y appsettings.json.
