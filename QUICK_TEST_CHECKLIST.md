# ⚡ Checklist Rápido de Pruebas - Cinema

## 🚀 Inicio Rápido (5 minutos)

### 1. Arrancar Backend
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
dotnet run
```
✅ Debe mostrar: `Now listening on: http://localhost:5000`

### 2. Arrancar Frontend
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d chrome --web-port 5210
```
✅ Debe abrir Chrome con la app

---

## 🔍 Pruebas Básicas (10 minutos)

### Backend - API Tests

**Abre en navegador:**

1. ✅ Health: `http://localhost:5000/health`
   - Debe mostrar: `{"status":"ok",...}`

2. ✅ Movies: `http://localhost:5000/api/movies`
   - Debe mostrar: 3 películas (Inception, Interstellar, Dune)

3. ✅ Screenings: `http://localhost:5000/api/screenings`
   - Debe mostrar: 2 funciones

### Frontend - UI Tests

4. ✅ **Registro:**
   - Click "Regístrate"
   - Nombre: "Test User"
   - Email: "test@test.com"
   - Password: "123456"
   - Click "Crear Cuenta"
   - ✅ Debe crear el usuario

5. ✅ **Login:**
   - Email: "test@test.com"
   - Password: "123456"
   - Click "Iniciar Sesión"
   - ✅ Debe mostrar: "¡Bienvenido Test User!"
   - ✅ Debe redirigir a Home

6. ✅ **Verificar Token:**
   - F12 → Application → Local Storage
   - Buscar: `auth_token`
   - ✅ Debe tener un token JWT (empieza con "eyJ...")

---

## 🔐 Pruebas de Seguridad (5 minutos)

### 7. ✅ Password Incorrecta
- Login con: test@test.com / wrongpassword
- ❌ Debe fallar con "Invalid credentials"

### 8. ✅ Email Inexistente
- Login con: noexiste@test.com / 123456
- ❌ Debe fallar con "User not found"

### 9. ✅ Endpoint Protegido sin Token
**Postman:**
```
POST http://localhost:5000/api/movies
Body: {"title": "Test", "durationMinutes": 120, ...}
```
- ❌ Debe retornar 401 Unauthorized

### 10. ✅ Endpoint Protegido con Token
**Postman:**
```
POST http://localhost:5000/api/movies
Authorization: Bearer {tu_token_aqui}
Body: {"title": "New Movie", "description": "Test", "durationMinutes": 120, "genre": "Action", "director": "Test", "year": 2024}
```
- ✅ Debe retornar 201 Created

---

## 📊 Resultados

| Test | Status | Tiempo |
|------|--------|--------|
| 1. Backend inicia | ⬜ | ___ |
| 2. Frontend inicia | ⬜ | ___ |
| 3. Health check | ⬜ | ___ |
| 4. GET Movies | ⬜ | ___ |
| 5. GET Screenings | ⬜ | ___ |
| 6. Registro usuario | ⬜ | ___ |
| 7. Login exitoso | ⬜ | ___ |
| 8. Token guardado | ⬜ | ___ |
| 9. Password incorrecta | ⬜ | ___ |
| 10. Email inexistente | ⬜ | ___ |
| 11. POST sin token | ⬜ | ___ |
| 12. POST con token | ⬜ | ___ |

**Leyenda:** ✅ Pasó | ❌ Falló | ⬜ No probado

---

## 🆘 Problemas Comunes

### Backend no inicia
```bash
# Verificar puerto 5000 ocupado
netstat -ano | findstr :5000
```

### Frontend no inicia
```bash
flutter clean
flutter pub get
flutter run -d chrome
```

### Error CORS
Verificar `appsettings.json`:
```json
"Cors": {
  "AllowedOrigins": ["http://localhost:5210"]
}
```

### Token no se guarda
- Verificar DevTools Console (F12)
- Verificar Application → Local Storage

---

## 📝 Comandos Útiles

**Ver logs del backend:**
- Los logs aparecen en la terminal donde corriste `dotnet run`

**Reiniciar backend:**
- Ctrl+C en la terminal
- `dotnet run` de nuevo

**Reiniciar frontend:**
- `r` en la terminal de Flutter
- O Ctrl+C y `flutter run` de nuevo

**Hot reload frontend:**
- `r` en la terminal de Flutter (recarga rápida)

---

## ✅ Todo OK? Siguiente Paso

Si todas las pruebas pasan:
- Ver `TESTING_GUIDE.md` para pruebas detalladas
- Probar CRUD completo (Update, Delete)
- Integrar HomePage con API real

---

**Tiempo total estimado:** 20 minutos
**Última actualización:** 04 Nov 2025
