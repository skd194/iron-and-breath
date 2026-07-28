#!/usr/bin/env bash
# Launch the full Iron & Breath stack (web + api) with Docker Compose.
#   ./docker-up.sh          build + run in the foreground
#   ./docker-up.sh -d       run detached
set -euo pipefail
cd "$(dirname "$0")"

if [ ! -f .env ] && [ -f .env.example ]; then
  cp .env.example .env
  echo "Created .env from .env.example - set JWT_KEY (and GOOGLE_CLIENT_ID) for real use."
fi

echo
echo "  web -> http://localhost:8080"
echo "  api -> http://localhost:5201/swagger"
echo

exec docker compose up --build "$@"
