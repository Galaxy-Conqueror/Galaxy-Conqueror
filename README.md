# 🌌 Galaxy Conqueror

**Galaxy Conqueror** is a multiplayer game where players compete to dominate the galaxy. Each player starts with a single planet and a spaceship. Players mine resources, upgrade defenses, and battle others to conquer enemy planets and rise to galactic supremacy.

---

## 🚀 Local Development

### 🐳 Docker Setup (via Docker Desktop)

To get started locally, use Docker and Docker Compose:

#### 🔧 Commands

```bash
# Start services in foreground
docker-compose up

# Start services in detached mode (recommended)
docker-compose up -d

# Stop all services
docker-compose down

# Stop and remove all volumes (recommended clean shutdown)
docker-compose down -v
```

### 🧪 Features

- Test Flyway migrations
- Connect to the database via PG Admin
- Interact with the database via the Galaxy Conqueror API

## 🧬 MVP (Minimum Viable Product)

### 👨‍🚀 Player

Upon joining, a player:

- Sets a username
- Is assigned a randomly located planet
- Is given a basic spaceship and planet defenses

Each player:

- Can own only one planet
- Can use only one spaceship

### 🪐 Planet

Each planet:

- Has infinite resources
- Can only be owned by one player
- Has upgradable defensive mechanisms

### 🛡️ Defensive Mechanism

- Defends planet during battle
- Takes damage from incoming attacks
- Is automatically restored after battle

### ⛏️ Resource Extractor

- Extracts resources over time
- Can be upgraded to improve efficiency

### 🛸 Spaceship

- Roams the galaxy, enabling attacks
- Has an upgradable weapon
- Requires fuel to travel
- Can only be refueled at the player's home planet

### 🔫 Spaceship Weapon

- Used to attack enemy planets
- Upgradable for increased combat strength

## 🕹️ Gameplay Loop

### 🌌 Attacking a Planet

1. Travel to an enemy planet.
2. Initiate battle.
3. Engage in a Space Invaders-style minigame:
   - Shoot at enemy defenses
   - Dodge incoming bullets
4. Win by destroying defenses before the spaceship is destroyed.
5. Collect enemy resources and return to your home planet.

## 👾 New Player User Story

### 🌟 On Joining the Game

- Set a username
- Receive a randomly placed planet
- Name the planet
- Start with level 1 in:
  - Planet
  - Spaceship
  - Defenses
  - Resource extractor

### 🧭 Gameplay Overview

- View a description of your planet (maybe with ASCII art!)
- View a galaxy map with symbols and legend

### 🌐 Global Commands

- Fly to Another Planet
  - If fuel runs out → spaceship respawns at home planet (reset to level 1)
- View Battle History (Phase 2)
- View Player Stats (Phase 2)
- View Leaderboard (Phase 2)

### 🏠 At Your Home Planet

- Upgrades
  - Show cost for all available upgrades
  - Require user confirmation
- Actions:
  - Upgrade resource extractor
  - Upgrade turret
  - Upgrade spaceship
  - Repair spaceship
  - Deposit resources
  - Refuel

### 🚩 At an Enemy Planet

- Observe Planet
  - See owner, name, description, (optional: masked stats)
- Attack Planet
  - Pull necessary data and initiate battle
- Recon (Phase 2)
  - Use fuel to inspect planet's defense strength

### ⚔️ In Battle

- Move spaceship
- Shoot defenses
- Dodge bullets
- Forfeit battle
- Battle ends automatically after timeout

### 🏆 If You Win

- Load planet resources into spaceship (limited to max capacity)
- Deduct resources from the defeated planet
- Enemy turret health is restored
- Planet enters a shielded state temporarily

### 💀 If You Lose

- Respawn at home planet (reset spaceship to level 1)
- Enemy defenses are restored automatically
