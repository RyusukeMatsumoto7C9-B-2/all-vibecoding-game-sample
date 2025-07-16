@echo off
setlocal enabledelayedexpansion

REM Check arguments
if "%~1"=="" (
    echo Error: Color specification is required.
    echo Usage: sprite-create-tool.bat ^(R,G,B,A^)
    echo Example: sprite-create-tool.bat ^(255,255,255,255^)
    echo Example: sprite-create-tool.bat ^(1.0,1.0,1.0,1.0^)
    exit /b 1
)

REM PowerShell script path
set "SCRIPT_DIR=%~dp0"
set "MODULE_PATH=%SCRIPT_DIR%SpriteCreator.psm1"

REM Execute PowerShell command
powershell -ExecutionPolicy Bypass -Command "Import-Module '%MODULE_PATH%'; Create-Sprite -ColorString '%~1'"

endlocal