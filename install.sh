#!/usr/bin/env bash
#
# Iron & Breath — one-run installer for an Ubuntu DigitalOcean Droplet.
#
# From a fresh droplet (as root or a sudo user):
#
#   git clone https://github.com/skd194/iron-and-breath.git
#   cd iron-and-breath
#   sudo ./install.sh                       # HTTP, access by the droplet IP
#   sudo SITE_ADDRESS=gym.example.com ./install.sh   # auto-HTTPS for a domain
#
# It installs Docker if missing, generates secrets, (optionally) imports your
# existing database from ./data/ironandbreath.db, builds, and starts the stack.
# Safe to re-run — it won't overwrite an existing .env or your data.
set -euo pipefail

cd "$(dirname "$0")"

say()  { printf '\n\033[1;36m==> %s\033[0m\n' "$*"; }
ok()   { printf '\033[1;32m  ✓ %s\033[0m\n' "$*"; }
warn() { printf '\033[1;33m  ! %s\033[0m\n' "$*"; }

COMPOSE_FILE="docker-compose.prod.yml"
[ -f "$COMPOSE_FILE" ] || { echo "Run this from the repo root (missing $COMPOSE_FILE)."; exit 1; }

# ---- 1. Docker ----
if ! command -v docker >/dev/null 2>&1; then
  say "Installing Docker"
  curl -fsSL https://get.docker.com | sh
  ok "Docker installed"
else
  ok "Docker already installed"
fi

# Resolve the compose command (plugin or standalone).
if docker compose version >/dev/null 2>&1; then
  DC="docker compose"
elif command -v docker-compose >/dev/null 2>&1; then
  DC="docker-compose"
else
  say "Installing Docker Compose plugin"
  apt-get update -y && apt-get install -y docker-compose-plugin
  DC="docker compose"
fi
ok "Using: $DC"

# ---- 2. Environment / secrets ----
if [ ! -f .env ]; then
  say "Creating .env"
  cp deploy/.env.prod.example .env
  JWT_KEY="$(openssl rand -hex 32)"
  # Fill JWT_KEY and SITE_ADDRESS (honour an env override, default :80).
  sed -i "s|^JWT_KEY=.*|JWT_KEY=${JWT_KEY}|" .env
  sed -i "s|^SITE_ADDRESS=.*|SITE_ADDRESS=${SITE_ADDRESS:-:80}|" .env
  ok "Generated .env with a random JWT_KEY (SITE_ADDRESS=${SITE_ADDRESS:-:80})"
  warn "Add CLAUDE_API_KEY / GOOGLE_CLIENT_ID to .env later to enable those features."
else
  ok ".env already present — leaving it untouched"
  # Allow SITE_ADDRESS to be updated via env on a re-run.
  if [ -n "${SITE_ADDRESS:-}" ]; then
    sed -i "s|^SITE_ADDRESS=.*|SITE_ADDRESS=${SITE_ADDRESS}|" .env
    ok "Updated SITE_ADDRESS=${SITE_ADDRESS}"
  fi
fi

# ---- 3. Data ----
mkdir -p data
if [ -f data/ironandbreath.db ]; then
  ok "Found existing database at ./data/ironandbreath.db — your data will be used"
else
  warn "No ./data/ironandbreath.db found — starting with a fresh database."
  warn "To import your data, upload it then re-run:"
  warn "  scp server/IronAndBreath.Api/data/ironandbreath.db root@THIS_DROPLET:$(pwd)/data/ironandbreath.db"
fi

# ---- 4. Firewall (best effort) ----
if command -v ufw >/dev/null 2>&1; then
  ufw allow OpenSSH >/dev/null 2>&1 || true
  ufw allow 80/tcp  >/dev/null 2>&1 || true
  ufw allow 443/tcp >/dev/null 2>&1 || true
  ok "Opened firewall ports 80/443 (and SSH)"
fi

# ---- 5. Build & start ----
say "Building and starting the stack (first run can take a few minutes)"
$DC -f "$COMPOSE_FILE" up -d --build

# ---- 6. Done ----
IP="$(curl -s --max-time 5 ifconfig.me 2>/dev/null || echo YOUR_DROPLET_IP)"
SITE="$(grep -E '^SITE_ADDRESS=' .env | cut -d= -f2-)"
say "Iron & Breath is up"
if [ "$SITE" = ":80" ] || [ -z "$SITE" ]; then
  ok "Open:  http://${IP}"
else
  ok "Open:  https://${SITE}   (DNS for ${SITE} must point at ${IP})"
fi
echo
echo "  Logs:     $DC -f $COMPOSE_FILE logs -f"
echo "  Restart:  $DC -f $COMPOSE_FILE restart"
echo "  Update:   git pull && $DC -f $COMPOSE_FILE up -d --build"
echo "  Stop:     $DC -f $COMPOSE_FILE down"
