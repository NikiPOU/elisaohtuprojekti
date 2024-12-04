import json

class DataEncoding:

    def create_json_file(self, data: dict):
        # Muodostetaan json tiedostoja tietyille dataseteille

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
        match_data["score_ct"] = data["match_data"]["score_ct"]
        match_data["score_t"] = data["match_data"]["score_t"]
        self.write_json_file(match_data, "match_data.json")
    
    def write_json_file(self, data: dict, file_name: str):
        json_data = json.dumps(data)
        with open(f"{file_name}", "w") as file:
            file.write(json_data)
