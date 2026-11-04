#!/usr/bin/env bash
# Launch the full Iron & Breath stack (web + api) with Podman Compose.
#   ./podman-up.sh          build + run in the foreground
#   ./podman-up.sh -d       run detached
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

# Prefer the built-in "podman compose", fall back to standalone podman-compose.
if podman compose version >/dev/null 2>&1; then
  exec podman compose up --build "$@"
elif command -v podman-compose >/dev/null 2>&1; then
  exec podman-compose up --build "$@"
else
  echo "No compose provider found. Install 'podman compose' (podman 4.1+) or podman-compose." >&2
  exit 1
fi
