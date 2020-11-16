using UnityEngine;
using NativeWebSocket;
using System.Threading;
using System.Collections.Generic;

public class WebSocketConnection : MonoBehaviour {

    public delegate void OnMessageRecieved(ImageMessage currImage);
    public static OnMessageRecieved messageRecieved;

    public delegate void OnMessageStarted(DocumentInfo DocInfo);
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
                messageComplete?.Invoke();
            }
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    public async void StartConnection(string photoshopIP, string port) {
        Dictionary<string, string> headers = new Dictionary<string, string> {
            { "User-Agent", "Unity3D" }
        };
        websocket = new WebSocket("ws://" + photoshopIP + ":" + port, headers);

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

        //HACK: this is probably not a safe check (maybe decode as string everytime??? idfk its late)
        if (messageData.Length > 75) {
        //TODO: Fix this 

            numLayers--;
            Message.Instance.ShowMessage("Loading layers...");
            //desierializing large JSON causes app to hang in coroutine so we can do another thread instead.
            Thread loadImageMessage = new Thread(LoadImageMessageThread);
            loadImageMessage.Start(messageData);
        } else {
            string message = System.Text.Encoding.UTF8.GetString(messageData);
            DocumentInfo docInfo = JsonUtility.FromJson<DocumentInfo>(message);
            numLayers = docInfo.layers;
            Message.Instance.ShowMessage("Extracting " + numLayers + " layers...");
            messageStarted?.Invoke(docInfo);
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
