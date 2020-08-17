using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class SendMessage : MonoBehaviour {

    const int PORT_NUM = 2222;

    IPEndPoint endPoint;
    Socket sock;
    byte[] send_buffer;

    void Start() {
        //init socket
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {
            EnableBroadcast = true
        };
        endPoint = new IPEndPoint(IPAddress.Broadcast, PORT_NUM);
        Debug.Log("server listening on port number: " + PORT_NUM);
    }

    void CloseSocket() {
        sock.Close();
        sock.Dispose();
        Debug.Log("server socket closed");
    }

    void OnApplicationQuit() {
        CloseSocket();
    }

    public void SendPacket(string message) {
        try {
            send_buffer = Encoding.ASCII.GetBytes(message);
            sock.SendTo(send_buffer, endPoint);
            Debug.Log(message);
        } catch (SocketException s) {
            Debug.Log(s);
        }
    }
}