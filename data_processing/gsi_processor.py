from gsi_encoding import DataEncoding

class DataProcessor:
    def __init__(self):
        self.player_positions = {}
        self.statistics_data = []
        self.data_encoder = DataEncoding()

    def parse_movement_data_live(self, all_player_data): # Gets player name and position from data and adds them to a dictionary
        try:
            players = all_player_data["allplayers"]
            for _, player in players.items():
                name = player["name"]
                position = player["position"]
                self.player_positions[name] = position

                self.data_encoder.create_json_file(self.player_positions, "player_positions.json")
        except KeyError:
            pass

    def parse_statistics_live(self, all_player_data): # Gets relevant player statistics, converts them to string and adds the string to list
        try:
            players = all_player_data["allplayers"]
            for _, player in players.items():
                name = player["name"]
                team = player["team"]
                health = player["state"]["health"]
                kills = player["match_stats"]["kills"]
                assists = player["match_stats"]["assists"]
                deaths = player["match_stats"]["deaths"]
            
                player_stats = (f"Name: {name} | Team: {team} | Health: {health} | Kills: {kills} | Assists: {assists} | Deaths: {deaths}")
                self.statistics_data.append(player_stats)

                self.data_encoder.create_json_file(self.statistics_data, "statistics.json")
        except KeyError:
            pass

    def parse_utility_live(self, all_player_data):
        for player in all_player_data:
            grenades = player.get("grenades")