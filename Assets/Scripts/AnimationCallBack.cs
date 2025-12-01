using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallBack : MonoBehaviour
{
    private ScreensManager screensManager;

    private void Awake()
    {
        screensManager = FindAnyObjectByType<ScreensManager>();
    }

    public void OnAnimationComplete()
    {
        screensManager.OnHome();
    }
}
