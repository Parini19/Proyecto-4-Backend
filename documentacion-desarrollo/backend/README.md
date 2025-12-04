# Documentación del Backend - Cinema API

Documentación específica del backend del proyecto Cinema (.NET 9 API + Firestore).

---

## 📚 Documentos Disponibles

### 🔌 API REST

**[API_COLLECTION.md](./API_COLLECTION.md)**
- Colección completa de endpoints del API
- Ejemplos de requests y responses
- Códigos de estado HTTP
- Estructura de datos
- Colección para importar en Postman/Insomnia

### 💳 Sistema de Pagos y Facturación

**[PAYMENT_BILLING_SYSTEM_DOCS.md](./PAYMENT_BILLING_SYSTEM_DOCS.md)**
- Arquitectura del sistema de pagos
- Entidades: Booking, Payment, Ticket, Invoice
- Servicios Firestore
- Servicios de negocio (QR, Email, PDF)
- Controladores API
- Flujo completo de reserva y pago
- Ejemplos de uso

### 🖼️ Cloudinary (Imágenes)

**[CLOUDINARY_UPLOAD_GUIDE.md](./CLOUDINARY_UPLOAD_GUIDE.md)**
- Configuración de Cloudinary
- Cómo subir imágenes
- Optimización de imágenes
- Transformaciones
- Buenas prácticas

**[get_cloudinary_urls.md](./get_cloudinary_urls.md)**
- Script para obtener URLs de imágenes
- Conversión de paths locales a URLs de Cloudinary
- Útil para migración de datos

---

## 🏗️ Arquitectura del Backend

### Stack Tecnológico
- **.NET 9** - Framework web
- **ASP.NET Core** - API REST
- **Firestore** - Base de datos NoSQL (Google Cloud)
- **Cloudinary** - Almacenamiento de imágenes
- **QRCoder** - Generación de códigos QR
- **iTextSharp** - Generación de PDFs

### Estructura del Proyecto

```
Cinema.Api/
├── Controllers/        ← Endpoints del API
│   ├── MoviesController.cs
│   ├── BookingsController.cs
│   ├── PaymentsController.cs
│   ├── ScreeningsController.cs
│   ├── TheaterRoomsController.cs
│   └── ...
├── Models/            ← Entidades de dominio
│   ├── Movie.cs
│   ├── Booking.cs
│   ├── Payment.cs
│   ├── Ticket.cs
│   ├── Invoice.cs
│   └── ...
├── Services/          ← Lógica de negocio
│   ├── Firestore/    ← Servicios de Firestore
│   ├── QRCodeService.cs
│   ├── EmailService.cs
│   ├── PaymentSimulationService.cs
│   └── ...
└── Program.cs         ← Configuración de la app
```

### Principales Endpoints

#### Películas
- `GET /api/movies` - Obtener todas las películas
- `GET /api/movies/{id}` - Obtener película por ID
- `POST /api/movies` - Crear película

#### Funciones (Screenings)
- `GET /api/screenings` - Obtener funciones
- `GET /api/screenings/movie/{movieId}` - Funciones de una película
- `POST /api/screenings/seed` - Poblar funciones (desarrollo)

#### Reservas y Pagos
- `POST /api/bookings` - Crear reserva
- `POST /api/payments/process` - Procesar pago
- `GET /api/tickets/{bookingId}` - Obtener boletos
- `GET /api/invoices/{bookingId}` - Obtener factura

---

## 🚀 Inicio Rápido

### 1. Configurar el Proyecto
Ver [../general/SETUP.md](../general/SETUP.md)

### 2. Ejecutar el API
```bash
cd src/Cinema.Api
dotnet restore
dotnet run --urls="https://localhost:7238"
```

### 3. Probar Endpoints
Importa la colección del [API_COLLECTION.md](./API_COLLECTION.md) en Postman

### 4. Poblar Base de Datos
Ver [/database-seeding/README.md](../../database-seeding/README.md)

---

## 🔒 Configuración

### Variables de Entorno / appsettings.json

```json
{
  "FirebaseConfig": {
    "ProjectId": "tu-proyecto-id",
    "CredentialsPath": "path/to/serviceAccountKey.json"
  },
  "Cloudinary": {
    "CloudName": "tu-cloud-name",
    "ApiKey": "tu-api-key",
    "ApiSecret": "tu-api-secret"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@cinema.com",
    "SenderPassword": "tu-password"
  }
}
```

---

## 🧪 Testing

### Ejecutar Tests
```bash
cd src/Cinema.Api.Tests
dotnet test
```

### Testing Manual
Ver [../general/TESTING_GUIDE.md](../general/TESTING_GUIDE.md)

---

## 📝 Documentación Adicional

- **Arquitectura**: Ver [/docs/02-BACKEND-ARCHITECTURE.md](../../docs/02-BACKEND-ARCHITECTURE.md)
- **API Documentation**: Ver [/docs/04-API-DOCUMENTATION.md](../../docs/04-API-DOCUMENTATION.md)
- **Frontend Integration**: Ver [../frontend/FRONTEND_INTEGRATION_STATUS.md](../frontend/FRONTEND_INTEGRATION_STATUS.md)

---

**Última actualización**: Noviembre 26, 2025
