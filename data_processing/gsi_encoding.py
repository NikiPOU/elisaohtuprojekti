import json
import asyncio

class DataEncoding:
    def __init__(self, websocket_handler):
        self.websocket_handler = websocket_handler

    def create_json_file(self, data: dict):
        position_data = {}
        for steam_id, player_data in data["player_data"].items():
            position_data[steam_id] = player_data["position"]
        self.write_json_file(position_data, "player_positions.json")

    def write_json_file(self, data: dict, file_name: str):
        json_data = json.dumps(data)
        with open(file_name, "w") as file:
            file.write(json_data)
        print(f"File {file_name} updated.")

        if self.websocket_handler.connected_clients:
            asyncio.create_task(self.websocket_handler.broadcast(data))

