-- USERS
INSERT INTO users (id, google_id, email, username) VALUES
('11111111-1111-1111-1111-111111111111', 'google-uid-1', 'user1@example.com', 'CommanderOne'),
('22222222-2222-2222-2222-222222222222', 'google-uid-2', 'user2@example.com', 'StarPilot');

-- PLANETS
INSERT INTO planets (user_id, name, design, description, resource_reserve, x, y) VALUES
('11111111-1111-1111-1111-111111111111', 'Terra Prime', 'Desert', 'First colonized world', 500, 10, 20),
('22222222-2222-2222-2222-222222222222', 'Nova Terra', 'Ice', 'Remote mining colony', 300, 30, 40);

-- RESOURCE EXTRACTORS
INSERT INTO resource_extractors (planet_id, level) VALUES
(1, 2),
(2, 3);

-- TURRETS
INSERT INTO turrets (planet_id, level) VALUES
(1, 1),
(2, 2);

-- SPACESHIPS
INSERT INTO spaceships (user_id, design, description, level, current_fuel, current_health, resource_reserve, x, y) VALUES
('11111111-1111-1111-1111-111111111111', 'Falcon', 'Fast attack ship', 3, 100, 90, 200, 15, 25),
('22222222-2222-2222-2222-222222222222', 'Titan', 'Heavy defense ship', 2, 80, 100, 150, 35, 45);

-- BATTLES
INSERT INTO battles (
    attacker_spaceship_id, defender_planet_id, started_at, ended_at,
    attacker_won, damage_to_spaceship, damage_to_turret, resources_looted
) VALUES
(1, 2, '2025-04-08 12:00:00', '2025-04-08 12:05:00', TRUE, 20, 40, 100),
(2, 1, '2025-04-08 13:00:00', '2025-04-08 13:10:00', FALSE, 50, 10, 0);
