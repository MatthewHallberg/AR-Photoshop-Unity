using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;

public class RecieveMessage : MonoBehaviour {

    const int PORT_NUM = 2222;

    public delegate void OnMessageRecieved(string result);
    public static OnMessageRecieved messageRecieved;

    UdpClient receiver;
    string currMessage = String.Empty;

    void Start() {
        // Create UDP client
        receiver = new UdpClient(PORT_NUM);
        Debug.Log("listening on port number: " + PORT_NUM);
        receiver.BeginReceive(DataReceived, receiver);
    }

    void Update() {
        if (currMessage.Length > 0) {
            Debug.Log("Message recieved: " + currMessage);
            messageRecieved?.Invoke(currMessage);
            currMessage = string.Empty;
        }
    }

    void OnApplicationQuit() {
        receiver.Close();
        receiver.Dispose();
    }

    // This is called whenever data is received
    void DataReceived(IAsyncResult ar) {

        UdpClient c = (UdpClient)ar.AsyncState;
        IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);

        currMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);

        // Restart listening for udp data packages
        c.BeginReceive(DataReceived, ar.AsyncState);
    }
}