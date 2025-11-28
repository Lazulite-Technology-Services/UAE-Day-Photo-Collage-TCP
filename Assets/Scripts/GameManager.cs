using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ScreenShotHandler screenShotHandler;

    [SerializeField] private Button captureButton;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        captureButton.onClick.AddListener(screenShotHandler.CropInsideMask);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
