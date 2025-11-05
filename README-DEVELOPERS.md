# 🎬 Cinema Project - Guía para Desarrolladores

## 🚀 Inicio Rápido (Quick Start)

### Primera vez trabajando en el proyecto:

1. **Ejecuta el setup inicial:**
   ```bash
   SETUP.bat
   ```
   Este script verifica que tengas todo instalado y configura las dependencias.

2. **Inicia el proyecto completo:**
   ```bash
   START-ALL.bat
   ```
   Esto iniciará automáticamente el backend y frontend en ventanas separadas.

3. **Abre tu navegador:**
   - Frontend: http://localhost:5173
   - Backend API: http://localhost:5000/swagger

¡Listo! Ya puedes desarrollar.

---

## 📋 Pre-requisitos (Solo Primera Vez)

### Software Requerido:

1. **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)**
   - Verificar: `dotnet --version`

2. **[Flutter SDK](https://flutter.dev/docs/get-started/install/windows)**
   - Verificar: `flutter --version`
   - Debe ser versión 3.35.4 o superior

3. **[Google Chrome](https://www.google.com/chrome/)** (para Flutter Web)

4. **Modo Desarrollador de Windows** (requerido por Flutter)
   - Ve a: `Configuración > Privacidad y seguridad > Para desarrolladores`
   - Activa: "Modo de desarrollador"
   - O ejecuta: `start ms-settings:developers`

---

## 🎯 Comandos Disponibles

### Opción 1: Iniciar Todo (Recomendado)

```bash
START-ALL.bat
```
- Inicia Backend + Frontend en ventanas separadas
- Backend en http://localhost:5000
- Frontend en http://localhost:5173

### Opción 2: Iniciar por Separado

**Backend:**
```bash
start-backend.bat
```

**Frontend:**
```bash
cd "Cinema Frontend\Proyecto-4-Frontend"
start-frontend.bat
```

### Opción 3: Comandos Manuales (Avanzado)

**Backend:**
```bash
cd src\Cinema.Api
dotnet run
```

**Frontend:**
```bash
cd "Cinema Frontend\Proyecto-4-Frontend"
flutter run -d chrome --web-port 5173
```

---

## 🔧 Desarrollo

### Hot Reload (Frontend)

Mientras el frontend está corriendo:
- Presiona `r` para hot reload (recarga cambios rápidamente)
- Presiona `R` para hot restart (reinicio completo)
- Presiona `q` para salir

### Cambios en el Backend

Si modificas código del backend:
1. Presiona `Ctrl+C` para detener el servidor
2. Ejecuta `dotnet run` nuevamente

O simplemente guarda el archivo y el servidor se reiniciará automáticamente (si tienes watch habilitado).

---

## 📁 Estructura del Proyecto

```
Cinema/
├── src/
│   ├── Cinema.Api/          # Backend API (.NET)
│   ├── Cinema.Application/  # Lógica de negocio
│   ├── Cinema.Domain/       # Entidades de dominio
│   └── Cinema.Infrastructure/ # Repositorios y servicios
│
├── Cinema Frontend/
│   └── Proyecto-4-Frontend/ # Frontend (Flutter)
│
├── start-backend.bat        # Script para iniciar backend
├── START-ALL.bat            # Script para iniciar todo
└── SETUP.bat                # Script de configuración inicial
```

---

## ⚙️ Configuración

### Backend (appsettings.Development.json)

```json
{
  "Firebase": {
    "Enabled": false  // false = usa datos en memoria
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173"  // Puerto del frontend
    ]
  },
  "Jwt": {
    "Key": "TuClaveSecretaAqui",
    "ExpiresMinutes": 60
  }
}
```

### Frontend (lib/core/config.dart)

```dart
static const apiBaseUrl = String.fromEnvironment(
  'API_BASE_URL',
  defaultValue: 'http://localhost:5000',  // URL del backend (HTTP en desarrollo)
);
```

**Nota:** En desarrollo usamos HTTP (puerto 5000) para evitar problemas de certificados SSL.

---

## 🐛 Troubleshooting

### Error: "Developer Mode not enabled"
**Solución:** Habilita el Modo Desarrollador en Windows
- `Configuración > Para desarrolladores > Modo de desarrollador`

### Error: "CORS policy blocked"
**Solución:** Verifica que el puerto del frontend esté en `appsettings.Development.json`
```json
"AllowedOrigins": ["http://localhost:5173"]
```

### Error: "dotnet command not found"
**Solución:** Instala .NET 9.0 SDK desde https://dotnet.microsoft.com/download

### Error: "flutter command not found"
**Solución:**
1. Instala Flutter desde https://flutter.dev
2. Agrega Flutter al PATH del sistema

### Backend no inicia en puerto 5000
**Solución:** Verifica que el puerto no esté ocupado
```bash
netstat -ano | findstr :5000
```

### Frontend no encuentra Chrome
**Solución:** Instala Google Chrome o usa:
```bash
flutter run -d edge --web-port 5173
```

---

## 🧪 Testing

### Ejecutar tests del Backend

```bash
cd src\Cinema.Api
dotnet test
```

### Ejecutar tests del Frontend

```bash
cd "Cinema Frontend\Proyecto-4-Frontend"
flutter test
```

---

## 📝 Notas Importantes

### Firebase

- Por defecto, Firebase está **deshabilitado** (`"Enabled": false`)
- El sistema usa repositorios **InMemory** (datos en memoria)
- Los datos se pierden al reiniciar el servidor
- Para usar Firebase real, contacta al líder del proyecto

### Credenciales

- **NUNCA** commitees archivos con credenciales reales
- Usa el template: `appsettings.Development.json.example`
- Las credenciales locales están en `.gitignore`

### Puertos

| Servicio | Puerto | URL |
|----------|--------|-----|
| Backend HTTP | 5000 | http://localhost:5000 |
| Backend HTTPS | 7238 | https://localhost:7238 |
| Frontend Web | 5173 | http://localhost:5173 |
| Swagger UI | 5000 | http://localhost:5000/swagger |

**En desarrollo**: El frontend usa HTTP (puerto 5000) para evitar problemas de certificados.

---

## 🆘 Soporte

Si tienes problemas:

1. Ejecuta `SETUP.bat` nuevamente
2. Verifica que todos los pre-requisitos estén instalados
3. Revisa la sección de Troubleshooting
4. Contacta al equipo de desarrollo

---

## 🚢 Deployment

Para producción, sigue la guía en `docs/DEPLOYMENT.md` (si existe).

---

**¡Happy Coding! 🎉**
