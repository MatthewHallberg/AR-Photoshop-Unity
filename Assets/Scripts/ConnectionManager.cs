using System.Net;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(SendUDP))]
[RequireComponent(typeof(ListenUDP))]
[RequireComponent(typeof(ReceiveTCP))]
public class ConnectionManager : Singleton<ConnectionManager> {

    public static readonly int LISTEN_UDP_PORT = 2221;
    public static readonly int SEND_UDP_PORT = 2222;
    public static readonly int TCP_PORT = 2223;

    string Photoshop_IPAddress = string.Empty;

    SendUDP sendUDP;
    ListenUDP listenUDP;
    ReceiveTCP tcp;

    void Start() {
        sendUDP = GetComponent<SendUDP>();
        listenUDP = GetComponent<ListenUDP>();
        tcp = GetComponent<ReceiveTCP>();

        //start listening for IP address from photoshop
        listenUDP.ListenForUDP();
        sendUDP.StartSendUDP();
    }

    void OnEnable() {
        ListenUDP.messageRecieved += RegisterPhotoshopIPAddress;
    }

    void OnDisable() {
        ListenUDP.messageRecieved -= RegisterPhotoshopIPAddress;
    }

    public string GetPhotoshopIPAddress() {
        return Photoshop_IPAddress;
    }

    public void RegisterPhotoshopIPAddress(string IPAddress) {
        //save photoshop IP address
        Photoshop_IPAddress = IPAddress;
        //start tcp connection
        tcp.StartTCPConnection();
        //send our IP address
        SendLocalIPAddress();
    }

    public void OnTCPConnectionClosed() {

    }

    public void SendUDP(string message) {
        sendUDP.SendPacket(message);
    }

    void SendLocalIPAddress() {
        SendUDP(GetLocalIPAddress());
    }

    string GetLocalIPAddress() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                return ip.ToString();
            }
        }
        return string.Empty;
    }
}
