# 🎬 Proyecto 4 - Backend (ASP.NET Core Clean Architecture)

Este es el backend del sistema **Cinema**, desarrollado con **ASP.NET Core 9.0** siguiendo el patrón **Clean Architecture**.  
Incluye autenticación (JWT/Firebase), logging con Serilog y está preparado para conectarse con el frontend en Flutter.

---

## 🚀 Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)  
- Visual Studio 2022 / VS Code  
- Git  

---

## 🛠 Configuración inicial

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/Parini19/Proyecto-4-Backend.git
   cd Proyecto-4-Backend
   ```

2. Restaurar dependencias:

   ```bash
   dotnet restore
   ```

3. Compilar el proyecto:

   ```bash
   dotnet build
   ```

---

## ▶ Ejecutar el proyecto

Ejecuta el API:

```bash
dotnet run --project src/Cinema.Api
```

Esto levantará el servidor en:

```
https://localhost:7238
http://localhost:5184
```

Puedes probar la API en:

- **Swagger**: [https://localhost:7238/swagger](https://localhost:7238/swagger)  
- **Health Check**: [https://localhost:7238/health](https://localhost:7238/health)  

---

## 📦 Estructura del proyecto

```
Proyecto-4-Backend/
 ├─ src/
 │   ├─ Cinema.Api/             # Capa de presentación (Web API)
 │   ├─ Cinema.Application/     # Casos de uso / lógica de aplicación
 │   ├─ Cinema.Domain/          # Entidades y reglas de negocio
 │   └─ Cinema.Infrastructure/  # Conexiones externas (BD, Firebase, etc.)
 └─ tests/                      # (Futuro) proyectos de pruebas unitarias
```

---

## 🔗 Integración con Frontend

El frontend espera consumir la API desde:

```
https://localhost:7238
```

Asegúrate de que el backend esté corriendo antes de iniciar el proyecto Flutter.  
En caso de cambiar el puerto, ajusta el valor de `API_BASE_URL` en el frontend.

---


# Proyecto-4-Backend

Justificación de arquitectura

-Monolito con .NET + Services: Usar una arquitectura híbrida. Gran parte del back-end será desarrollado en . Net y usará servicios de Firebase para funciones necesarias. Esto es para tener un balance entre la facilidad de los servicios de firebase y crear un backend, que se pueda adaptar a las necesidades y lógica propias del completo del proyecto

-Clean Architecture: Organiza el código por capas y módulos, facilita pruebas unitarias por medio de frameworks y tecnologías específicas, se puede desarrollar un codebase flexible y uso de librerías y packages para el desarrollo	del funcionalidades específicas del proyecto




# Ejecutar local el CI de manera local


Restaurar dependencias:
-`dotnet restore`


Ejecutar el linter y chequear formato:
-`dotnet format --check`


Construir el proyecto:
-`dotnet build --configuration Release`


Ejecutar pruebas
-`dotnet test --configuration Release`


# Definition of done
-Las siguinetes clauseas son las condiciondes de DoD en el proyecto

-La funcionalidad del código se determina en condición binaria 

-El código está desarrollado, compilado y sin errores de compilación o linting 

-El código ha sido revisado y aprobado en un Pull Request por al menos un 
miembro del equipo. 

-Se han pasado satisfactoriamente todas las pruebas unitarias y de 
integración asociadas. 

-Se han realizado pruebas funcionales mínimas para validar la funcionalidad 
implementada. 

-No existen conflictos con la rama base tras la integración del código. 

-El código ha sido revisado en aspectos de seguridad y rendimiento básicos. 

-Se integra correctamente en el entorno de staging o pruebas sin errores 
críticos. 


# Guía de uso de StyleCop en proyectos .NET
¿Qué es StyleCop?
StyleCop es una herramienta de análisis estático de código que verifica que el código C# cumpla con las convenciones de estilo establecidas. StyleCop.Analyzers es la versión moderna que se integra con el compilador Roslyn y funciona con .NET Core/.NET 5+.
Instalación

INstalacion mediante NuGet Package Manager
Clic derecho en tu proyecto en Visual Studio
Selecciona "Manage NuGet Packages"
Busca StyleCop.Analyzers
Instala el paquete

-1. Crear archivo stylecop.json
Crea un archivo stylecop.json en la raíz con la configuración básica:

-2. Incluir stylecop.json en el proyecto
Añadir la configuracion al archivo .csproj del proyecto