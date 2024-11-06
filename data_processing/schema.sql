DROP TABLE IF EXISTS players_test CASCADE;

CREATE TABLE players_test (
    id SERIAL PRIMARY KEY,
    username TEXT,
    kills INT,
    deaths INT
);
