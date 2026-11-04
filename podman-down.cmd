@echo off
REM Stop the Iron & Breath stack. Add -v to also drop the data volume: podman-down.cmd -v
cd /d "%~dp0"
podman compose version >nul 2>&1
if %errorlevel%==0 goto sub
podman-compose down %*
goto :eof
:sub
podman compose down %*
goto :eof
