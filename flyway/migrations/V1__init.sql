CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    google_id TEXT NOT NULL UNIQUE,
    email TEXT NOT NULL UNIQUE,
    username TEXT NOT NULL
);

INSERT INTO users (email, google_id, username) VALUES
('commander1@galaxy.io', 'g123', 'CommanderOne'),
('strategist2@galaxy.io', 'g456', 'StrategistTwo'),
('explorer3@galaxy.io', 'g789', 'ExplorerThree');