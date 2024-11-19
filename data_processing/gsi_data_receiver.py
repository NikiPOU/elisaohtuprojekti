import json
import os
import asyncio
from websocket_handler import WebSocketHandler
from gsi_processor import DataProcessor

class DataReceiver:
    def __init__(self):
        self.websocket_handler = WebSocketHandler()
        loop = asyncio.get_event_loop()
        loop.create_task(self.websocket_handler.start_server())  # Run in background
        self.data_processor = DataProcessor(self.websocket_handler)

    def get_gsi_data(self, data):
        self.data_processor.parse_data(data)


if __name__ == "__main__":
    with open(os.path.join(os.path.dirname(__file__), "test.json"), 'r') as file:
        data = json.load(file)
    receiver = DataReceiver()
    receiver.get_gsi_data(data)
    asyncio.get_event_loop().run_forever()
