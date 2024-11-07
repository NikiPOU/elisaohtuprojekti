from sqlalchemy import text
from db import create_database

class DatabaseUpdator:
    def __init__(self):
        # Tässä saadaan tietokantayhteys, mutta pitäisi toteuttaa toisella tavalla niin, ettei koko metodia kutsuta "uudelleen"

        self.connection = self.create_database()

    def update_database(self, data: dict):
        # Tässä testaus testitaululle datan lisäämisestä jollain arvoilla

        for steam_id, player_data in data["player_data"].items():
            username = player_data["name"]
            kills = player_data["kills"]
            deaths = player_data["deaths"]
            sql = text("INSERT INTO players_test (steamid, username, kills, deaths) VALUES (:steam_id, :username, :kills, :deaths)")
        
            self.connection.execute(
                sql, {
                    "steam_id": steam_id,
                    "username": username,
                    "kills": kills,
                    "deaths": deaths
                }
            )
            self.connection.commit()