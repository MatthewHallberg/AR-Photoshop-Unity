using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ReceiveTCP : MonoBehaviour {

    const int PORT_NUM = 2223;

    public delegate void OnMessageRecieved(byte[] result);
    public static OnMessageRecieved messageRecieved;

    TcpListener tcpListener;
    Thread tcpListenerThread;
    TcpClient connectedTcpClient;
    bool dataWasAvailable;
    byte[] fullData;

    void Start() {
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests)) {
            IsBackground = true
        };
        tcpListenerThread.Start();
    }

    void Update() {

        if (fullData != null) {
            print(fullData.Length);
            messageRecieved?.Invoke(fullData);
            fullData = null;
        }
    }

    void CloseSocket() {
        tcpListenerThread.Abort();
        tcpListener.Stop();
        Debug.Log("tcp socket closed");
    }

    void OnApplicationQuit() {
        CloseSocket();
    }

    void ListenForIncommingRequests() {
        try {
            // Create listener on localhost port 8052. 			
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), PORT_NUM);
            tcpListener.Start();
            Debug.Log("Server is listening");
            while (true) {
                using (connectedTcpClient = tcpListener.AcceptTcpClient()) {
                    // Get a stream object for reading
                    using (NetworkStream stream = connectedTcpClient.GetStream()) {
                        //read in 1024 chunks
                        byte[] data = new byte[1024];
                        using (MemoryStream memoryStream = new MemoryStream()) {
                            int length;
                            // Read incomming stream into byte arrary.
                            while ((length = stream.Read(data, 0, data.Length)) != 0) {
                                memoryStream.Write(data, 0, length);
                                if (!stream.DataAvailable && dataWasAvailable) {
                                    fullData = memoryStream.ToArray();
                                    memoryStream.SetLength(0);
                                }
                                dataWasAvailable = stream.DataAvailable;
                            }
                        }
                    }
                }
            }
        } catch (SocketException socketException) {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }
}