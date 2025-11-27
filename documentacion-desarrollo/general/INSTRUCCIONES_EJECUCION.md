# 🚀 INSTRUCCIONES PARA EJECUTAR EL SISTEMA
## Cinema App - Sistema de Pago y Facturación

---

## ✅ PASO 1: EJECUTAR EL BACKEND (.NET API)

### Opción A: Desde la Terminal

```bash
# Navegar a la carpeta del API
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"

# Ejecutar el API
dotnet run
```

### Opción B: Desde Visual Studio
1. Abrir `Cinema.sln` en Visual Studio
2. Establecer `Cinema.Api` como proyecto de inicio
3. Presionar F5 o hacer clic en "Run"

### Verificar que funciona:
- ✅ La terminal debe mostrar: `Now listening on: https://localhost:7238`
- ✅ Abrir navegador en: https://localhost:7238/swagger
- ✅ Deberías ver la interfaz de Swagger UI

**IMPORTANTE**: Mantén esta terminal abierta mientras pruebas el frontend.

---

## ✅ PASO 2: CONFIGURAR CHROME PARA LOCALHOST

Para que Chrome acepte el certificado SSL auto-firmado:

1. **Abrir Chrome**
2. **Ir a**: `chrome://flags/#allow-insecure-localhost`
3. **Habilitar**: "Allow invalid certificates for resources loaded from localhost"
4. **Reiniciar Chrome**

---

## ✅ PASO 3: EJECUTAR EL FRONTEND (FLUTTER)

### Abrir NUEVA terminal (dejar el backend corriendo)

```bash
# Navegar a la carpeta del frontend
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"

# Ejecutar en Chrome
flutter run -d chrome --web-port=5173
```

### Verificar que funciona:
- ✅ La terminal debe mostrar: `Running on: http://localhost:5173`
- ✅ Chrome debería abrirse automáticamente
- ✅ Deberías ver la aplicación de Cinema

---

## 🧪 PASO 4: PROBAR EL FLUJO COMPLETO

### 1. Login
- Usuario de prueba (crear uno nuevo si es necesario)

### 2. Seleccionar Película
- Ir a la página de películas
- Seleccionar una película
- Elegir una función

### 3. Seleccionar Asientos
- Seleccionar entre 1-8 asientos
- Click en "Continuar"

### 4. Menú de Alimentos (Opcional)
- Agregar combos o alimentos
- O hacer click en "Omitir alimentos"

### 5. Resumen de Compra
- ✅ **AQUÍ SE CREA EL BOOKING EN EL API**
- Verifica el total
- Click en "Continuar al Pago"

### 6. Pago
- **Tarjetas de prueba válidas**:
  - Visa: `4111111111111111`
  - MasterCard: `5500000000000004`
  - Amex: `378282246310005`
- CVV: Cualquier 3 dígitos (ej: `123`)
- Fecha: Cualquier fecha futura (ej: `12/25`)
- Nombre: Tu nombre

- ✅ **AQUÍ SE PROCESA EL PAGO Y SE GENERAN TICKETS**
- Click en "Pagar"

### 7. Confirmación
- Deberías ver:
  - ✅ Código de Reserva
  - ✅ Número de Factura (INV-2025-XXXX)
  - ✅ Cantidad de Boletos Generados
  - ✅ Resumen de la compra

- Click en "Ver Mis Tickets"

### 8. Mis Tickets
- ✅ Deberías ver todos tus boletos
- ✅ Cada boleto tiene su propio QR code
- ✅ Estados: Activo, Usado, o Expirado
- ✅ Puedes hacer click para ver el QR en grande
- ✅ Puedes descargar el PDF del boleto

---

## 📝 VERIFICAR EN EL BACKEND

Mientras pruebas, verifica en la consola del backend:

### Logs que deberías ver:

```
[INF] Creating booking for screening...
[INF] Booking created successfully: xxx
[INF] Processing payment for booking xxx
[INF] Payment xxx approved for booking xxx
[INF] Emails sent for booking xxx
```

### Ver Emails Simulados
Los emails se loguean en la consola del backend (modo desarrollo):

1. **Email de Confirmación de Reserva**
2. **Email con Boletos (QR codes)**
3. **Email con Factura**

---

## 🔍 VERIFICAR EN FIRESTORE

Puedes verificar que los datos se guardaron en Firebase:

1. Ir a: https://console.firebase.google.com/
2. Seleccionar proyecto: `magiacinema-c5b10`
3. Ir a Firestore Database
4. Deberías ver las colecciones:
   - ✅ `bookings/` - Tu reserva
   - ✅ `payments/` - Tu pago
   - ✅ `tickets/` - Tus boletos (uno por asiento)
   - ✅ `invoices/` - Tu factura
   - ✅ `counters/invoice_counter` - Contador de facturas

---

## 🐛 SOLUCIÓN DE PROBLEMAS

### Error: "NET::ERR_CERT_AUTHORITY_INVALID"
**Solución**: Verificar que habilitaste `chrome://flags/#allow-insecure-localhost`

### Error: "CORS policy blocked"
**Solución**: El backend ya tiene CORS configurado para puerto 5173. Verifica que estés usando ese puerto.

