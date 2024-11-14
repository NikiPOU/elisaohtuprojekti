# test_client.py
import asyncio
import websockets

async def test_client():
    uri = "ws://localhost:8080"
    async with websockets.connect(uri) as websocket:
        print("Connected to WebSocket server")
        await websocket.send("Hello, server!")  # Send a test message to the server
        
        # Continuously listen for messages
        try:
            while True:
                message = await websocket.recv()
                print("Received from server:", message)
        except websockets.exceptions.ConnectionClosed:
            print("Connection closed")

asyncio.run(test_client())
