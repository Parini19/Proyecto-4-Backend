@echo off
echo ========================================
echo   Cinema - First Time Setup
echo ========================================
echo.
echo This script will prepare your environment for development.
echo.
echo Checking requirements...
echo.

REM Check .NET
echo [1/3] Checking .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo    [X] .NET SDK NOT FOUND
    echo.
    echo    Please install .NET 9.0 SDK from:
    echo    https://dotnet.microsoft.com/download/dotnet/9.0
    echo.
    set MISSING=1
) else (
    echo    [OK] .NET SDK installed
)

REM Check Flutter
echo.
echo [2/3] Checking Flutter SDK...
flutter --version >nul 2>&1
if errorlevel 1 (
    echo    [X] Flutter SDK NOT FOUND
    echo.
    echo    Please install Flutter from:
    echo    https://flutter.dev/docs/get-started/install/windows
    echo.
    set MISSING=1
) else (
    echo    [OK] Flutter SDK installed
)

REM Check Developer Mode (Flutter requirement on Windows)
echo.
echo [3/3] Checking Windows Developer Mode...
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" /v AllowDevelopmentWithoutDevLicense >nul 2>&1
if errorlevel 1 (
    echo    [!] Developer Mode not enabled or cannot detect
    echo.
    echo    To enable Developer Mode:
    echo    1. Press Windows + I to open Settings
    echo    2. Go to: Privacy and security ^> For developers
    echo    3. Enable: Developer Mode
    echo.
    echo    Or run: start ms-settings:developers
    echo.
    set DEVMODE_WARNING=1
) else (
    echo    [OK] Developer Mode enabled
)

if defined MISSING (
    echo.
    echo ========================================
    echo [!] SETUP INCOMPLETE
    echo ========================================
    echo Please install the missing requirements above.
    pause
    exit /b 1
)

echo.
echo ========================================
echo [!] Installing Dependencies
echo ========================================
echo.

REM Setup Backend
echo [1/2] Setting up Backend...
cd /d "%~dp0src\Cinema.Api"
echo     Restoring NuGet packages...
dotnet restore
if errorlevel 1 (
    echo     [X] Backend setup failed
    pause
    exit /b 1
)
echo     [OK] Backend ready
echo.

REM Setup Frontend
echo [2/2] Setting up Frontend...
cd /d "%~dp0..\Cinema Frontend\Proyecto-4-Frontend"
echo     Installing Flutter packages...
flutter pub get
if errorlevel 1 (
    echo     [X] Frontend setup failed
    pause
    exit /b 1
)
echo     [OK] Frontend ready
echo.

if defined DEVMODE_WARNING (
    echo ========================================
    echo [!] IMPORTANT: Enable Developer Mode
    echo ========================================
    echo.
    echo Flutter requires Developer Mode on Windows.
    echo.
    echo Would you like to open Developer settings now? (Y/N)
    set /p OPEN_SETTINGS=
    if /i "%OPEN_SETTINGS%"=="Y" (
        start ms-settings:developers
        echo.
        echo After enabling Developer Mode, re-run this setup.
    )
    echo.
)

echo.
echo ========================================
echo [OK] SETUP COMPLETE!
echo ========================================
echo.
echo You can now start the project using:
echo.
echo   START-ALL.bat       - Start both Backend + Frontend
echo   start-backend.bat   - Start Backend only
echo   or in Frontend folder: start-frontend.bat
echo.
echo Happy coding!
echo.
pause
