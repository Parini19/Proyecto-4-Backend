# 🚀 Instrucciones para Correr el Proyecto Cinema

## ✅ Estado Actual
- ✅ Backend compilado y funcionando
- ✅ Frontend compilado y funcionando
- ✅ Errores de compilación arreglados

---

## 🔧 1. BACKEND (.NET API)

### Opción 1: Desde PowerShell/CMD

```bash
# Navegar al directorio del API
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"

# Correr en modo HTTPS (recomendado)
dotnet run --launch-profile https

# O correr en modo HTTP
dotnet run
```

**URLs del Backend:**
- ✅ HTTPS: https://localhost:7238
- ✅ HTTP: http://localhost:5000
- ✅ Swagger: https://localhost:7238/swagger
- ✅ Health Check: https://localhost:7238/health

### Opción 2: Desde Visual Studio
1. Abrir `Cinema.sln` en Visual Studio
2. Seleccionar `Cinema.Api` como proyecto de inicio
3. Presionar `F5` o click en "Run"

---

## 🌐 2. FRONTEND WEB (Flutter en Chrome)

### Comando para correr

```bash
# Terminal 1 - Navegar al directorio
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"

# Verificar Flutter instalado
flutter doctor

# Correr en Chrome con puerto 5200
flutter run -d chrome --web-port 5200
```

**URL del Frontend Web:**
- ✅ http://localhost:5200

### Comandos útiles durante desarrollo
Mientras la app está corriendo, en la terminal puedes presionar:
- **`r`** - Hot Reload (recarga cambios rápidamente)
- **`R`** - Hot Restart (reinicia la app completa)
- **`q`** - Salir (cierra la aplicación)
- **`h`** - Ver todos los comandos disponibles

---

## 📱 3. FRONTEND ANDROID (Emulador)

### Paso 1: Configurar la IP del Backend

**3.1 Encuentra tu IP local:**

```bash
ipconfig
```

Busca tu **IPv4 Address** (ejemplo: `192.168.1.100`)

**3.2 Edita el archivo de configuración:**

- **Archivo:** `lib/core/config/api_config.dart`
- **Ubicación:** `C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend\lib\core\config\api_config.dart`
- **Línea 6:** Cambia `'192.168.1.100'` por **TU IP**

```dart
static const String _localIp = '192.168.1.123'; // ⚠️ PON TU IP AQUÍ
```

### Paso 2: Correr en Android

```bash
# Terminal 2 - Nueva ventana de PowerShell
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"

# Ver dispositivos disponibles
flutter devices

# Correr en el emulador (debería detectarse automáticamente)
flutter run -d emulator-5554

# O dejar que Flutter elija automáticamente
flutter run
```

**Dispositivos disponibles:**
- ✅ `emulator-5554` - Android 16 (API 36)
- ✅ `chrome` - Google Chrome (Web)
- ✅ `edge` - Microsoft Edge (Web)
- ✅ `windows` - Windows Desktop

---

## 🔥 4. CORRER WEB Y ANDROID AL MISMO TIEMPO

Puedes correr en **múltiples plataformas simultáneamente** usando terminales separadas:

### Terminal 1 (Web):
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d chrome --web-port 5200
```

### Terminal 2 (Android):
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d emulator-5554
```

---

## 📊 RESUMEN VISUAL

```
┌─────────────────────────────────────────────────────────┐
│  BACKEND (.NET API)                                     │
│  http://localhost:5000                                  │
│  https://localhost:7238                                 │
└─────────────────────────────────────────────────────────┘
                    ▲              ▲
                    │              │
        ┌───────────┘              └──────────────┐
        │                                         │
┌───────────────────┐                  ┌─────────────────────┐
│  WEB (Chrome)     │                  │  ANDROID (Emulator)  │
│  localhost:5200   │                  │  emulator-5554       │
│                   │                  │                      │
│  Usa: localhost   │                  │  Usa: TU_IP          │
└───────────────────┘                  └─────────────────────┘
```

---

## 🎯 WORKFLOW COMPLETO (Paso a Paso)

