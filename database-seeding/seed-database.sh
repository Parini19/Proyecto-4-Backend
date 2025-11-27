#!/bin/bash
# ========================================
# Cinema Database Seeder Script (Bash)
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

BASE_URL="https://localhost:7238/api"

# Colores
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
RED='\033[0;31m'
WHITE='\033[1;37m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

echo -e "${CYAN}========================================"
echo -e "  Cinema Database Seeder"
echo -e "========================================${NC}"
echo ""

# ========================================
# Paso 1: Theater Rooms (Salas)
# ========================================
echo ""
echo -e "${YELLOW}PASO 1: Theater Rooms (Salas de Cine)"
echo -e "--------------------------------------${NC}"

echo -n "→ Creando salas de cine..."
ROOMS_RESPONSE=$(curl -X POST "${BASE_URL}/theaterrooms/seed?clearExisting=true" -k -s)

if [ $? -eq 0 ]; then
    echo -e " ${GREEN}✓${NC}"

    NORMAL_ROOMS=$(echo $ROOMS_RESPONSE | grep -o '"normalRooms":[0-9]*' | grep -o '[0-9]*')
    VIP_ROOMS=$(echo $ROOMS_RESPONSE | grep -o '"vipRooms":[0-9]*' | grep -o '[0-9]*')
    TOTAL_ROOMS=$(echo $ROOMS_RESPONSE | grep -o '"count":[0-9]*' | head -1 | grep -o '[0-9]*')

    echo ""
    echo -e "${GREEN}  Salas creadas:${NC}"
    echo -e "${WHITE}    • Salas normales: $NORMAL_ROOMS${NC}"
    echo -e "${WHITE}    • Salas VIP: $VIP_ROOMS${NC}"
    echo -e "${WHITE}    • Total: $TOTAL_ROOMS salas${NC}"
else
    echo -e " ${RED}✗${NC}"
    exit 1
fi

# ========================================
# Paso 2: Screenings (Funciones)
# ========================================
echo ""
echo -e "${YELLOW}PASO 2: Screenings (Funciones)"
echo -e "--------------------------------------${NC}"

echo -n "→ Creando funciones..."
SCREENINGS_RESPONSE=$(curl -X POST "${BASE_URL}/screenings/seed?clearExisting=true" -k -s)

if [ $? -eq 0 ]; then
    echo -e " ${GREEN}✓${NC}"

    TOTAL_SCREENINGS=$(echo $SCREENINGS_RESPONSE | grep -o '"count":[0-9]*' | head -1 | grep -o '[0-9]*')
    NOW_PLAYING=$(echo $SCREENINGS_RESPONSE | grep -o '"nowPlayingCount":[0-9]*' | grep -o '[0-9]*')
    POPULAR=$(echo $SCREENINGS_RESPONSE | grep -o '"popularCount":[0-9]*' | grep -o '[0-9]*')
    TOTAL_MOVIES=$(echo $SCREENINGS_RESPONSE | grep -o '"totalMoviesWithScreenings":[0-9]*' | grep -o '[0-9]*')

    echo ""
    echo -e "${GREEN}  Funciones creadas:${NC}"
    echo -e "${WHITE}    • Total: $TOTAL_SCREENINGS funciones${NC}"
    echo -e "${WHITE}    • Películas 'En Cartelera': $NOW_PLAYING${NC}"
    echo -e "${WHITE}    • Películas 'Más Populares': $POPULAR${NC}"
    echo -e "${WHITE}    • Películas con funciones: $TOTAL_MOVIES${NC}"
else
    echo -e " ${RED}✗${NC}"
    exit 1
fi

# ========================================
# Resumen Final
# ========================================
echo ""
echo -e "${GREEN}========================================"
echo -e "  ✓ Base de datos poblada exitosamente"
echo -e "========================================${NC}"
echo ""
echo -e "${CYAN}Resumen:${NC}"
echo -e "${WHITE}  • Salas: $TOTAL_ROOMS${NC}"
echo -e "${WHITE}  • Funciones: $TOTAL_SCREENINGS${NC}"
echo -e "${WHITE}  • Películas con funciones: $TOTAL_MOVIES${NC}"
echo ""
echo -e "${YELLOW}Nota:${NC}"
echo -e "${WHITE}  Las películas 'Próximamente' NO tienen funciones.${NC}"
echo -e "${WHITE}  El administrador debe agregarlas manualmente desde el Panel de Admin.${NC}"
echo ""
