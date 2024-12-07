from sqlalchemy import text
from db import db

class DatabaseUpdator:
    def __init__(self):
        '''A class that tests if data can be added to the database.'''
        pass

    def update_database(self, data: dict):
        '''In this class we get a database connection, it should 
        be implemented in another way so that the whole method is not called “again”.
        '''


        for steam_id, player_data in data["player_data"].items():
            username = player_data["name"]
            kills = player_data["kills"]
            deaths = player_data["deaths"]
            sql = text(
            "INSERT INTO players_test (steamid, username, kills, deaths) "
            "VALUES (:steam_id, :username, :kills, :deaths) "
            "ON CONFLICT (steamid) DO UPDATE "
            "SET kills = :kills, deaths = :deaths"
            )

            db.session.execute(
                sql, {
                    "steam_id": steam_id,
                    "username": username,
                    "kills": kills,
                    "deaths": deaths
                }
            )
            db.session.commit()