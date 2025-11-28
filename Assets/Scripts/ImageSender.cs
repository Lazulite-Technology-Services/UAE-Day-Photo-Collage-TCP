using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ImageSender : MonoBehaviour
{
    public string serverIP = "192.168.1.20"; // PC IP
    public int port = 5000;
    public Texture2D imageToSend;

    public void SendImage()
    {
        byte[] imageBytes = imageToSend.EncodeToPNG(); // or EncodeToJPG()

        TcpClient client = new TcpClient(serverIP, port);
        NetworkStream stream = client.GetStream();

        // Send image length first
        byte[] lengthBytes = System.BitConverter.GetBytes(imageBytes.Length);
        stream.Write(lengthBytes, 0, lengthBytes.Length);

        // Send data
        stream.Write(imageBytes, 0, imageBytes.Length);

        stream.Close();
        client.Close();

        Debug.Log("Image sent!");
    }
}
