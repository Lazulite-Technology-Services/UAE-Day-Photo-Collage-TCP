using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotHandler : MonoBehaviour
{
    [SerializeField] private RawImage targetCameraImage;
    [SerializeField] private RawImage resultImage;

    private WebCamTexture webCamTexture;

    [SerializeField] private RectTransform maskImage;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        if (webCamTexture == null) return;

        // Fix rotation
        targetCameraImage.rectTransform.localEulerAngles =
            new Vector3(0, 0, -webCamTexture.videoRotationAngle);

        // Fix mirroring
        targetCameraImage.uvRect = new Rect(
            webCamTexture.videoVerticallyMirrored ? 1 : 0,
            0,
            webCamTexture.videoVerticallyMirrored ? -1 : 1,
            1);
    }

    private void Init()
    {

        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }

        // Find the first available camera
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogWarning("No camera found!");
            return;
        }

        // Use back camera if available
        string camName = devices[0].name;
        foreach (var d in devices)
        {
            if (!d.isFrontFacing)
            {
                camName = d.name;
                break;
            }
        }

        // Setup camera texture
        webCamTexture = new WebCamTexture(camName);
        targetCameraImage.texture = webCamTexture;
        targetCameraImage.material.mainTexture = webCamTexture;

        webCamTexture.Play();
    }

    public void CropInsideMask()
    {
        // 1. Get mask rect in screen coordinates
        Vector3[] corners = new Vector3[4];
        maskImage.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float width = corners[2].x - xMin;
        float height = corners[2].y - yMin;

        // 2. Convert screen rect → webcam texture rect
        float texX = (xMin / Screen.width) * webCamTexture.width;
        float texY = (yMin / Screen.height) * webCamTexture.height;
        float texW = (width / Screen.width) * webCamTexture.width;
        float texH = (height / Screen.height) * webCamTexture.height;

        // 3. Read frame from camera
        Texture2D frame = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
        frame.SetPixels(webCamTexture.GetPixels());
        frame.Apply();

        // 4. Crop region inside the mask
        Texture2D cropped = new Texture2D((int)texW, (int)texH);
        cropped.SetPixels(frame.GetPixels((int)texX, (int)texY, (int)texW, (int)texH));
        cropped.Apply();

        // 5. Output to UI
        resultImage.texture = cropped;
    }

    void OnDisable()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