### Error: "Failed to create booking"
**Causas posibles**:
- No estás logueado → Hacer login primero
- Backend no está corriendo → Verificar en https://localhost:7238/swagger
- Firestore no configurado → Verificar archivo `magiacinema-adminsdk.json`

### Error: "Network error" en pagos
**Causas posibles**:
- Backend no responde → Reiniciar el API
- Booking no existe → Asegúrate de llegar desde el flujo completo
- HTTPS bloqueado → Verificar flags de Chrome

### Frontend no compila
**Solución**:
```bash
# Limpiar y reconstruir
flutter clean
flutter pub get
flutter run -d chrome --web-port=5173
```

---

## 📊 ENDPOINTS PARA PROBAR MANUALMENTE

Si quieres probar los endpoints directamente en Swagger:

### 1. Crear Booking
```
POST /api/bookings/create
Body:
{
  "userId": "tu-user-id",
  "screeningId": "ST1",
  "seatNumbers": ["A1", "A2"],
  "ticketPrice": 150.0,
  "subtotalFood": 0.0
}
```

### 2. Procesar Pago
```
POST /api/payments/process
Body:
{
  "bookingId": "id-del-booking-creado",
  "amount": 339.0,
  "cardNumber": "4111111111111111",
  "cardHolderName": "Test User",
  "expiryMonth": "12",
  "expiryYear": "25",
  "cvv": "123"
}
```

### 3. Ver Tickets
```
GET /api/tickets/user/{tu-user-id}
```

### 4. Ver Facturas
```
GET /api/invoices/user/{tu-user-id}
```

---

## 🎯 CHECKLIST DE PRUEBA

### Pre-requisitos
- [ ] Backend corriendo en https://localhost:7238
- [ ] Frontend corriendo en http://localhost:5173
- [ ] Chrome configurado para certificados localhost
- [ ] Usuario logueado en la app

### Flujo de Compra
- [ ] Seleccionar película ✓
- [ ] Seleccionar función ✓
- [ ] Seleccionar asientos (1-8) ✓
- [ ] (Opcional) Agregar alimentos ✓
- [ ] Ver resumen de compra ✓
- [ ] Crear booking (automático) ✓
- [ ] Ingresar datos de tarjeta ✓
- [ ] Procesar pago ✓
- [ ] Ver confirmación ✓
- [ ] Ver tickets generados ✓

### Verificaciones Backend
- [ ] Booking creado en Firestore ✓
- [ ] Payment registrado ✓
- [ ] Tickets generados (uno por asiento) ✓
- [ ] Invoice generada con numeración ✓
- [ ] 3 emails logueados en consola ✓

### Verificaciones Frontend
- [ ] Página de tickets muestra boletos ✓
- [ ] QR codes visibles ✓
- [ ] Estados correctos (Activo/Usado/Expirado) ✓
- [ ] Descarga de PDF funciona ✓
- [ ] Refresh actualiza la lista ✓

---

## 📸 CAPTURAS RECOMENDADAS (Para Documentación)

1. **Swagger UI** del backend
2. **Selección de asientos** en el frontend
3. **Página de pago** con tarjeta
4. **Confirmación** con datos de booking
5. **Página de tickets** con QR codes
6. **Consola del backend** con logs de emails
7. **Firestore** con las colecciones creadas

---

## 🎓 DATOS PARA DEMOSTRACIÓN

### Tarjetas de Prueba
- **Visa**: 4111111111111111
- **MasterCard**: 5500000000000004
- **Amex**: 378282246310005
- **Discover**: 6011111111111117

### Escenarios de Prueba

**Escenario 1: Compra Simple**
- 2 asientos regulares
- Sin alimentos
- Pago con Visa

**Escenario 2: Compra Completa**
- 4 asientos (2 regulares, 2 VIP)
- 2 combos de alimentos
- Pago con MasterCard

**Escenario 3: Múltiples Compras**
- Hacer 3 compras diferentes
- Verificar numeración de facturas (INV-2025-0001, 0002, 0003)
- Ver todos los tickets en "Mis Tickets"

---

## 💡 TIPS

1. **Mantén ambas consolas abiertas** para ver logs en tiempo real
2. **Usa DevTools de Chrome** (F12) para ver peticiones de red
3. **Verifica Firestore** después de cada compra
4. **Los emails se loguean** en la consola del backend
5. **Los tickets expiran** 30 minutos después de la función

---

## 🆘 CONTACTO Y SOPORTE

Si encuentras algún error:
1. Verifica que ambos servicios estén corriendo
2. Revisa los logs en ambas consolas
3. Verifica la configuración de Chrome
4. Revisa la documentación completa en:
   - `PAYMENT_BILLING_SYSTEM_DOCS.md`
   - `FRONTEND_INTEGRATION_STATUS.md`
   - `RESUMEN_IMPLEMENTACIONES.md`

---

**¡Listo para Probar! 🚀**

1. Backend: `cd src/Cinema.Api && dotnet run`
2. Frontend: `cd "Cinema Frontend/Proyecto-4-Frontend" && flutter run -d chrome --web-port=5173`
3. Chrome: Configurar flags
4. ¡A PROBAR! 🎬
