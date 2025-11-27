# ========================================
# Cinema Database Seeder Script
# ========================================
# Este script limpia y puebla la base de datos de Firestore
# con datos limpios y organizados.
#
# Orden de ejecución:
# 1. Theater Rooms (Salas de cine)
# 2. Screenings (Funciones de películas)
#
# Nota: Las películas ya deben existir en Firestore antes de ejecutar este script.
# ========================================

$BaseUrl = "https://localhost:7238/api"
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Cinema Database Seeder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Función para hacer requests HTTP con manejo de errores
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Endpoint,
        [string]$Description
    )

    Write-Host "→ $Description..." -NoNewline

    try {
        $response = Invoke-RestMethod `
            -Uri "$BaseUrl$Endpoint" `
            -Method $Method `
            -SkipCertificateCheck `
            -ContentType "application/json"

        Write-Host " ✓" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host " ✗" -ForegroundColor Red
        Write-Host "  Error: $_" -ForegroundColor Red
        throw
    }
}

# ========================================
# Paso 1: Theater Rooms (Salas)
# ========================================
Write-Host ""
Write-Host "PASO 1: Theater Rooms (Salas de Cine)" -ForegroundColor Yellow
Write-Host "--------------------------------------" -ForegroundColor Yellow

$roomsResponse = Invoke-ApiRequest `
    -Method "POST" `
    -Endpoint "/theaterrooms/seed?clearExisting=true" `
    -Description "Creando salas de cine"

Write-Host ""
Write-Host "  Salas creadas:" -ForegroundColor Green
Write-Host "    • Salas normales: $($roomsResponse.normalRooms)" -ForegroundColor White
Write-Host "    • Salas VIP: $($roomsResponse.vipRooms)" -ForegroundColor White
Write-Host "    • Total: $($roomsResponse.count) salas" -ForegroundColor White

# ========================================
# Paso 2: Screenings (Funciones)
# ========================================
Write-Host ""
Write-Host "PASO 2: Screenings (Funciones)" -ForegroundColor Yellow
Write-Host "--------------------------------------" -ForegroundColor Yellow

$screeningsResponse = Invoke-ApiRequest `
    -Method "POST" `
    -Endpoint "/screenings/seed?clearExisting=true" `
    -Description "Creando funciones"

Write-Host ""
Write-Host "  Funciones creadas:" -ForegroundColor Green
Write-Host "    • Total: $($screeningsResponse.count) funciones" -ForegroundColor White
Write-Host "    • Películas 'En Cartelera': $($screeningsResponse.nowPlayingCount)" -ForegroundColor White
Write-Host "    • Películas 'Más Populares': $($screeningsResponse.popularCount)" -ForegroundColor White
Write-Host "    • Películas con funciones: $($screeningsResponse.totalMoviesWithScreenings)" -ForegroundColor White

Write-Host ""
Write-Host "  Películas incluidas:" -ForegroundColor Cyan
foreach ($movie in $screeningsResponse.moviesWithScreenings | Select-Object -First 5) {
    $status = if ($movie.isNowPlaying) { "[En Cartelera]" } else { "[Popular]" }
    Write-Host "    • $status $($movie.title) (Rating: $($movie.rating))" -ForegroundColor White
}
if ($screeningsResponse.moviesWithScreenings.Count -gt 5) {
    Write-Host "    ... y $($screeningsResponse.moviesWithScreenings.Count - 5) más" -ForegroundColor Gray
}

# ========================================
# Resumen Final
# ========================================
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  ✓ Base de datos poblada exitosamente" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Resumen:" -ForegroundColor Cyan
Write-Host "  • Salas: $($roomsResponse.count)" -ForegroundColor White
Write-Host "  • Funciones: $($screeningsResponse.count)" -ForegroundColor White
Write-Host "  • Películas con funciones: $($screeningsResponse.totalMoviesWithScreenings)" -ForegroundColor White
Write-Host ""
Write-Host "Nota:" -ForegroundColor Yellow
Write-Host "  Las películas 'Próximamente' NO tienen funciones." -ForegroundColor White
Write-Host "  El administrador debe agregarlas manualmente desde el Panel de Admin." -ForegroundColor White
Write-Host ""
