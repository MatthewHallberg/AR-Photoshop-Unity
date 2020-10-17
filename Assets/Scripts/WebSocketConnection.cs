using UnityEngine;
using NativeWebSocket;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

public class WebSocketConnection : MonoBehaviour {

    public delegate void OnMessageRecieved(ImageMessage currImage);
    public static OnMessageRecieved messageRecieved;

    public delegate void OnMessageStarted();
    public static OnMessageStarted messageStarted;

    public delegate void OnMessageCompleted();
    public static OnMessageCompleted messageComplete;

    WebSocket websocket;
    List<ImageMessage> currImages = new List<ImageMessage>();

    void Update() {

        if (websocket == null) {
            return;
        }

        if (currImages.Count > 0) {
            ImageMessage imageMessage = currImages[0];
            ImageMessage imageCopy = imageMessage;
            messageRecieved?.Invoke(imageCopy);
            currImages.Remove(imageMessage);
            if (numLayers == 0) {
                Message.Instance.ShowMessage("Loading images...");
                messageComplete?.Invoke();
            }
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
            ParseMessage(bytes);
        };

        await websocket.Connect();
    }

    int numLayers;
    void ParseMessage(byte[] messageData) {

        if (messageData.Length > 10) {
            numLayers--;
            Message.Instance.ShowMessage("Recieving layers...");
            //desierializing large JSON causes app to hang in coroutine so we can do another thread instead.
            Thread loadImageMessage = new Thread(LoadImageMessageThread);
            loadImageMessage.Start(messageData);
        } else {
            string message = System.Text.Encoding.UTF8.GetString(messageData);
            numLayers = int.Parse(message);
            Message.Instance.ShowMessage("Extracting " + message + " layers...");
            messageStarted?.Invoke();  
        }
    }

    void LoadImageMessageThread(object data) {
        string message = System.Text.Encoding.UTF8.GetString((byte[])data);
        ImageMessage imageMessage = JsonUtility.FromJson<ImageMessage>(message);
        currImages.Add(imageMessage);
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
