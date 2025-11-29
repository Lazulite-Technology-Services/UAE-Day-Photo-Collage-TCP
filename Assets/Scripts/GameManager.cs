using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //[SerializeField] private ScreenShotHandler screenShotHandler;

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
        //captureButton.onClick.AddListener(screenShotHandler.CropInsideMask);
        cmdButton.onClick.AddListener(()=> commandPanel.SetActive(true));
        saveButton.onClick.AddListener(SaveIp);
        closeButton.onClick.AddListener(CloseCommandPanel);
    }  
    
    private void SaveIp()
    {
        PlayerPrefs.SetString("ip", ipField.text);
    }

    private void CloseCommandPanel()
    {
        commandPanel.SetActive(false);  
    }

    public void SendPNG()
    {
        TcpClient client = new TcpClient();
        client.Connect(PlayerPrefs.GetString("ip"), port);

        NetworkStream stream = client.GetStream();
        BinaryWriter writer = new BinaryWriter(stream);

        // Send length first
        writer.Write(finalImageBytes.Length);
        // Send PNG bytes
        writer.Write(finalImageBytes);

        writer.Close();
        client.Close();
    }
}
