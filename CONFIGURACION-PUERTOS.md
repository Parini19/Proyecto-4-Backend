# Configuración de Puertos Fijos - Proyecto Cinema

## Puertos Configurados (FIJOS)

### Backend API (.NET)
- **HTTP**: `http://localhost:5000` (Puerto Fijo)
- **HTTPS**: `https://localhost:7238` (Puerto Fijo)
- **Swagger UI**: `https://localhost:7238/swagger`
- **Health Check**: `https://localhost:7238/health`

### Frontend Web (Flutter)
- **Web (Chrome/Edge)**: `http://localhost:5173` (Puerto Fijo)
- **DevTools**: Puerto dinámico (asignado automáticamente por Flutter)

### Emulador Android
- **Emulador**: `emulator-5554` (Puerto por defecto de Android Emulator)
- **API URL**: Usa la IP local de tu PC (ej: `http://192.168.1.100:5000`)

---

## Por Qué Usar Puertos Fijos

Similar a Angular que usa el puerto 4200 por defecto, tener puertos fijos:

1. **Evita problemas de CORS**: No necesitas actualizar la configuración cada vez
2. **Consistencia en desarrollo**: Todos los desarrolladores usan los mismos puertos
3. **Configuración predecible**: Scripts y documentación siempre válidos
4. **Facilita debugging**: Siempre sabes dónde está corriendo cada servicio

---

## Configuración CORS en Backend

Archivo: `src/Cinema.Api/appsettings.Development.json`

```json
"Cors": {
  "AllowedOrigins": [
    "https://localhost:5173",
    "http://localhost:5173"
  ]
}
```

**Nota**: Si cambias el puerto del frontend, debes actualizar esta configuración.

---

## Cómo Iniciar el Proyecto

### Opción 1: Scripts Automatizados (Recomendado)

#### Iniciar Todo
```bash
START-ALL.bat
```
Este script:
- Inicia el backend en puerto 5000/7238
- Espera 5 segundos
- Inicia el frontend en puerto 5173

#### Solo Backend
```bash
start-backend.bat
```

#### Solo Frontend
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
start-frontend.bat
```

### Opción 2: Manual

#### Backend
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
dotnet run
```

#### Frontend Web
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d chrome --web-port 5173
```

#### Frontend Android (Emulador)
```bash
# 1. Lanzar emulador
flutter emulators --launch Pixel_8

# 2. Esperar que arranque (30-60 segundos)

# 3. Correr la app
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d emulator-5554
```

---

## Configuración en VS Code

El proyecto ya tiene configuración de launch para VS Code.

Archivo: `.vscode/launch.json` (en el proyecto frontend)

```json
{
  "name": "Flutter Web (Chrome, 5173)",
  "request": "launch",
  "type": "dart",
  "program": "lib/main.dart",
  "args": [
    "-d", "chrome",
    "--web-port=5173",
    "--dart-define=API_BASE_URL=https://localhost:7238"
  ]
}
```

Para usar:
1. Abre VS Code en la carpeta del frontend
2. Presiona `F5` o ve a Run > Start Debugging
3. Selecciona "Flutter Web (Chrome, 5173)"

---

## Verificar Que Todo Está Corriendo

### Backend
```bash
curl http://localhost:5000/health
# O visita: https://localhost:7238/swagger
```

### Frontend
Abre el navegador en: `http://localhost:5173`

### Ver Logs
Los logs del backend aparecen en la consola donde ejecutaste `dotnet run`.
Los logs del frontend aparecen en la consola de Flutter.

---

## Troubleshooting

### Error: "Port 5173 already in use"
```bash
# Windows: Encuentra y mata el proceso
netstat -ano | findstr :5173
taskkill /PID <process_id> /F

# O reinicia VS Code / Terminal
```

### Error: "CORS policy blocked"
1. Verifica que el backend esté corriendo
2. Confirma que `appsettings.Development.json` tenga el puerto 5173 en AllowedOrigins
3. Reinicia el backend si cambiaste la configuración

### Error: Android no puede conectarse
1. **NO uses `localhost`** en Android - usa tu IP local
2. Encuentra tu IP: `ipconfig` (busca IPv4 Address)
3. Actualiza `lib/core/config/api_config.dart` con tu IP
4. Asegúrate que el firewall permita conexiones en el puerto 5000

---

## Resumen Visual

```
┌─────────────────────────────────────────────────────────┐
│  BACKEND (.NET API)                                     │
│  http://localhost:5000         (FIJO)                   │
│  https://localhost:7238        (FIJO)                   │
└─────────────────────────────────────────────────────────┘
                    ▲              ▲
                    │              │
        ┌───────────┘              └──────────────┐
        │                                         │
┌───────────────────┐                  ┌─────────────────────┐
│  WEB (Chrome)     │                  │  ANDROID (Emulator)  │
│  localhost:5173   │                  │  emulator-5554       │
│  (FIJO)           │                  │                      │
│                   │                  │  Usa: 192.168.x.x    │
└───────────────────┘                  └─────────────────────┘
```

---

## Nota Importante

**NO cambies estos puertos** a menos que sea absolutamente necesario. Si necesitas cambiarlos:

1. Actualiza `appsettings.Development.json` (CORS)
2. Actualiza `start-frontend.bat`
3. Actualiza `.vscode/launch.json`
4. Actualiza `START-ALL.bat`
5. Actualiza esta documentación

---

**Última actualización**: 5 de Noviembre, 2025
**Configuración estable**: ✅ Puertos fijos configurados
