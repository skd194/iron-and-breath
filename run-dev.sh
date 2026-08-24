#!/usr/bin/env bash
# Run Iron & Breath locally: API (:5201) + client (:5173) together.
# One command, both processes, interleaved logs. Ctrl+C stops both.
#
#   ./run-dev.sh
set -euo pipefail
cd "$(dirname "$0")"

export ASPNETCORE_ENVIRONMENT=Development

echo
echo "  Iron & Breath - starting dev stack"
echo "  app -> http://localhost:5173"
echo "  api -> http://localhost:5201/swagger"
echo

( cd server/IronAndBreath.Api && exec dotnet run ) &
API=$!
( cd client && exec npm run dev ) &
WEB=$!

# Stop both children on Ctrl+C / exit.
trap 'kill "$API" "$WEB" 2>/dev/null || true' INT TERM EXIT

echo "API pid $API, client pid $WEB. Press Ctrl+C to stop both."
wait
