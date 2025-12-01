using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private TextMeshProUGUI countDown;

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
        //submit.onClick.AddListener(() => EnableNextScreen(2));
        homeButton.onClick.AddListener(OnHome);

        homeButton.gameObject.SetActive(false);

        EnableNextScreen(0);
    }

    private void SendImageToWall()
    {
        screenShotHandler.SaveScreenShot();
        EnableNextScreen(3);
    }

    private void RetakeImage()
    {
        captureButton.gameObject.SetActive(true);
        retakeButton.gameObject.SetActive(false);
        proceedButton.gameObject.SetActive(false);

        OnCapture();
        screenShotHandler.Retake();
    }

    private void OnCapture()
    {
        captureButton.gameObject.SetActive(false);

        StartCoroutine(CountdownTimer(3));       

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

                //screenShotHandler.Retake();

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

    private IEnumerator CountdownTimer(int duration)
    {
        int remainingTime = duration;
        countDown.gameObject.SetActive(true);

        while (remainingTime > 0)
        {
            countDown.text = remainingTime.ToString();
            Debug.Log("Time remaining: " + remainingTime + " seconds");
            yield return new WaitForSeconds(1f); // Wait 1 second
            remainingTime--;
        }

        countDown.gameObject.SetActive(false);
        screenShotHandler.TakeSnapshot();

        Debug.Log("Timer finished!");

        //countDown.gameObject.SetActive(false);
        retakeButton.gameObject.SetActive(true);
        proceedButton.gameObject.SetActive(true);
    }

    public void OnHome()
    {
        currentIndex = 0;
        EnableNextScreen(0);
    }
}
