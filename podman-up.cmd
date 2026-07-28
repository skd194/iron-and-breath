@echo off
REM Launch the full Iron & Breath stack (web + api) with Podman Compose.
REM Double-click, or run from a terminal. Pass extra flags, e.g.  podman-up.cmd -d
cd /d "%~dp0"

if not exist ".env" if exist ".env.example" (
  copy /y ".env.example" ".env" >nul
  echo Created .env from .env.example - set JWT_KEY ^(and GOOGLE_CLIENT_ID^) for real use.
)

echo.
echo   web -> http://localhost:8080
echo   api -> http://localhost:5201/swagger
echo.

REM Prefer the built-in "podman compose", fall back to standalone podman-compose.
podman compose version >nul 2>&1
if %errorlevel%==0 goto sub
where podman-compose >nul 2>&1
if %errorlevel%==0 goto standalone
echo ERROR: No compose provider found. Install "podman compose" ^(podman 4.1+^) or podman-compose.
exit /b 1
:sub
podman compose up --build %*
goto :eof
:standalone
podman-compose up --build %*
goto :eof
