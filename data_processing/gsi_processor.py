from gsi_encoding import DataEncoding
from database_updator import DatabaseUpdator

class DataProcessor:
    def __init__(self):
        self.game_data = {"player_data": {}, "match_data": {}}
        self.data_encoder = DataEncoding()
        self.database_updator = DatabaseUpdator()
    
    def parse_data(self, data: dict):
        # Tekee gsi-datasta uuden sanakirjan, jossa on vain tarvittava data

        try:
            players = data["allplayers"]
            for steam_id, player in players.items():
                name = player["name"]
                position = player["position"]
                team = player["team"]
                health = player["state"]["health"]
                kills = player["match_stats"]["kills"]
                assists = player["match_stats"]["assists"]
                deaths = player["match_stats"]["deaths"]
                

                self.game_data["player_data"][steam_id] = {
                    "name": name,
                    "position": position,
                    "team": team,
                    "health": health,
                    "kills": kills,
                    "assists": assists,
                    "deaths": deaths
                }
            self.game_data["match_data"]["map"] = data["map"]["name"]
            self.game_data["match_data"]["round"] = data["map"]["round"]

            # Game data syötetään encodingiin ja tietokantaan laitettavaksi
            self.data_encoder.create_json_file(self.game_data)
            self.database_updator.update_database(self.game_data)
            
        except Exception as e:
            print(f"Error processing data: {e}")
            return {"status": "error", "error": str(e)}