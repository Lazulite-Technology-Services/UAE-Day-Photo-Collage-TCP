using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreensManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ScreenShotHandler screenShotHandler;

    [SerializeField] private Button register, submit;

    [SerializeField] private Button proceedButton;
    [SerializeField] private Button retakeButton;
    [SerializeField] private Button captureButton;
    [SerializeField] private Button homeButton;

    [SerializeField] private GameObject[] screens;

    [SerializeField] private int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        proceedButton.onClick.AddListener(SendImageToWall);
        captureButton.onClick.AddListener(OnCapture);
        retakeButton.onClick.AddListener(RetakeImage);
        register.onClick.AddListener(() => EnableNextScreen(1));
        submit.onClick.AddListener(() => EnableNextScreen(2));
        homeButton.onClick.AddListener(OnHome);

        homeButton.gameObject.SetActive(false);

        EnableNextScreen(0);
    }

    private void SendImageToWall()
    {
        screenShotHandler.SaveScreenShot();
    }

    private void RetakeImage()
    {
        captureButton.gameObject.SetActive(true);
        retakeButton.gameObject.SetActive(false);
        proceedButton.gameObject.SetActive(false);

        screenShotHandler.Retake();
    }

    private void OnCapture()
    {
        captureButton.gameObject.SetActive(false);
        retakeButton.gameObject.SetActive(true);
        proceedButton.gameObject.SetActive(true);
    }

    private void ResetUI()
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(false);
        }

        if (currentIndex == 0)
        {
            homeButton.gameObject.SetActive(false);
        }
        else
        {
            homeButton.gameObject.SetActive(true);
        }

        if (currentIndex == 2)
        {
            captureButton.gameObject.SetActive(false);
            proceedButton.gameObject.SetActive(false);
        }
    }

    public void EnableNextScreen(int index)
    {
        currentIndex = index;
        ResetUI();

        switch (index)
        {
            case 0: //Home screen
                screens[0].SetActive(true);
                break;
            case 1: //Register page
                screens[1].SetActive(true);
                break;
            case 2: //Screen capture page
                screens[2].SetActive(true);

                screenShotHandler.Retake();

                captureButton.gameObject.SetActive(true);
                retakeButton.gameObject.SetActive(false);
                proceedButton.gameObject.SetActive(false);
                break;
            case 3://Thank you page
                screens[3].SetActive(true);
                break;
            default:
                break;
        }
    }

    private void ShowLoading()
    {

    }

    private void StartCountDown()
    {

    }

    private void OnHome()
    {
        currentIndex = 0;
        EnableNextScreen(0);
    }
}
