using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationManager : MonoBehaviour
{
    [SerializeField] private ScreensManager screensManager;

    private string filePath;

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField phoneField;
    [SerializeField] private TMP_InputField emailField;

    [SerializeField] private Button submit;

    private void Awake()
    {
        //screensManager = FindAnyObjectByType<ScreensManager>();

        filePath = Path.Combine(Application.persistentDataPath, "users.csv");

        submit.onClick.AddListener(OnRegisterButtonClicked);

        // Create file with header if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Name,Phone,Email\n");
        }
    }

    public void OnRegisterButtonClicked()
    {
        if (nameField.text == string.Empty || phoneField.text == string.Empty || emailField.text == string.Empty)
        {
            return;
        }

        string name = nameField.text;
        string phone = phoneField.text;
        string email = emailField.text;

        SaveUser(name, phone, email);
    }

    public void SaveUser(string name, string phone, string email)
    {
        string newLine = $"{name},{phone},{email}\n";
        File.AppendAllText(filePath, newLine);

        Debug.Log("User saved to CSV: " + filePath);

        screensManager.EnableNextScreen(2);
    }
}
