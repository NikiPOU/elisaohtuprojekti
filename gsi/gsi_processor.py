import json

class DataProcessor:
    def __init__(self):
        self.player_positions = {}
        self.statistics_data = []

    def parse_movement_data_live(self, all_player_data):
        #print(all_player_data)
        for player in all_player_data:
            name = player.get("name")
            position = player.get("position")
            self.player_positions[name] = position

    def parse_statistics_live(self, all_player_data):
        for player in all_player_data:
            name = player.get("name")
            team = player.get("team")
            health = player.get("health")
            kills = player.get("kills")
            assists = player.get("assists")
            deaths = player.get("deaths")
            
            player_stats = (f"Name: {name} | Team: {team} | Health: {health} | Kills: {kills} | Assists: {assists} | Deaths: {deaths}")
            self.statistics_data.add(player_stats)
