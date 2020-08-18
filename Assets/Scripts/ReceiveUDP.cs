using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;

public class ReceiveUDP : MonoBehaviour {

    const int PORT_NUM = 2223;

    public delegate void OnMessageRecieved(byte[] result);
    public static OnMessageRecieved messageRecieved;

    UdpClient receiver;
    byte[] receivedBytes;

    void Start() {
        receiver = new UdpClient(PORT_NUM);
        Debug.Log("client listening on port number: " + PORT_NUM);
        receiver.BeginReceive(DataReceived, receiver);
    }

    void Update() {
        if (receivedBytes != null) {
            Debug.Log("new UDP packet");
            messageRecieved?.Invoke(receivedBytes);
            receivedBytes = null;
        }
    }

    void CloseSocket() {
        receiver.Close();
        receiver.Dispose();
        Debug.Log("client socket closed");
    }

    void OnApplicationQuit() {
        CloseSocket();
    }

    void DataReceived(IAsyncResult ar) {

        UdpClient c = (UdpClient)ar.AsyncState;
        IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);

        // Restart listening for udp data packages
        c.BeginReceive(DataReceived, ar.AsyncState);
    }
}