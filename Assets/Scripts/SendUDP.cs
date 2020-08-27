using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class SendUDP : MonoBehaviour {

    IPEndPoint endPoint;
    Socket sock;
    byte[] send_buffer;

    public void StartSendUDP() {
        //init socket
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {
            EnableBroadcast = true
        };
        endPoint = new IPEndPoint(IPAddress.Broadcast, ConnectionManager.SEND_UDP_PORT);
        Debug.Log("Sending UDP on port number: " + ConnectionManager.SEND_UDP_PORT);
    }

    public void CloseSocket() {
        if (sock != null) {
            sock.Close();
            sock.Dispose();
            Debug.Log("server socket closed");
        }
    }

    void OnApplicationQuit() {
        CloseSocket();
    }

    public void SendPacket(string message) {
        try {
            send_buffer = Encoding.ASCII.GetBytes(message);
            sock.SendTo(send_buffer, endPoint);
        } catch (SocketException s) {
            Debug.Log(s);
        }
    }
}
