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
