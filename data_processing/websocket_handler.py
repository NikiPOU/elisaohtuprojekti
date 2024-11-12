import asyncio
import json
import websockets
import os
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler

# Path to the player data file
file_path = 'player_positions.json'
connected_clients = set()

# WebSocket handler to manage connections and send data
async def handler(websocket, path='/'):  # Accept `path` even if unused
    # Register new client
    connected_clients.add(websocket)
    try:
        # Keep the connection open
        async for message in websocket:
            print(f"Received message from client: {message}")
    finally:
        # Unregister client on disconnect
        connected_clients.remove(websocket)

# Function to broadcast data to all connected clients
async def broadcast_data(data):
    if connected_clients:
        message = json.dumps(data)
        await asyncio.wait([client.send(message) for client in connected_clients])

# Event handler to detect changes to player_data.json
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
    # Set up WebSocket server
    async with websockets.serve(handler, "0.0.0.0", 8080):
        print("WebSocket server started on ws://localhost:8080")

        # Set up file watcher
        loop = asyncio.get_event_loop()
        event_handler = FileChangeHandler(loop)
        observer = Observer()
        observer.schedule(event_handler, path='.', recursive=False)
        observer.start()

        # Keep the server running
        try:
            await asyncio.Future()
        finally:
            observer.stop()
            observer.join()

# Run the server
asyncio.run(main())
