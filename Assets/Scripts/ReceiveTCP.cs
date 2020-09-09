using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ReceiveTCP : MonoBehaviour {

    const int PORT_NUM = 2223;
    const string IMAGE_START = "start";
    const string IMAGE_END = "done";
    const string SET_START = "new";
    const string SET_END = "end";
    const char DELIMINATOR = '@';

    public delegate void OnMessageRecieved(ImageMessage currImage);
    public static OnMessageRecieved messageRecieved;

    public delegate void OnMessageStarted();
    public static OnMessageStarted messageStarted;

    TcpListener tcpListener;
    Thread tcpListenerThread;
    TcpClient connectedTcpClient;

    List<ImageMessage> loadedImages = new List<ImageMessage>();
    bool imagesComplete = false;
    bool imagesStarted = false;
    ImageMessage currImage;
    bool wasConnected;

    public void StartTCPConnection() {
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests)) {
            IsBackground = true
        };
        tcpListenerThread.Start();
    }

    void Update() {

        if (imagesStarted) {
            messageStarted?.Invoke();
            imagesStarted = false;
        }

        if (imagesComplete) {
            foreach (ImageMessage image in loadedImages) {
                messageRecieved?.Invoke(image);
            }
            loadedImages.Clear();
            imagesComplete = false;
        }

        if (connectedTcpClient == null) {
            return;
        }

        if (wasConnected && !connectedTcpClient.Connected && tcpListener != null) {
            CloseSocket();
        }
    }

    void CloseSocket() {
        if (tcpListener != null) {
            wasConnected = false;
            tcpListenerThread.Abort();
            tcpListener.Stop();
            tcpListener = null;
        }
        if (connectedTcpClient != null) {
            connectedTcpClient.Dispose();
            connectedTcpClient = null;
        }
        Debug.Log("tcp socket closed");
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
                    wasConnected = true;
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

                                bool containsStart = message.Contains(IMAGE_START);
                                bool containsDone = message.Contains(IMAGE_END);

                                //not start or end so just write to mem stream
                                if (!containsStart && !containsDone) {
                                    memoryStream.Write(data, 0, data.Length);
                                } else {

                                    //message could contain "....pixels....,done,start,imageWidth,imageHeight,...pixels..."
                                    string[] splitMessage = message.Split(DELIMINATOR);

                                    //handle starting a new image or ending an existing one
                                    if (containsDone) {

                                        int endIndex = System.Array.IndexOf(splitMessage, IMAGE_END);
                                        //add pixels to mem stream that come before done message
                                        byte[] precedingPixels = Encoding.UTF8.GetBytes(splitMessage[endIndex - 1]);

                                        if (precedingPixels.Length > 0) {
                                            memoryStream.Write(precedingPixels, 0, precedingPixels.Length);
                                        }

                                        //copy pixels to current image
                                        currImage.pixels = memoryStream.ToArray();
                                        //add image to list of loading images
                                        loadedImages.Add(currImage);
                                        //clear the memory stream
                                        memoryStream.SetLength(0);

                                        bool containsEnd = message.Contains(SET_END);
                                        if (containsEnd) {
                                            imagesComplete = true;
                                        }
                                    }

                                    //check for start and end in same scope because both could be in same buffer
                                    if (containsStart) {

                                        int startIndex = System.Array.IndexOf(splitMessage, IMAGE_START);
                                        //add pixels to mem stream that come after start message
                                        byte[] remainingPixels = Encoding.UTF8.GetBytes(splitMessage[startIndex + 3]);


                                        if (remainingPixels.Length > 0) {
                                            memoryStream.Write(remainingPixels, 0, remainingPixels.Length);
                                        }

                                        //create new image message
                                        currImage = new ImageMessage {
                                            width = int.Parse(splitMessage[startIndex + 1]),
                                            height = int.Parse(splitMessage[startIndex + 2])
                                        };

                                        bool containsNew = message.Contains(SET_START);
                                        if (containsNew) {
                                            imagesStarted = true;
                                        }
                                    }
                                }
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