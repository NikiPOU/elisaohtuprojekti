import asyncio
import json
import websockets

class WebSocketHandler:
    def __init__(self):
        self.connected_clients = set()

    async def handler(self, websocket, path='/'):
        self.connected_clients.add(websocket)
        try:
            async for message in websocket:
                print(f"Received from client: {message}")
        finally:
            self.connected_clients.remove(websocket)

    async def start_server(self):
        print("WebSocket server starting on ws://localhost:8080")
        async with websockets.serve(self.handler, "0.0.0.0", 8080):
            await asyncio.Future()  # Keep the server running

    async def broadcast(self, data):
        if self.connected_clients:
            message = json.dumps(data)
            await asyncio.gather(*[client.send(message) for client in self.connected_clients])
            print("Broadcasted data to all connected clients.")
        else:
            print("No clients connected.")

