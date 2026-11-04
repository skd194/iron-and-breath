@echo off
rem Run Iron & Breath locally: API (:5201) + client (:5173) together.
rem Double-click this file, or run `run-dev` from a terminal.
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-dev.ps1" %*
