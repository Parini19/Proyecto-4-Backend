@echo off
echo ========================================
echo   Cinema - Starting All Services
echo ========================================
echo.
echo This will open 3 windows:
echo   1. Backend API (http://localhost:5000)
echo   2. Frontend Web (http://localhost:5173)
echo   3. Mobile Emulator (Pixel 8)
echo.
echo Press any key to continue...
pause >nul

echo.
echo [1/3] Starting Backend...
start "Cinema Backend" cmd /k "%~dp0start-backend.bat"

echo Waiting 5 seconds for backend to start...
timeout /t 5 /nobreak >nul

echo.
echo [2/3] Starting Frontend Web...
start "Cinema Frontend" cmd /k "%~dp0..\Cinema Frontend\Proyecto-4-Frontend\start-frontend.bat"

echo.
echo [3/3] Launching Mobile Emulator...
start "Cinema Mobile Emulator" cmd /k "flutter emulators --launch Pixel_8 && timeout /t 10 && cd /d "%~dp0..\Cinema Frontend\Proyecto-4-Frontend" && flutter run"

echo.
echo ========================================
echo   All services are starting!
echo ========================================
echo.
echo Backend:   http://localhost:5000/swagger
echo Frontend:  http://localhost:5173
echo Emulator:  Pixel 8 (Android)
echo.
echo Close this window safely. The services are running in separate windows.
echo.
pause
