#!/usr/bin/env pwsh
# Launch the full Iron & Breath stack (web + api) with Docker Compose.
#   ./docker-up.ps1            build + run in the foreground
#   ./docker-up.ps1 -d         run detached
$ErrorActionPreference = 'Stop'
Set-Location -Path $PSScriptRoot

if (-not (Test-Path '.env') -and (Test-Path '.env.example')) {
    Copy-Item '.env.example' '.env'
    Write-Host 'Created .env from .env.example - set JWT_KEY (and GOOGLE_CLIENT_ID) for real use.' -ForegroundColor Yellow
}

Write-Host ''
Write-Host '  web -> http://localhost:8080'
Write-Host '  api -> http://localhost:5201/swagger'
Write-Host ''

docker compose up --build @args
