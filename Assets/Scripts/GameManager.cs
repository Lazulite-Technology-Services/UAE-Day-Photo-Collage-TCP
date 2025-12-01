using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //[SerializeField] private ScreenShotHandler screenShotHandler;

    [SerializeField] private HTTPManager httpManager;

    [SerializeField] private Button cmdButton, saveButton, closeButton;

    [SerializeField] private TMP_InputField ipField;

    [SerializeField] private GameObject commandPanel;

    private int port = 5000;

    public byte[] finalImageBytes;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        TestConnection();

        //captureButton.onClick.AddListener(screenShotHandler.CropInsideMask);
        cmdButton.onClick.AddListener(OpenCommandPanel);
        saveButton.onClick.AddListener(SaveIp);
        closeButton.onClick.AddListener(CloseCommandPanel);
    }  

    private void TestConnection()
    {
        TcpClient client = new TcpClient();
        try
        {
            Debug.Log("Client port : " + PlayerPrefs.GetString("ip"));
            client.Connect(PlayerPrefs.GetString("ip"), port);
            Debug.Log("Connected to PC");
            client.Close();
        }
        catch
        {
            Debug.LogError("Failed to connect. Check IP, port, and firewall.");
        }
    }
    
    private void SaveIp()
    {
        PlayerPrefs.SetString("ip", ipField.text);
    }

    private void CloseCommandPanel()
    {
        commandPanel.SetActive(false);  
    }

    private void OpenCommandPanel()
    {
        commandPanel.SetActive(true);
        ipField.text = PlayerPrefs.GetString("ip");
    }

    public void SendPNG()
    {
        TcpClient client = new TcpClient();
        Debug.Log("Client port : " + PlayerPrefs.GetString("ip"));
        client.Connect(PlayerPrefs.GetString("ip"), port);

        NetworkStream stream = client.GetStream();
        BinaryWriter writer = new BinaryWriter(stream);

        // Send length first
        writer.Write(finalImageBytes.Length);
        // Send PNG bytes
        writer.Write(finalImageBytes);
        writer.Flush();

        writer.Close();
        client.Close();
    }

    //public void SendPNG()
    //{
    //    StartCoroutine(httpManager.SendPNG(finalImageBytes));
    //}
}
