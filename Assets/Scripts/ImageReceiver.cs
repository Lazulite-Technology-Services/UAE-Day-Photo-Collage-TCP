using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

public class ImageReceiver : MonoBehaviour
{
    public int port = 5000;

    void Start()
    {
        StartListener();
    }

    async void StartListener()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Debug.Log("Waiting for connection...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            Debug.Log("Client connected!");

            NetworkStream stream = client.GetStream();

            // Read length first
            byte[] lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);
            int dataLength = System.BitConverter.ToInt32(lengthBytes, 0);

            // Read image
            byte[] imageData = new byte[dataLength];
            stream.Read(imageData, 0, dataLength);

            // Save file
            string filePath = Application.dataPath + "/receivedImage.png";
            File.WriteAllBytes(filePath, imageData);

            Debug.Log("Saved to: " + filePath);

            stream.Close();
            client.Close();
        }
    }
}
