# 📸 Guía Completa: Upload de Imágenes con Cloudinary

## ✅ Implementación Completada

### Backend (.NET)
1. ✅ **CloudinaryDotNet** package instalado
2. ✅ **CloudinaryImageService** creado (`src/Cinema.Api/Services/CloudinaryImageService.cs`)
3. ✅ **MoviesController** actualizado con endpoints de upload
4. ✅ **Credenciales configuradas** en `appsettings.Development.json`

### Frontend (Flutter)
1. ✅ **MovieService** creado con métodos CRUD
2. ✅ **image_picker y file_picker** packages instalados
3. ✅ **ImagePickerField** widget creado para seleccionar imágenes
4. ✅ **Movies providers** creados para gestión de estado

---

## 🎯 Cómo Funciona

### Flujo de Upload de Imagen

```
1. Usuario abre diálogo de agregar/editar película
   ↓
2. Hace click en "Seleccionar Imagen"
   ↓
3. Selecciona imagen desde su computadora
   ↓
4. Imagen se convierte a Base64 en el frontend
   ↓
5. Al guardar película, se envía junto con los datos
   ↓
6. Backend recibe Base64 y lo sube a Cloudinary
   ↓
7. Cloudinary devuelve URL pública
   ↓
8. URL se guarda en Firestore junto con la película
```

---

## 🔧 Configuración de Cloudinary

### Credenciales (Ya configuradas en appsettings.Development.json)

```json
{
  "Cloudinary": {
    "CloudName": "dntcviwyy",
    "ApiKey": "889753441957871",
    "ApiSecret": "6uodHCwefoMUwU9aNXmO4Lk7BEw"
  }
}
```

---

## 📝 Endpoints Disponibles

### 1. POST /api/movies/add-movie
Crea una película y sube su poster a Cloudinary

**Request Body**:
```json
{
  "movie": {
    "title": "Dune: Part Two",
    "description": "Paul Atreides...",
    "year": 2024,
    "durationMinutes": 166,
    "genre": "Ciencia Ficción, Aventura",
    "director": "Denis Villeneuve",
    "rating": 8.8,
    "classification": "PG-13",
    "isNew": true,
    "showtimes": ["14:00", "17:30", "21:00"]
  },
  "posterBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRg..." // Opcional
}
```

**Response**:
```json
{
  "success": true,
  "id": "generated-id",
  "posterUrl": "https://res.cloudinary.com/dntcviwyy/image/upload/v123456/movies/Dune_Part_Two_abc123.jpg"
}
```

### 2. PUT /api/movies/edit-movie/{id}
Actualiza una película y su poster

**Request Body**: Igual que add-movie

### 3. POST /api/movies/upload-poster
Sube solo una imagen (sin película)

**Request Body**:
```json
{
  "base64Image": "data:image/jpeg;base64,/9j/4AAQSkZJRg...",
  "fileName": "dune2.jpg" // Opcional
}
```

**Response**:
```json
{
  "success": true,
  "imageUrl": "https://res.cloudinary.com/dntcviwyy/image/upload/v123456/movies/dune2.jpg"
}
```

### 4. DELETE /api/movies/delete-movie/{id}
Elimina película Y su imagen de Cloudinary

---

## 🖼️ Dos Formas de Subir Imágenes

### Opción 1: Upload Manual a Cloudinary (Rápido)

1. **Ir al Dashboard de Cloudinary**:
   ```
   https://console.cloudinary.com/
   ```

2. **Navegar a Media Library**:
   - Click en "Media Library" en el menú izquierdo

3. **Crear carpeta "movies"** (si no existe):
   - Click en "Create Folder"
   - Nombrarla "movies"

4. **Subir imágenes**:
   - Click en "Upload" → "Upload Files"
   - Seleccionar archivos JPG/PNG/WEBP
   - Las imágenes se subirán a: `movies/nombre.jpg`

5. **Copiar URLs**:
   - Click en cada imagen
   - Copiar la URL completa:
   ```
   https://res.cloudinary.com/dntcviwyy/image/upload/v1234567/movies/dune2.jpg
   ```

6. **Usar URLs en el Admin Panel**:
   - Pegar la URL en el campo "URL del Poster"
   - O guardarlas para el seeding

### Opción 2: Upload desde Admin Panel (Automático)

1. **Abrir Admin Panel**:
   ```
   http://localhost:5173/admin
   ```

2. **Ir a "Gestión de Películas"**

3. **Click en "Nueva Película"**

4. **Seleccionar Imagen**:
   - Click en el área de imagen
   - Seleccionar archivo desde tu computadora
   - Ver preview de la imagen

5. **Llenar datos de la película**:
   - Título, género, duración, etc.

6. **Click en "Agregar"**:
   - La imagen se sube automáticamente a Cloudinary
   - La URL se guarda automáticamente
   - ¡Todo listo!

---

## 🎬 Seeding de Películas con Imágenes

### Opción A: Con URLs de Cloudinary (Recomendado)

1. **Subir imágenes manualmente a Cloudinary** (Opción 1 arriba)

2. **Actualizar el MoviesSeederController** con las URLs reales:

