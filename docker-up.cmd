@echo off
REM Launch the full Iron & Breath stack (web + api) with Docker Compose.
REM Double-click, or run from a terminal. Pass extra flags, e.g.  docker-up.cmd -d
cd /d "%~dp0"

if not exist ".env" if exist ".env.example" (
  copy /y ".env.example" ".env" >nul
  echo Created .env from .env.example - set JWT_KEY ^(and GOOGLE_CLIENT_ID^) for real use.
)

echo.
echo   web -> http://localhost:8080
echo   api -> http://localhost:5201/swagger
echo.

docker compose up --build %*
