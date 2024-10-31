import json

class DataProcessor:
    def __init__(self):
        self.player_positions = {}
        self.statistics_data = []

    def parse_movement_data_live(self, all_player_data): # Gets player name and position from data and adds the to a dictionary
        try:
            players = all_player_data["allplayers"]
            for id, player in players.items():
                name = player["name"]
                position = player["position"]
                self.player_positions[name] = position
                json_positions = json.dumps(self.player_positions)
                with open("gsi/player_positions.json", "w") as file:
                    file.write(json_positions)
        except KeyError:
            pass

    def parse_statistics_live(self, all_player_data): # Gets relevant player statistics, converts them to string and adds the string to list
        try:
            players = all_player_data["allplayers"]
            for id, player in players.items():
                name = player["name"]
                team = player["team"]
                health = player["state"]["health"]
                kills = player["match_stats"]["kills"]
                assists = player["match_stats"]["assists"]
                deaths = player["match_stats"]["deaths"]
            
                player_stats = (f"Name: {name} | Team: {team} | Health: {health} | Kills: {kills} | Assists: {assists} | Deaths: {deaths}")
                self.statistics_data.append(player_stats)

                json_statistics = json.dumps(self.statistics_data)
                with open("gsi/statistics.json", "w") as file:
                    file.write(json_statistics)
        except KeyError:
            pass

    def parse_utility_live(self, all_player_data):
        for player in all_player_data:
            grenades = player.get("grenades")