```csharp
new Movie
{
    Id = "1",
    Title = "Dune: Part Two",
    PosterUrl = "https://res.cloudinary.com/dntcviwyy/image/upload/v1732586169/movies/dune_2.jpg",
    // ... resto de datos
}
```

3. **Ejecutar el seeder**:
   - POST https://localhost:7238/api/moviesseeder/seed

### Opción B: Con URLs Públicas Temporales

Usar URLs de TMDB, IMDb u otras fuentes públicas:

```csharp
PosterUrl = "https://image.tmdb.org/t/p/original/path.jpg"
```

**Nota**: Para producción, se recomienda subir todas a Cloudinary.

---

## 📋 Integración Frontend-Backend Completa

### 1. Home Page (Usuarios)

El `home_page.dart` ahora usa el API en lugar de datos hardcodeados:

```dart
// ANTES (hardcoded):
final movies = MoviesData.nowPlaying;

// AHORA (desde API):
final moviesAsync = ref.watch(nowPlayingMoviesProvider);
moviesAsync.when(
  data: (movies) => MoviesList(movies),
  loading: () => CircularProgressIndicator(),
  error: (error, stack) => ErrorWidget(error),
);
```

### 2. Admin Panel

El admin puede:
- ✅ Ver todas las películas desde Firestore
- ✅ Agregar películas con upload de imagen
- ✅ Editar películas y cambiar imagen
- ✅ Eliminar películas (y su imagen en Cloudinary)

---

## 🧪 Testing del Sistema Completo

### Paso 1: Ejecutar Backend

```bash
cd src/Cinema.Api
dotnet run
```

Verificar: https://localhost:7238/swagger

### Paso 2: Ejecutar Frontend

```bash
cd "Cinema Frontend/Proyecto-4-Frontend"
flutter run -d chrome --web-port=5173
```

### Paso 3: Configurar Chrome

1. Ir a: `chrome://flags/#allow-insecure-localhost`
2. Habilitar
3. Reiniciar Chrome

### Paso 4: Probar Upload

1. **Ir al Admin Panel**: http://localhost:5173/admin
2. **Login como admin**
3. **Ir a "Gestión de Películas"**
4. **Click en "Nueva Película"**
5. **Seleccionar una imagen**
6. **Llenar datos**
7. **Click en "Agregar"**
8. **Verificar**:
   - Imagen subida a Cloudinary
   - Película guardada en Firestore
   - URL de imagen almacenada correctamente

### Paso 5: Verificar en Cloudinary

1. Ir a: https://console.cloudinary.com/
2. Media Library → movies/
3. Ver imagen subida

### Paso 6: Verificar en Firestore

1. Ir a: https://console.firebase.google.com/
2. Firestore Database
3. Colección `movies/`
4. Ver documento con `posterUrl`

### Paso 7: Ver en Homepage (Usuarios)

1. Ir a: http://localhost:5173/
2. Ver películas en cartelera
3. Imágenes deberían cargarse correctamente

---

## ⚡ Optimizaciones de Cloudinary

Cloudinary ofrece transformaciones automáticas de imágenes:

### Resize automático:
```
https://res.cloudinary.com/dntcviwyy/image/upload/w_400,h_600,c_fill/movies/dune2.jpg
```

### Optimización de formato:
```
https://res.cloudinary.com/dntcviwyy/image/upload/f_auto,q_auto/movies/dune2.jpg
```

### Aplicar en el frontend:
```dart
String getOptimizedUrl(String originalUrl) {
  return originalUrl.replaceFirst('/upload/', '/upload/f_auto,q_auto,w_400/');
}
```

---

## 🐛 Troubleshooting

### Error: "Image upload failed"

**Causas**:
- Credenciales incorrectas en appsettings.json
- API Key expirada
- Límites de cuenta excedidos

**Solución**:
- Verificar credenciales en Cloudinary Dashboard
- Revisar logs del backend

### Error: "No se puede seleccionar imagen"

**Causas**:
- Paquetes no instalados correctamente
- Permisos del navegador

**Solución**:
```bash
flutter clean
flutter pub get
```

### Imágenes no se ven en el frontend

**Causas**:
- URL incorrecta
- CORS de Cloudinary

**Solución**:
- Verificar URL en Firestore
- Cloudinary permite CORS por defecto

---

## 📚 Próximos Pasos

1. ✅ **Implementar en admin panel** - Widget ImagePickerField listo
2. ⏳ **Actualizar home_page** - Usar providers en lugar de datos hardcodeados
3. ⏳ **Seed inicial** - Subir 24 películas con imágenes
4. ⏳ **Testing completo** - Probar flujo end-to-end

---

## 💡 Tips

- **Límite de cuenta gratuita**: 25 GB almacenamiento, 25 créditos/mes
- **Tamaño recomendado de posters**: 400x600 px (ratio 2:3)
- **Formatos soportados**: JPG, PNG, WEBP, GIF
- **Tamaño máximo por imagen**: 10 MB (cuenta gratuita)
- **Compresión automática**: Cloudinary optimiza imágenes automáticamente

---

**Fecha**: Noviembre 2025
**Versión**: 1.0.0
**Estado**: ✅ Backend completo | ⏳ Frontend en progreso
