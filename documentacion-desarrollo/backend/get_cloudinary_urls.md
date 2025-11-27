# 📋 Cómo Obtener URLs de Cloudinary

## Método 1: Individual (Recomendado para pocas imágenes)

1. Ve a: https://console.cloudinary.com/console/media_library
2. Click en cada imagen
3. En el panel derecho, busca "URL"
4. Click en "Copy URL"
5. Pégala en una lista

## Método 2: Usando la Consola del Navegador (Rápido para muchas imágenes)

1. **Abre Media Library en Cloudinary**
2. **Asegúrate de que todas las imágenes estén visibles** (scroll si es necesario)
3. **Abre DevTools** (F12)
4. **Ve a la consola** (Console tab)
5. **Pega este código**:

```javascript
// Obtener todas las URLs de imágenes en la página actual
const images = document.querySelectorAll('img[src*="cloudinary"]');
const urls = Array.from(images)
  .map(img => img.src)
  .filter(url => url.includes('upload'))
  .map(url => {
    // Limpiar la URL (quitar transformaciones)
    const match = url.match(/(https:\/\/res\.cloudinary\.com\/[^\/]+\/image\/upload\/v\d+\/.*?\.(jpg|png|webp|jpeg))/i);
    return match ? match[1] : url;
  });

// Mostrar URLs
console.log('=== URLs de Cloudinary ===');
urls.forEach((url, index) => {
  console.log(`${index + 1}. ${url}`);
});

// Copiar al portapapeles
copy(urls.join('\n'));
console.log('\n✅ URLs copiadas al portapapeles!');
```

6. **Las URLs se copiarán automáticamente**
7. **Pégalas en un archivo de texto**

## Método 3: Usando la API de Cloudinary (Avanzado)

Si tienes muchas imágenes, puedes usar la API:

```bash
curl "https://api.cloudinary.com/v1_1/dntcviwyy/resources/image" \
  -u "889753441957871:6uodHCwefoMUwU9aNXmO4Lk7BEw"
```

---

## 📝 Formato para Darme las URLs

Por favor, dame las URLs en este formato:

```
1. https://res.cloudinary.com/dntcviwyy/image/upload/v1234567/dune_2.jpg
2. https://res.cloudinary.com/dntcviwyy/image/upload/v1234567/kung_fu_panda_4.jpg
3. https://res.cloudinary.com/dntcviwyy/image/upload/v1234567/godzilla_kong.jpg
...
24. https://res.cloudinary.com/dntcviwyy/image/upload/v1234567/avatar_2.jpg
```

O simplemente las 24 URLs, una por línea.

---

## 🎯 Siguiente Paso

Una vez que me des las URLs, yo:
1. Actualizaré el `MoviesSeederController.cs` con las URLs reales
2. Te diré cómo ejecutar el seeder
3. Las 24 películas se guardarán en Firestore con sus imágenes
