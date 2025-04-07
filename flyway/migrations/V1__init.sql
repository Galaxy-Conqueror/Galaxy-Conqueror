CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    google_id VARCHAR(255) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    username VARCHAR(255)
);

CREATE TABLE planets (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL,
    name VARCHAR(255),
    design VARCHAR(255),
    description VARCHAR(255),
    resource_reserve INTEGER,
    x INTEGER,
    y INTEGER,
    CONSTRAINT fk_planet_user FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE
);

CREATE TABLE resource_extractors (
    id SERIAL PRIMARY KEY,
    planet_id INTEGER NOT NULL,
    level INTEGER,
    CONSTRAINT fk_extractor_planet FOREIGN KEY (planet_id)
        REFERENCES planets(id)
        ON DELETE CASCADE
);

CREATE TABLE turrets (
    id SERIAL PRIMARY KEY,
    planet_id INTEGER NOT NULL,
    level INTEGER,
    CONSTRAINT fk_turret_planet FOREIGN KEY (planet_id)
        REFERENCES planets(id)
        ON DELETE CASCADE
);

CREATE TABLE spaceships (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL,
    design VARCHAR(255),
    description VARCHAR(255),
    level INTEGER,
    current_fuel INTEGER,
    current_health INTEGER,
    resource_reserve INTEGER,
    x INTEGER,
    y INTEGER,
    CONSTRAINT fk_spaceship_user FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE
);

CREATE TABLE battles (
    id SERIAL PRIMARY KEY,
    attacker_spaceship_id INTEGER NOT NULL,
    defender_planet_id INTEGER NOT NULL,
    started_at TIMESTAMP,
    ended_at TIMESTAMP,
    attacker_won BOOLEAN,
    damage_to_spaceship INTEGER,
    damage_to_turret INTEGER,
    resources_looted INTEGER,
    CONSTRAINT fk_battle_attacker FOREIGN KEY (attacker_spaceship_id)
        REFERENCES spaceships(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_battle_defender FOREIGN KEY (defender_planet_id)
        REFERENCES planets(id)
        ON DELETE CASCADE
);