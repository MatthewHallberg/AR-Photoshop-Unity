using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;

public class ListenUDP : MonoBehaviour {

    public delegate void OnMessageRecieved(string result);
    public static OnMessageRecieved messageRecieved;

    UdpClient receiver;
    string currMessage = string.Empty;

    public void ListenForUDP() {
        // Create UDP client
        receiver = new UdpClient(ConnectionManager.LISTEN_UDP_PORT) {
            EnableBroadcast = true
        };
        // allow reusing port
        receiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //start thread to recieve data
        receiver.BeginReceive(DataReceived, receiver);
        Debug.Log("listenging for udp on port: " + ConnectionManager.LISTEN_UDP_PORT);
    }

    void Update() {
        if (currMessage.Length > 0) {
            Debug.Log("Message recieved: " + currMessage);
            messageRecieved?.Invoke(currMessage);
            currMessage = string.Empty;
        }
    }

    void CloseConnection() {
        if (receiver != null) {
            receiver.Close();
            receiver.Dispose();
        }
    }

    void OnApplicationQuit() {
        CloseConnection();
    }

    // This is called whenever data is received
    void DataReceived(IAsyncResult ar) {

        UdpClient c = (UdpClient)ar.AsyncState;
        IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, ConnectionManager.LISTEN_UDP_PORT);
        byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);

        currMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);

        // Restart listening for udp data packages
        c.BeginReceive(DataReceived, ar.AsyncState);
    }
}
