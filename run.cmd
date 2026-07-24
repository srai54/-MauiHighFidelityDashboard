@echo off
setlocal
rem ------------------------------------------------------------------
rem  HighFidelity.Ui launcher
rem  "dotnet run" can only target ONE framework at a time, so this
rem  script asks which platform you want and runs the right command.
rem ------------------------------------------------------------------
set SDK=C:\Program Files (x86)\Android\android-sdk

echo.
echo  Select platform to run:
echo    [1] Windows
echo    [2] Android (emulator / device)
echo.
choice /C 12 /N /M "  Choose 1 or 2: "
if errorlevel 2 goto android

:windows
echo.
echo  Starting Windows app...
dotnet build -t:Run -f net10.0-windows10.0.19041.0 HighFidelity.Ui.csproj
goto :eof

:android
echo.
echo  Checking for a connected Android device/emulator...
"%SDK%\platform-tools\adb.exe" devices | findstr /R /C:"device$" >nul
if not errorlevel 1 goto deploy

echo  No device found - starting the Pixel 7 emulator (software rendering)...
start "Android Emulator" "%SDK%\emulator\emulator.exe" -avd pixel_7_-_api_36_0 -gpu swiftshader_indirect -no-snapshot -no-boot-anim -no-audio
echo  Waiting for the emulator to boot (this can take a few minutes)...
"%SDK%\platform-tools\adb.exe" wait-for-device
:bootwait
for /f "delims=" %%b in ('"%SDK%\platform-tools\adb.exe" shell getprop sys.boot_completed 2^>nul') do set BOOTED=%%b
if not "%BOOTED%"=="1" (
    timeout /t 5 /nobreak >nul
    goto bootwait
)
echo  Emulator booted.

:deploy
echo  Building and deploying to Android...
dotnet build -t:Run -f net10.0-android HighFidelity.Ui.csproj
goto :eof
