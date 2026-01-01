@echo off
REM NuGet Publishing Script for Windows CMD
REM Uses NUGET_API_KEY environment variable

setlocal enabledelayedexpansion

echo ========================================
echo   NfcReaderLib NuGet Publishing Script
echo ========================================
echo.

REM Check if API key is set
if "%NUGET_API_KEY%"=="" (
    echo ERROR: NUGET_API_KEY environment variable is not set!
    echo.
    echo Please set it with:
    echo   set NUGET_API_KEY=your-api-key-here
    echo.
    echo Or set it permanently with:
    echo   setx NUGET_API_KEY "your-api-key-here"
    exit /b 1
)

echo [OK] API key found in environment variable
echo.

REM Set variables
set PROJECT=NfcReaderLib.csproj
set CONFIG=Release
set OUTPUT=nupkg

REM Clean output directory
if exist %OUTPUT% (
    echo Cleaning output directory...
    rmdir /s /q %OUTPUT%
)
mkdir %OUTPUT%

REM Build
echo Building project...
dotnet build %PROJECT% --configuration %CONFIG%
if errorlevel 1 (
    echo ERROR: Build failed!
    exit /b 1
)
echo [OK] Build completed
echo.

REM Pack
echo Packing NuGet package...
dotnet pack %PROJECT% --configuration %CONFIG% --no-build --output %OUTPUT%
if errorlevel 1 (
    echo ERROR: Pack failed!
    exit /b 1
)
echo [OK] Package created
echo.

REM Find package file
for %%f in (%OUTPUT%\*.nupkg) do set PACKAGE=%%f
echo Package: %PACKAGE%
echo.

REM Confirm
set /p CONFIRM="Ready to publish to NuGet.org. Continue? (Y/N): "
if /i not "%CONFIRM%"=="Y" (
    echo Publishing cancelled.
    exit /b 0
)
echo.

REM Push to NuGet
echo Publishing to NuGet.org...
dotnet nuget push %OUTPUT%\*.nupkg --api-key %NUGET_API_KEY% --source https://api.nuget.org/v3/index.json --skip-duplicate
if errorlevel 1 (
    echo ERROR: Publishing failed!
    exit /b 1
)

echo.
echo ========================================
echo   Successfully published to NuGet!
echo ========================================
echo.
echo It may take a few minutes to appear in search results.
echo Visit: https://www.nuget.org/packages/NfcReaderLib
echo.

endlocal
