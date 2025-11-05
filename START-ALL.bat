@echo off
echo ========================================
echo   Cinema - Starting Backend + Frontend
echo ========================================
echo.
echo This will open 2 windows:
echo   1. Backend API (http://localhost:5000)
echo   2. Frontend Web (http://localhost:5173)
echo.
echo Press any key to continue...
pause >nul

echo.
echo Starting Backend...
start "Cinema Backend" cmd /k "%~dp0start-backend.bat"

echo Waiting 5 seconds for backend to start...
timeout /t 5 /nobreak >nul

echo.
echo Starting Frontend...
start "Cinema Frontend" cmd /k "%~dp0..\Cinema Frontend\Proyecto-4-Frontend\start-frontend.bat"

echo.
echo ========================================
echo   Both services are starting!
echo ========================================
echo.
echo Backend:  http://localhost:5000/swagger
echo Frontend: http://localhost:5173
echo.
echo Close this window safely. The services are running in separate windows.
echo.
pause
