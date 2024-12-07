from gsi_encoding import DataEncoding
from database_updator import DatabaseUpdator

class DataProcessor:
    '''A class that sorts only needed information from 
    given data and adds it to a dictionary. 
    Then it goes to encoding and is saved in database.

    Attributes:
        DataEncoding:
            An instance of the DataEncoding class 
            used for encoding and saving     
            the processed GSI data into JSOn file.
        DatabaseUpdator: 
            An instance of the DatabaseUpdator 
            class used for updating the database.
'''

    def __init__(self):
        '''Initializes the DataProcessor instance. 
        Also sets up instances of dataEncoding and DatabaseUpdator.
        '''
        self.data_encoder = DataEncoding()
        self.database_updator = DatabaseUpdator()
    
    def parse_data(self, data: dict):
        '''Sorts and processes raw GSI data 
        and extracts relevant match and player information into a dict. 
        Also passes it to the DataEncoding and 
        DatabaseUpdator modules for further handling.

        Args: 
            data: 
                dictionary that contains the raw gsi data from CS game.
        '''
        try:
            self.game_data = {"player_data": {}, "match_data": {}}
            players = data["allplayers"]
            for steam_id, player in players.items():
                name = player["name"]
                position = player["position"]
                team = player["team"]
                health = player["state"]["health"]
                kills = player["match_stats"]["kills"]
                assists = player["match_stats"]["assists"]
                deaths = player["match_stats"]["deaths"]
                forward = player["forward"]
                

                self.game_data["player_data"][steam_id] = {
                    "name": name,
                    "position": position,
                    "team": team,
                    "health": health,
                    "kills": kills,
                    "assists": assists,
                    "deaths": deaths,
                    "forward": forward
                }
            self.game_data["match_data"]["map"] = data["map"]["name"]
            self.game_data["match_data"]["round"] = data["map"]["round"]

            self.data_encoder.create_json_file(self.game_data)
            self.database_updator.update_database(self.game_data)
            
        except Exception as e:
            print(f"Error processing data: {e}")
            return {"status": "error", "error": str(e)}