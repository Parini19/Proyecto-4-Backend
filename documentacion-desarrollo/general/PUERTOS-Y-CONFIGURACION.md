# 🔌 Puertos y Configuración del Proyecto

## 📊 Diagrama de Puertos

```
┌─────────────────────────────────────────────────────────────┐
│                    NAVEGADOR DEL USUARIO                    │
│                                                             │
│  ┌──────────────────────┐      ┌──────────────────────┐   │
│  │  Frontend Web        │      │   Backend API         │   │
│  │  http://localhost:   │◄────►│   http://localhost:   │   │
│  │  5173                │      │   5000                │   │
│  └──────────────────────┘      └──────────────────────┘   │
│         (Flutter)                    (.NET API)           │
└─────────────────────────────────────────────────────────────┘
```

---

## 📍 Resumen de Puertos

| Servicio | Puerto | Protocolo | URL Completa | Uso |
|----------|--------|-----------|--------------|-----|
| **Frontend** | 5173 | HTTP | http://localhost:5173 | Interfaz web del usuario |
| **Backend HTTP** | 5000 | HTTP | http://localhost:5000 | API REST (desarrollo) |
| **Backend HTTPS** | 7238 | HTTPS | https://localhost:7238 | API REST (producción) |
| **Swagger UI** | 5000 | HTTP | http://localhost:5000/swagger | Documentación API |

---

## 🔗 Conexiones

### Frontend → Backend

El frontend hace llamadas al backend en:

```dart
// lib/core/config.dart
static const apiBaseUrl = 'http://localhost:5000';
```

**Ejemplos de llamadas:**
- `http://localhost:5000/api/movies` ← Listar películas
- `http://localhost:5000/api/FirebaseTest/login` ← Login
- `http://localhost:5000/health` ← Health check

---

### Backend → Frontend (CORS)

El backend permite requests SOLO desde estos orígenes:

```json
// src/Cinema.Api/appsettings.Development.json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",   // ← Frontend HTTP
      "https://localhost:5173"   // ← Frontend HTTPS (futuro)
    ]
  }
}
```

**¿Qué significa esto?**
- Solo el frontend en `http://localhost:5173` puede hacer requests al backend
- Cualquier otro origen será bloqueado por CORS

---

## ⚙️ Configuración por Archivo

### 1. Frontend: `lib/core/config.dart`

```dart
class AppConfig {
  static const apiBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://localhost:5000',  // ← Puerto del BACKEND
  );
}
```

**¿Qué configurar aquí?**
- La URL donde está el BACKEND
- En desarrollo: `http://localhost:5000`
- En producción: URL de tu servidor (ej: `https://api.cinema.com`)

---

### 2. Backend: `appsettings.Development.json`

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",   // ← Puerto del FRONTEND
      "https://localhost:5173"
    ]
  }
}
```

**¿Qué configurar aquí?**
- Las URLs desde donde el FRONTEND puede hacer requests
- En desarrollo: `http://localhost:5173`
- En producción: URL de tu frontend (ej: `https://cinema.com`)

---

### 3. Backend: `Properties/launchSettings.json`

```json
{
  "profiles": {
    "Cinema.Api": {
      "applicationUrl": "https://localhost:7238;http://localhost:5000"
      //                       ↑                      ↑
      //                    HTTPS (prod)          HTTP (dev)
    }
  }
}
```

**¿Qué configurar aquí?**
- Los puertos donde el BACKEND escucha
- Normalmente NO necesitas cambiar esto

---

## 🚨 Errores Comunes

### ❌ Error: "CORS policy blocked"

**Causa:** El puerto del frontend no está en la lista de CORS del backend.

**Solución:** Agrega el puerto del frontend a `appsettings.Development.json`:

```json
"AllowedOrigins": [
  "http://localhost:5173"  // ← Tu puerto del frontend
]
```

---

### ❌ Error: "Failed to fetch" o "net::ERR_CONNECTION_REFUSED"

**Causa:** El backend no está corriendo o está en un puerto diferente.

**Solución:**
1. Verifica que el backend esté corriendo: http://localhost:5000/health
2. Si no responde, inicia el backend con `start-backend.bat`
3. Verifica que el puerto en `config.dart` sea el correcto

---

### ❌ Error: "Certificate not trusted" (HTTPS)

**Causa:** Intentas usar HTTPS sin certificado de desarrollo válido.

**Solución en desarrollo:**
- Usa HTTP (puerto 5000) en lugar de HTTPS (puerto 7238)
- Ya está configurado por defecto en `config.dart`

**Solución para HTTPS:**
```bash
dotnet dev-certs https --trust
```

---

## 📝 Checklist de Configuración

### Para nuevos desarrolladores:

- [ ] Backend corre en puerto 5000 (HTTP)
- [ ] Frontend corre en puerto 5173
- [ ] `config.dart` apunta a `http://localhost:5000`
- [ ] `appsettings.Development.json` permite `http://localhost:5173`
- [ ] Probaste abrir http://localhost:5000/swagger (debe cargar)
- [ ] Probaste abrir http://localhost:5173 (debe cargar el frontend)

---

## 🔄 ¿Necesitas cambiar puertos?

### Cambiar puerto del Frontend:

1. **Modificar comando de inicio:**
   ```bash
   flutter run -d chrome --web-port 8080  # Nuevo puerto
   ```

2. **Actualizar CORS en backend:**
   ```json
   "AllowedOrigins": [
     "http://localhost:8080"  // Nuevo puerto
   ]
   ```

---

### Cambiar puerto del Backend:

1. **Modificar `launchSettings.json`:**
   ```json
   "applicationUrl": "http://localhost:9000"  // Nuevo puerto
   ```

2. **Actualizar frontend:**
   ```dart
   defaultValue: 'http://localhost:9000'  // Nuevo puerto
   ```

---

## 🎯 Resumen Visual

```
FRONTEND (5173)  ──request──>  BACKEND (5000)

                   ◄───OK──── (si 5173 está en CORS)

FRONTEND (8080)  ──request──>  BACKEND (5000)

                   ◄───ERROR── (si 8080 NO está en CORS)
```

**Regla simple:**
- El puerto del FRONTEND debe estar en la lista CORS del BACKEND
- El puerto del BACKEND debe estar en la config del FRONTEND

---

**¿Dudas?** Lee el archivo `README-DEVELOPERS.md`
