using UnityEngine;
using NativeWebSocket;

public class WebSocketConnection : MonoBehaviour {

    WebSocket websocket;

    void Update() {

        if (websocket == null) {
            return;
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    public async void StartConnection(string photoshopIP, string port) {
        websocket = new WebSocket("ws://" + photoshopIP + ":" + port);

        websocket.OnOpen += () => {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) => {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) => {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) => {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(message);
        };

        // waiting for messages
        await websocket.Connect();
    }

    public async void SendWebSocketMessage(string msg) {
        if (websocket.State == WebSocketState.Open) {
            // Sending plain text
            await websocket.SendText(msg);
        }
    }

    async void OnApplicationQuit() {

        if (websocket == null) {
            return;
        }

        await websocket.Close();
    }
}
