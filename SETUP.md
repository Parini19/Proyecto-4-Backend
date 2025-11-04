# Cinema API - Configuración de Desarrollo

Este documento explica cómo configurar el proyecto para desarrollo local.

## Requisitos Previos

- .NET 9.0 SDK
- Cuenta de Firebase con proyecto configurado
- Visual Studio 2022 o VS Code

## Configuración Inicial

### 1. Configurar Firebase

1. Ve a [Firebase Console](https://console.firebase.google.com/)
2. Crea o selecciona tu proyecto
3. Ve a **Project Settings** → **Service Accounts**
4. Haz clic en **Generate New Private Key**
5. Guarda el archivo JSON descargado en: `src/Cinema.Api/Config/`
6. Renombra el archivo o toma nota de su nombre

### 2. Configurar appsettings.Development.json

1. Copia el archivo de ejemplo:
   ```bash
   cp src/Cinema.Api/appsettings.Development.json.example src/Cinema.Api/appsettings.Development.json
   ```

2. Edita `src/Cinema.Api/appsettings.Development.json` y reemplaza:
   - `YOUR_FIREBASE_ADMINSDK_FILE.json` → Nombre del archivo JSON de Firebase
   - `YOUR_FIREBASE_API_KEY_HERE` → API Key de Firebase (Project Settings → General → Web API Key)
   - `YOUR_FIREBASE_PROJECT_ID` → Project ID de Firebase
   - `CHANGE_THIS_TO_A_SECURE_RANDOM_SECRET_KEY...` → Una clave secreta aleatoria (mínimo 32 caracteres)

### 3. Ejemplo de Configuración

```json
{
  "Firebase": {
    "ConfigPath": "Config/mi-proyecto-firebase-adminsdk.json",
    "apiKey": "AIzaSyABC123...",
    "ProjectId": "mi-proyecto-12345"
  },
  "Jwt": {
    "Key": "mi-super-secreto-key-que-nadie-puede-adivinar-12345",
    "Issuer": "CinemaApi",
    "Audience": "CinemaApiUsers",
    "ExpiresMinutes": 60
  }
}
```

## Ejecutar el Proyecto

```bash
cd src/Cinema.Api
dotnet run
```

La API estará disponible en: http://localhost:5000

## ⚠️ Seguridad

- **NUNCA** commitees `appsettings.Development.json` con credenciales reales
- **NUNCA** commitees archivos de Firebase (`*-adminsdk.json`)
- Estos archivos ya están en `.gitignore` para protegerte

## Soporte

Para problemas de configuración, consulta la documentación de:
- [Firebase Admin SDK](https://firebase.google.com/docs/admin/setup)
- [ASP.NET Core Configuration](https://docs.microsoft.com/aspnet/core/fundamentals/configuration)
