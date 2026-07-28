#!/usr/bin/env pwsh
# Launch the full Iron & Breath stack (web + api) with Podman Compose.
#   ./podman-up.ps1            build + run in the foreground
#   ./podman-up.ps1 -d         run detached
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

# Prefer the built-in "podman compose", fall back to standalone podman-compose.
& podman compose version 2>$null | Out-Null
if ($LASTEXITCODE -eq 0) {
    podman compose up --build @args
}
elseif (Get-Command podman-compose -ErrorAction SilentlyContinue) {
    podman-compose up --build @args
}
else {
    throw 'No compose provider found. Install "podman compose" (podman 4.1+) or podman-compose.'
}
