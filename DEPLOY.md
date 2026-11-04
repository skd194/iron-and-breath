# Deploying Iron & Breath to a DigitalOcean Droplet

A one-run installer brings the whole app up on a fresh Ubuntu droplet: the API,
the web UI, and the database — with your existing data if you upload it.

- **Stack:** Caddy (HTTP/HTTPS) → nginx (serves the app + proxies `/api`) → .NET API → SQLite
- **Database:** SQLite, stored on the droplet at `./data/ironandbreath.db`, persisted across restarts/updates. Migrations run automatically on start.
- **Your data:** uploaded once over `scp` (kept out of the public repo).

---

## 1. Create the droplet

- **Image:** Ubuntu 24.04 LTS
- **Size:** Basic / Regular — 1 GB RAM is enough to run; **2 GB** is recommended so the first in-droplet build is comfortable.
- Add your SSH key. Note the droplet's public **IP**.

> Low on RAM? You can build the images elsewhere, but 2 GB avoids that. A 1 GB droplet works if you add a swap file (see Troubleshooting).

## 2. Point a domain at it (optional, for HTTPS)

If you have a domain, create an **A record** for it (e.g. `gym.example.com`) pointing to the droplet's IP. You can skip this and use plain HTTP by IP, then add a domain later.

## 3. Install

SSH into the droplet and run:

```bash
ssh root@YOUR_DROPLET_IP

git clone https://github.com/skd194/iron-and-breath.git
cd iron-and-breath

# HTTP, access by the droplet IP:
sudo ./install.sh

# …or, if you set up a domain in step 2, get automatic HTTPS:
sudo SITE_ADDRESS=gym.example.com ./install.sh
```

The installer:
1. installs Docker (if missing),
2. creates `.env` and generates a secure `JWT_KEY`,
3. uses your database if `./data/ironandbreath.db` is present (see step 4),
4. opens firewall ports 80/443,
5. builds and starts everything.

When it finishes it prints the URL. The **first** build takes a few minutes.

## 4. Bring your existing data

Your current data lives in a local SQLite file on your PC. To use it on the droplet, upload it **once**, then re-run the installer (or just restart).

From your **local machine** (PowerShell or a terminal), in the project folder:

```bash
# Create the target dir on the droplet (first time only):
ssh root@YOUR_DROPLET_IP "mkdir -p /root/iron-and-breath/data"

# Upload your database:
scp server/IronAndBreath.Api/data/ironandbreath.db \
    root@YOUR_DROPLET_IP:/root/iron-and-breath/data/ironandbreath.db
```

Then on the droplet:

```bash
cd iron-and-breath
docker compose -f docker-compose.prod.yml restart api
```

Your account, program, sessions, equipment, nutrition and logged sets are now live on the droplet. (Migrations run automatically, so an older DB is upgraded to the latest schema on start.)

> Prefer a clean slate? Skip this step — the app starts with the seeded 5-day program and you register a new account.

## 5. Log in

- Open the printed URL (`http://YOUR_DROPLET_IP` or `https://your-domain`).
- Sign in with the same email/password you use locally (if you uploaded your DB), or register.

---

## Enabling optional features

Edit `.env` on the droplet, then `docker compose -f docker-compose.prod.yml up -d`:

| Variable | Purpose |
|----------|---------|
| `CLAUDE_API_KEY` | Turns the AI coach from the offline demo into the full streaming coach. |
| `GOOGLE_CLIENT_ID` | Enables Google sign-in. Add your site origin to the OAuth client's authorised origins. |
| `SITE_ADDRESS` | `:80` (HTTP by IP) or a domain (auto-HTTPS). |

## Day-to-day commands

Run these from `~/iron-and-breath` on the droplet:

```bash
# Logs (follow)
docker compose -f docker-compose.prod.yml logs -f

# Restart / stop / start
docker compose -f docker-compose.prod.yml restart
docker compose -f docker-compose.prod.yml down
docker compose -f docker-compose.prod.yml up -d

# Update to the latest code
git pull && docker compose -f docker-compose.prod.yml up -d --build
```

## Backups

The entire database is one file — back it up by copying it:

```bash
# On the droplet: timestamped local copy
cp data/ironandbreath.db "data/backup-$(date +%F).db"

# …or download it to your PC
scp root@YOUR_DROPLET_IP:/root/iron-and-breath/data/ironandbreath.db ./backup.db
```

## Adding a domain later (HTTP → HTTPS)

1. Point an A record at the droplet IP.
2. On the droplet:
   ```bash
   cd iron-and-breath
   sudo SITE_ADDRESS=gym.example.com ./install.sh
   ```
   Caddy obtains a Let's Encrypt certificate automatically and redirects HTTP→HTTPS.

## Troubleshooting

- **First build killed / out of memory (1 GB droplet):** add swap, then re-run the installer:
  ```bash
  fallocate -l 2G /swapfile && chmod 600 /swapfile && mkswap /swapfile && swapon /swapfile
  echo '/swapfile none swap sw 0 0' >> /etc/fstab
  ```
- **Can't reach the site:** check the firewall (`ufw status`) allows 80/443, and `docker compose -f docker-compose.prod.yml ps` shows all three containers `Up`.
- **HTTPS not issued:** the domain's DNS must resolve to the droplet **before** starting with `SITE_ADDRESS=<domain>`. Check `docker compose -f docker-compose.prod.yml logs caddy`.
- **API errors on start:** `docker compose -f docker-compose.prod.yml logs api` — the DB migrates on boot; a corrupt uploaded `.db` would show here.
- **Reset to a fresh database:** `docker compose -f docker-compose.prod.yml down`, remove/rename `data/ironandbreath.db`, then `up -d`.

## Security notes

- `.env` (your `JWT_KEY` and any API keys) and `data/` (your database) are git-ignored — they stay on the droplet and never reach the public repo.
- Keep the droplet patched: `apt update && apt upgrade -y`.
- Use a strong account password; consider restricting SSH to key-only auth.

## Upgrading to PostgreSQL (optional, later)

SQLite is plenty for single-user use. If you ever want managed Postgres (automatic
backups/failover via DigitalOcean), the app already supports it — set
`Database__Provider=Postgres` and `Database__ConnectionString=...` on the `api`
service. Migrating existing rows SQLite→Postgres is a separate step; ask when you
get there.