### 1️⃣ Iniciar Backend
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
dotnet run --launch-profile https
```

Espera ver:
```
Now listening on: https://localhost:7238
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

### 2️⃣ Probar Backend
Abre en el navegador:
- https://localhost:7238/swagger
- https://localhost:7238/health

### 3️⃣ Iniciar Frontend Web
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d chrome --web-port 5200
```

Espera ver:
```
✓ Built build\web\main.dart.js
Launching lib\main.dart on Chrome in debug mode...
Flutter run key commands.
r Hot reload.
```

### 4️⃣ (Opcional) Iniciar Frontend Android
En **otra terminal**:
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d emulator-5554
```

---

## 🐛 TROUBLESHOOTING

### Error: Backend no inicia
```bash
# Verificar .NET instalado
dotnet --version

# Restaurar dependencias
dotnet restore

# Limpiar y reconstruir
dotnet clean
dotnet build
```

### Error: Flutter no encuentra dispositivos
```bash
# Verificar instalación
flutter doctor -v

# Si Android no aparece
flutter doctor --android-licenses
```

### Error: Android no puede conectarse al Backend
1. ✅ Verifica que el Backend esté corriendo
2. ✅ Usa la IP de tu PC (no `localhost`)
3. ✅ Verifica que el firewall permita el puerto 5000
4. ✅ Asegúrate de que emulador esté en la misma red

### Error: "Port 5200 already in use"
```bash
# Usa otro puerto
flutter run -d chrome --web-port 5201

# O encuentra y mata el proceso en el puerto
netstat -ano | findstr :5200
taskkill /PID <process_id> /F
```

### Hot Reload no funciona
En la terminal de Flutter, presiona:
- **`R`** (mayúscula) - Hot Restart completo

---

## 📝 ARCHIVOS MODIFICADOS EN ESTA SESIÓN

### ✅ Archivos Arreglados:
1. ✅ `lib/core/services/user_service.dart` - Agregado método `register()`
2. ✅ `lib/core/widgets/cinema_text_field.dart` - Agregados parámetros `onTap`, `textCapitalization`, `inputFormatters`
3. ✅ `lib/features/auth/register_page.dart` - Arreglado llamado a `register()`
4. ✅ `lib/features/users/users_page.dart` - Reconstruido (estaba corrupto)
5. ✅ `lib/core/config/api_config.dart` - **NUEVO** - Configuración automática de URLs

---

## ⚡ QUICK START (Comandos Rápidos)

### Para Web (Más Fácil):
```bash
# 1. Backend (Terminal 1)
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
dotnet run

# 2. Frontend Web (Terminal 2)
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d chrome --web-port 5200
```

### Para Android:
```bash
# 1. Encuentra tu IP
ipconfig

# 2. Edita lib/core/config/api_config.dart con tu IP

# 3. Backend (Terminal 1)
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
dotnet run

# 4. Frontend Android (Terminal 2)
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d emulator-5554
```

---

## 📌 NOTAS IMPORTANTES

### Para Web:
- ✅ `localhost` funciona perfectamente
- ✅ CORS ya está configurado en el backend
- ✅ Puerto 5200 recomendado
- ✅ Hot Reload funciona perfectamente

### Para Android:
- ⚠️ **NO uses `localhost`** - usa la IP de tu PC
- ⚠️ Configura tu IP en `lib/core/config/api_config.dart`
- ⚠️ Backend debe estar corriendo
- ✅ Primera compilación tomará varios minutos

### Configuración Multi-Plataforma:
El archivo `lib/core/config/api_config.dart` **detecta automáticamente** si estás en Web o Android y usa la URL correcta:
- **Web** → `http://localhost:5000`
- **Android** → `http://TU_IP:5000`

---

## 🎉 ¡LISTO!

Ahora puedes correr el proyecto en:
- ✅ Web (Chrome) - http://localhost:5200
- ✅ Web (Edge) - `flutter run -d edge`
- ✅ Android (Emulador) - `flutter run -d emulator-5554`
- ✅ Windows (Desktop) - `flutter run -d windows`

---

**Generado el:** 4 de Noviembre, 2025
**Estado:** ✅ Proyecto Compilando Sin Errores
