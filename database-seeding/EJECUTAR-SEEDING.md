# 🚀 Instrucciones para Ejecutar el Seeding

## Paso 1: Asegúrate que el API esté corriendo

Si no está corriendo, ábrelo manualmente desde Visual Studio o con:

```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema\src\Cinema.Api"
dotnet run --urls="https://localhost:7238"
```

## Paso 2: Abre PowerShell y ejecuta estos comandos

### 2.1 - Crear Salas de Cine (20 salas)

```powershell
Invoke-WebRequest -Uri "https://localhost:7238/api/theaterrooms/seed?clearExisting=true" `
    -Method POST `
    -SkipCertificateCheck
```

**Resultado esperado:**
```
StatusCode: 200
Mensaje: "Created 20 theater rooms (15 normal + 5 VIP)"
```

### 2.2 - Crear Funciones (Screenings)

```powershell
Invoke-WebRequest -Uri "https://localhost:7238/api/screenings/seed?clearExisting=true" `
    -Method POST `
    -SkipCertificateCheck
```

**Resultado esperado:**
```
StatusCode: 200
Mensaje: "Created ~174 screenings for the next 7 days"
```

## ✅ Verificación

### Ver las salas creadas:
```powershell
Invoke-WebRequest -Uri "https://localhost:7238/api/theaterrooms/get-all-theater-rooms" `
    -Method GET `
    -SkipCertificateCheck
```

### Ver las funciones creadas:
```powershell
Invoke-WebRequest -Uri "https://localhost:7238/api/screenings/get-all-screenings" `
    -Method GET `
    -SkipCertificateCheck
```

---

## 📊 ¿Qué se crea?

### Salas de Cine (20 total)
- **Sala 1** a **Sala 15** → 96 asientos c/u
- **Sala VIP 1** a **Sala VIP 5** → 60 asientos c/u

### Funciones (~174 total)
- Solo para películas **"En Cartelera"** (isNew = true)
- Solo para películas **"Más Populares"** (top 8 por rating)
- **NO** para películas "Próximamente"
- 7 días de programación
- Horarios: 14:00, 17:30, 21:00, 23:30

---

## 🎯 Después del Seeding

Una vez ejecutado el seeding, el sistema queda listo para que el **administrador gestione todo desde el Panel de Admin**:

### El Admin puede:
1. ✅ **Agregar** nuevas salas de cine
2. ✅ **Editar** salas existentes (nombre, capacidad)
3. ✅ **Eliminar** salas que ya no se usen
4. ✅ **Agregar** funciones manualmente (para películas "Próximamente")
5. ✅ **Editar** funciones (cambiar horarios, salas)
6. ✅ **Eliminar** funciones canceladas

### Ubicación del Panel:
- Frontend: `http://localhost:5173/admin`
- Secciones:
  - **Películas** → Gestionar películas
  - **Funciones** → Gestionar screenings
  - **Salas de Cine** → Gestionar theater rooms

---

## ⚠️ Nota Importante

Este seeding es **solo para inicializar** la base de datos. Después de ejecutarlo:

- ❌ **NO necesitas volver a ejecutar** el seeding
- ✅ **Usa el Panel de Admin** para cualquier cambio
- ✅ **El admin gestiona** todo desde la interfaz web

El sistema queda en **modo normal de producción** donde todo se maneja desde el admin panel.
