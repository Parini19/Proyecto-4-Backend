@echo off
echo ========================================
echo   Cinema Backend - Starting...
echo ========================================
echo.

cd /d "%~dp0src\Cinema.Api"

echo [1/2] Checking .NET installation...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK not found. Please install .NET 9.0 SDK
    echo Download: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo     .NET SDK found!
echo.

echo [2/2] Starting Cinema API...
echo     URL: http://localhost:5000
echo     Swagger: http://localhost:5000/swagger
echo.
echo Press CTRL+C to stop the server
echo ========================================
echo.

dotnet run

pause
