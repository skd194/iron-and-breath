@echo off
REM Stop the Iron & Breath stack. Add -v to also drop the data volume: docker-down.cmd -v
cd /d "%~dp0"
docker compose down %*
