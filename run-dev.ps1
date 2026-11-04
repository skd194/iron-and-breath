# Run Iron & Breath locally: API (:5201) + client (:5173) together.
# One command, both processes, interleaved logs. Ctrl+C stops both.
#
#   ./run-dev.ps1        run both in the foreground
#
# Works in Windows PowerShell 5.1 and PowerShell 7+.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host ''
Write-Host '  Iron & Breath - starting dev stack' -ForegroundColor Cyan
Write-Host '  app -> http://localhost:5173'
Write-Host '  api -> http://localhost:5201/swagger'
Write-Host ''

$env:ASPNETCORE_ENVIRONMENT = 'Development'
$isWin = ($env:OS -eq 'Windows_NT')
$npm = if ($isWin) { 'npm.cmd' } else { 'npm' }

$procs = @()
try {
  $api = Start-Process -FilePath 'dotnet' -ArgumentList 'run' `
    -WorkingDirectory (Join-Path $root 'server/IronAndBreath.Api') `
    -NoNewWindow -PassThru
  $procs += $api

  $web = Start-Process -FilePath $npm -ArgumentList 'run', 'dev' `
    -WorkingDirectory (Join-Path $root 'client') `
    -NoNewWindow -PassThru
  $procs += $web

  Write-Host "API pid $($api.Id), client pid $($web.Id). Press Ctrl+C to stop both." -ForegroundColor DarkGray
  Wait-Process -Id $api.Id, $web.Id
}
finally {
  foreach ($p in $procs) {
    if ($p -and -not $p.HasExited) {
      Write-Host "Stopping pid $($p.Id)..." -ForegroundColor DarkGray
      if ($isWin) {
        taskkill /PID $p.Id /T /F 2>$null | Out-Null
      }
      else {
        try { $p.Kill($true) } catch {}
      }
    }
  }
}
