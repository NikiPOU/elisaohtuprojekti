import asyncio
import json
import websockets
import os
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler
import time

file_path = os.path.join(os.path.dirname(__file__), "player_positions.json")
connected_clients = set()

# Store the timestamp of the last broadcast
last_broadcast_time = time.time()

async def read_file_async(file_path):
    loop = asyncio.get_event_loop()
    return await loop.run_in_executor(None, lambda: open(file_path, 'r').read())

async def handler(websocket, path='/'):
    connected_clients.add(websocket)
    try:
        if os.path.exists(file_path):
            with open(file_path, 'r') as f:
                data = json.load(f)
            await websocket.send(json.dumps(data))
            print("Initial data sent to new client.")
        else:
            print("Filepath not found.")
        
        async for message in websocket:
            print(f"Received message from client: {message}")
    finally:
        connected_clients.remove(websocket)

# Function to broadcast data to all connected clients
async def broadcast_data(data):
    if connected_clients:
        message = json.dumps(data)
        await asyncio.gather(*[client.send(message) for client in connected_clients])
        print("Data broadcasted to all connected clients.")
    else:
        print("No connected clients to send data to.")

class FileChangeHandler(FileSystemEventHandler):
    def __init__(self, loop):
        super().__init__()
        self.loop = loop

    def on_modified(self, event):
        # Ensure we're dealing with the correct file
        if event.src_path == os.path.abspath(file_path):
            print(f"File modification detected on: {event.src_path}")
            global last_broadcast_time
            if time.time() - last_broadcast_time > 1:  # debounce
                last_broadcast_time = time.time()

                # We need to run async functions here in a thread-safe manner
                self.loop.create_task(self.handle_file_change())

    async def handle_file_change(self):
        # Read the file asynchronously and broadcast it
        data = await read_file_async(file_path)
        await broadcast_data(json.loads(data))

async def main():
    if not os.path.exists(file_path):
        print(f"Error: {file_path} does not exist.")
    else:
        print(f"File {file_path} found successfully.")

    async with websockets.serve(handler, "0.0.0.0", 8080):
        print("WebSocket server started on ws://localhost:8080")

        loop = asyncio.get_event_loop()
        event_handler = FileChangeHandler(loop)
        observer = Observer()
        observer.schedule(event_handler, path=os.path.dirname(file_path), recursive=False)
        observer.start()

        try:
            await asyncio.Future()
        finally:
            observer.stop()
            observer.join()

asyncio.run(main())
