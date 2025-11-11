@echo off
echo ========================================
echo   Cinema - Starting Mobile Emulator
echo ========================================
echo.
echo Launching Pixel 8 emulator...
echo.

REM Launch emulator
flutter emulators --launch Pixel_8

echo.
echo Waiting 15 seconds for emulator to boot...
timeout /t 15 /nobreak

echo.
echo Starting Flutter app on emulator...
cd /d "%~dp0..\Cinema Frontend\Proyecto-4-Frontend"
flutter run -d emulator-5554

echo.
echo Mobile app closed.
pause
