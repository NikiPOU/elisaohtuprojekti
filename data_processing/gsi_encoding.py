import json

class DataEncoding:

    def create_json_file(self, data: dict):
        # Muodostetaan json tiedostoja tietyille dataseteille

        position_data = {}
        for steam_id, player_data in data["player_data"].items():
            position_data[steam_id] = player_data["position"]
        self.write_json_file(position_data, "player_positions.json")

        # TO DO: Sama statistiikalle tähän
    
    def write_json_file(self, data: dict, file_name: str):
        json_data = json.dumps(data)
        with open(f"{file_name}", "w") as file:
            file.write(json_data)
