using UnityEngine;

[RequireComponent(typeof(ListenUDP))]
public class ConnectionManager : Singleton<ConnectionManager> {

    public static readonly int LISTEN_UDP_PORT = 2221;
    public static readonly int WEB_SOCKET_PORT = 2223;

    ListenUDP listenUDP;
    WebSocketConnection webSocket;

    void Start() {
        listenUDP = GetComponent<ListenUDP>();
        webSocket = GetComponent<WebSocketConnection>();

        //start listening for IP address from photoshop
        listenUDP.ListenForUDP();
    }

    void OnEnable() {
        ListenUDP.messageRecieved += RegisterPhotoshopIPAddress;
    }

    void OnDisable() {
        ListenUDP.messageRecieved -= RegisterPhotoshopIPAddress;
    }

    public void SendMessageToPhotoshop(string message) {
        webSocket.SendWebSocketMessage(message);
    }

    public void RegisterPhotoshopIPAddress(string photoshopIP) {
        //once connection is made we dont care about broadcast issues anymore
        MultiCastLockPixel.stopAcquiringLock = true;
        //start websocket connection
        webSocket.StartConnection(photoshopIP, WEB_SOCKET_PORT.ToString());
    }
}
