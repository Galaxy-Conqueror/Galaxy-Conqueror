# Local Development

## Docker Setup - Docker Desktop

### Commands:

1. "docker-compose up"
2. "docker-compose up -d" (preferred)
3. "docker-compose down"
4. "docker-compose down -v" (preferred)

### Functions:

1. Test Flyway migrations
2. Connect to the Database through PG Admin
3. Connect to the Database through Galaxy Conqueror API

# Galaxy Conqueror

In "Galaxy Conqueror," each player starts with a single planet that generates resources over time. The goal is to manage your resources, upgrade your planet's defences, or build forces to attack other players' planets. You can mine resources with workers, which you then use to strengthen your planet or create ships and weapons for attacking other players.

Players can challenge each other in turn-based battles, and the winner takes over the defeated player's planet. The game focuses on simple resource management, upgrades, and battles with the aim of becoming the most powerful player in the galaxy.

## MVP

### Player

- Upon joining, a player is given a single planet with a basic defensive mechanism, a resource extractor, and a spaceship with a basic offensive weapon.
- A player will only be allowed to have one planet.
- A player will only have one spaceship.
- A player's planet will be generated automatically and placed in a random spot when they register.

### Planet

- A planet has an infinite resource supply, which can be extracted and processed.

### Defensive Mechanism

- One defensive mechanism per planet which can be upgraded.
- It should be able to shoot an enemy spaceship when shot at.
- If attacked, its health will be decreased whenever it is shot.
- Once it has been destroyed, its health will be restored after attack.

### Resource Extractor

- A device that extracts resources. It can be upgraded to yield more resources over time.

### Spaceship

- Used to roam the galaxy and conquer planets.
- It has an upgradable weapon for combat and requires fuel (resources) to travel, refuelling at its origin planet.

### Spaceship Weapon

- An upgradable weapon used for attacking planets during combat.

## Gameplay

### A Player Attacks a Planet

1. A player will fly their spaceship to an enemy planet.
2. A player will initiate an attack.
3. A Space Invaders-styled game will begin where the player needs to shoot the enemy planet's defence mechanism. The defence mechanism will shoot bullets at the player's spaceship.
4. The battle is concluded when either the spaceship or the enemy defence mechanism has been sufficiently damaged.
5. The player will then load the enemy resources into their spaceship (as much as it can hold) and deposit the resources back at their own planet.
