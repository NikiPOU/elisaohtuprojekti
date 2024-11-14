import asyncio
import json
import websockets
import os
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler

file_path = 'player_positions.json'
connected_clients = set()

async def handler(websocket, path='/'):
    # Register new client
    connected_clients.add(websocket)
    try:
        async for message in websocket:
            print(f"Received message from client: {message}")
    finally:
        connected_clients.remove(websocket)

# Function to broadcast data to all connected clients
async def broadcast_data(data):
    if connected_clients:
        message = json.dumps(data)
        await asyncio.wait([client.send(message) for client in connected_clients])

# Event handler to detect changes to json files
class FileChangeHandler(FileSystemEventHandler):
    def __init__(self, loop):
        super().__init__()
        self.loop = loop

    def on_modified(self, event):
        if event.src_path == os.path.abspath(file_path):
            # Read and broadcast file contents on modification
            with open(file_path, 'r') as f:
                data = json.load(f)
            asyncio.run_coroutine_threadsafe(broadcast_data(data), self.loop)

# Main function to start the server and file watcher
async def main():
    async with websockets.serve(handler, "0.0.0.0", 8080):
        print("WebSocket server started on ws://localhost:8080")

        # Set up file watcher
        loop = asyncio.get_event_loop()
        event_handler = FileChangeHandler(loop)
        observer = Observer()
        observer.schedule(event_handler, path='.', recursive=False)
        observer.start()

        try:
            await asyncio.Future()
        finally:
            observer.stop()
            observer.join()

asyncio.run(main())
