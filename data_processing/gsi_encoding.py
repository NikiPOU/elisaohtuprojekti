import json

class DataEncoding:
    '''
    A class that encodes and saves data
    into different JSON files based on the content of the data.
    '''

    def create_json_file(self, data: dict):
        '''
        A method that processes a dictionary containing gsi data.
        This method also extracts parts of the data and saves them 
        into different JSON files accordingly.
        
        Args: 
        data: 
            Is a dictionary containing nested gsi data, 
            which is information about CS game, 
            like player name, player health, kda and match statistics etc
        '''


        position_data = {}
        for steam_id, player_data in data["player_data"].items():
            position_data[steam_id] = player_data["position"],player_data["name"], player_data["team"], player_data["health"], player_data["forward"]
        self.write_json_file(position_data, "player_positions.json")

        statistics = {}
        for steam_id, player_data in data["player_data"].items():
            statistics[steam_id] = player_data["name"], player_data["team"], player_data["health"],\
            player_data["kills"], player_data["assists"], player_data["deaths"]
        self.write_json_file(statistics, "statistics.json")

        match_data = {}
        match_data["map"] = data["match_data"]["map"]
        match_data["round"] = data["match_data"]["round"]
        self.write_json_file(match_data, "match_data.json")
    
    def write_json_file(self, data: dict, file_name: str):
        '''
        Turns a Python dictionary into JSON format 
        and writes it to a specified file.

        Args:
        data: 
                The dictionary to be made into JSON.
        file_name: 
                The name of the file where the 
                JSON data will be saved.

        This method uses `json.dumps` to convert the dictionary to a JSON-formatted string
        and writes it to the specified file. If the file already exists, its contents
        will be overwritten.
        '''
        json_data = json.dumps(data)
        with open(f"{file_name}", "w") as file:
            file.write(json_data)
