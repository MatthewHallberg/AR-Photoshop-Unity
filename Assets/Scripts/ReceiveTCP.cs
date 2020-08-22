using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ReceiveTCP : MonoBehaviour {

    const int PORT_NUM = 2223;

    public delegate void OnMessageRecieved(ImageMessage currImage);
    public static OnMessageRecieved messageRecieved;

    TcpListener tcpListener;
    Thread tcpListenerThread;
    TcpClient connectedTcpClient;

    ImageMessage currImage;
    bool imageLoaded;

    public void StartTCPConnection() {
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests)) {
            IsBackground = true
        };
        tcpListenerThread.Start();
    }

    void Update() {

        if (imageLoaded) {
            imageLoaded = false;
            messageRecieved?.Invoke(currImage);
        }
    }

    void CloseSocket() {
        if (tcpListener != null) {
            tcpListenerThread.Abort();
            tcpListener.Stop();
            Debug.Log("tcp socket closed");
        }
    }

    void OnApplicationQuit() {
        CloseSocket();
    }

    void ListenForIncommingRequests() {
        try {
            string photoshopIP = ConnectionManager.Instance.GetPhotoshopIPAddress();
            tcpListener = new TcpListener(IPAddress.Parse(photoshopIP), PORT_NUM);
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

                                //convert bytes to string so we can check them for start and end
                                //this is probably a terrible idea but I dont want to spend anymore time on this
                                string message = Encoding.UTF8.GetString(data, 0, data.Length);

                                if (message.Contains("start")) {
                                    string[] splitMessage = message.Split(',');
                                    //start of message will contain "imageWidth,imageHeight,...pixels..."
                                    currImage = new ImageMessage {
                                        width = int.Parse(splitMessage[1]),
                                        height = int.Parse(splitMessage[2])
                                    };
                                } else if (message.Contains("done")) {
                                    //end of message will contain "...pixels...,done,...."
                                    string[] splitMessage = message.Split(',');
                                    byte[] lastPixels = Encoding.UTF8.GetBytes(splitMessage[0]);
                                    if (lastPixels.Length > 0) {
                                       memoryStream.Write(lastPixels, 0, lastPixels.Length);
                                    }
                                    currImage.pixels = memoryStream.ToArray();
                                    memoryStream.SetLength(0);
                                    imageLoaded = true;
                                } else {
                                    //not start or end so just write to mem stream
                                   memoryStream.Write(data, 0, data.Length);
                                }
                            }
                        }
                    }
                    Debug.Log("Connection Closed");
                }
            }
        } catch (SocketException socketException) {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }
}