using UnityEngine;
using WebSocketSharp;

public class WsClient : MonoBehaviour
{
    WebSocket ws;

    private void Start()
    {
        ws = new WebSocket("ws://localhost:8080");

        // Subscribe to the OnOpen event to confirm connection success
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection established successfully.");
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message received from " + ((WebSocket)sender).Url + ", Data: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket error: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed.");
        };

        ws.Connect();
    }

    private void Update()
    {
        if (ws == null)
        {
            return;
        }

        // ws.Send("Hello");
    }

    private void OnApplicationQuit()
    {
        // Close WebSocket connection on application exit
        if (ws != null)
        {
            ws.Close();
        }
    }
}
