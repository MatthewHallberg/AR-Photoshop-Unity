using UnityEngine;
using NativeWebSocket;
using System.Collections;

public class WebSocketConnection : MonoBehaviour {

    public delegate void OnMessageRecieved(ImageMessage currImage);
    public static OnMessageRecieved messageRecieved;

    public delegate void OnMessageStarted();
    public static OnMessageStarted messageStarted;

    public delegate void OnMessageCompleted();
    public static OnMessageCompleted messageComplete;

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
            StartCoroutine(LoadImageRoutine(bytes));
        };

        await websocket.Connect();
    }

    IEnumerator LoadImageRoutine(byte[] messageData) {
        yield return new WaitForEndOfFrame();
        string message = System.Text.Encoding.UTF8.GetString(messageData);

        if (message == "start") {

            messageStarted?.Invoke();

        } else if (message == "end") {

            messageComplete?.Invoke();

        } else {

            ImageMessage imageMessage = JsonUtility.FromJson<ImageMessage>(message);
            messageRecieved?.Invoke(imageMessage);
        }
    }

    public async void SendWebSocketMessage(string msg) {
        if (websocket.State == WebSocketState.Open) {
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
