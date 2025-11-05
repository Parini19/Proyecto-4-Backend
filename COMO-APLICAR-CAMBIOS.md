# 🔥 Cómo Aplicar Cambios en Flutter (Hot Reload)

## 📍 Situación Actual:
✅ La app está corriendo en: **http://localhost:5200**
✅ Terminal con Flutter activa

---

## 🎯 MÉTODO 1: Hot Reload Desde la Terminal (MÁS FÁCIL)

### Paso 1: Encuentra la terminal donde está corriendo Flutter
Busca la ventana de PowerShell/CMD que muestra esto:
```
Flutter run key commands.
r Hot reload.
R Hot restart.
q Quit (terminate the application on the device).
```

### Paso 2: Click en esa terminal para darle foco

### Paso 3: Presiona la tecla `r` y luego Enter

**Verás algo como:**
```
Performing hot reload...
Reloaded 2 of 1234 libraries in 847ms.
```

✅ **¡LISTO!** Los cambios ya están aplicados. Regresa al navegador y verás los cambios.

---

## 🎯 MÉTODO 2: Hot Restart Completo (Si Hot Reload no funciona)

### En la misma terminal, presiona: `R` (mayúscula) + Enter

**Verás:**
```
Performing hot restart...
Restarted application in 2,345ms.
```

✅ Esto reinicia la app completamente con todos los cambios.

---

## 🎯 MÉTODO 3: Refrescar en el Navegador (Si prefieres)

Simplemente presiona **F5** o **Ctrl+R** en Chrome.

⚠️ **Nota:** Esto recarga toda la página, no es tan rápido como Hot Reload.

---

## 🔍 ¿QUÉ CAMBIOS VERÁS?

Después de hacer Hot Reload (`r`), verifica estos cambios:

### ✅ En la HomePage (http://localhost:5200):
1. **Navegación funcional:**
   - Click en "Cartelera" → Scroll suave a la sección
   - Click en "Próximos" → Scroll suave a esa sección
   - Click en "Promociones" → Mensaje "¡Próximamente!"

2. **Botón del Hero:**
   - Ahora dice **"Comprar Boletos"** 🎟️
   - Antes decía "Ver Ahora" ▶️

### ✅ En LoginPage:
1. **Click en "Iniciar Sesión"** desde HomePage
2. Verás:
   - Slogan: **"Reserva tus boletos en línea"**
   - Scroll bar en el **borde derecho** (ya no al lado del contenido)

3. **Click en "Continuar como invitado":**
   - Te regresa a HomePage nueva (no a la vieja)

---

## 📊 COMANDOS ÚTILES EN LA TERMINAL DE FLUTTER:

Mientras la app está corriendo, puedes presionar:

| Tecla | Acción | Velocidad |
|-------|--------|-----------|
| **`r`** | Hot Reload (cambios rápidos) | ⚡ 1-3 seg |
| **`R`** | Hot Restart (reinicio completo) | 🔄 3-10 seg |
| **`h`** | Ver todos los comandos | - |
| **`c`** | Limpiar consola | - |
| **`q`** | Salir (cerrar la app) | - |
| **`d`** | Detach (dejar app corriendo, cerrar Flutter) | - |

---

## 🐛 TROUBLESHOOTING:

### ❌ "No pasa nada cuando presiono `r`"
**Solución:**
1. Asegúrate de que la terminal tenga el foco (click en ella)
2. Verifica que no haya errores en la terminal
3. Intenta **`R`** (mayúscula) para restart completo
4. Si nada funciona, presiona `q` para salir y corre de nuevo:
   ```bash
   flutter run -d chrome --web-port 5200
   ```

### ❌ "Veo errores en la terminal después de `r`"
**Solución:**
1. Lee el error (normalmente dice qué archivo tiene problemas)
2. Si es un error de sintaxis, Claude ya lo arregló, solo presiona `R` para restart
3. Si persiste, avísame y lo reviso

### ❌ "La app se cerró / No está corriendo"
**Solución:**
Vuelve a correrla:
```bash
cd "C:\Users\Guillermo Parini\Documents\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d chrome --web-port 5200
```

---

## 💡 TIPS:

1. **Guarda archivos antes de Hot Reload:**
   - Si estás editando código manualmente, guarda (Ctrl+S) antes de hacer `r`

2. **Hot Reload no funciona para todo:**
   - Si cambias `initState()`, usa `R` (restart)
   - Si agregas imports nuevos, usa `R` (restart)
   - Si cambias constantes globales, usa `R` (restart)

3. **Chrome DevTools:**
   - Presiona F12 en Chrome para ver errores de consola
   - Útil para debugging

---

## 🎯 RESUMEN RÁPIDO:

```
1. Hice cambios en el código ✅
2. Terminal de Flutter está corriendo ✅
3. Tú presionas "r" en la terminal ⌨️
4. Esperas 2-3 segundos ⏱️
5. ¡Ves los cambios en el navegador! 🎉
```

---

## 🚀 PRÓXIMOS PASOS:

Una vez que veas los cambios:

1. **Prueba la navegación:**
   - Click en los links del menú
   - Verifica scroll suave

2. **Prueba el flujo de invitado:**
   - Click "Iniciar Sesión"
   - Click "Continuar como invitado"
   - Debe volver a HomePage

3. **Verifica los textos:**
   - "Comprar Boletos" en lugar de "Ver Ahora"
   - "Reserva tus boletos en línea" en login

---

**Si algo no funciona, avísame y lo revisamos juntos!** 😊
