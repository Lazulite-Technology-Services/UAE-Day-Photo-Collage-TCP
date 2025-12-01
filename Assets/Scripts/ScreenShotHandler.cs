using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class ScreenShotHandler : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private RawImage rawCamImage;
    [SerializeField] private RawImage resultImage;

    public WebCamTexture webCamTexture;

    [SerializeField] private RectTransform maskImage;

    private Camera myMainCamera;

    // Start is called before the first frame update
    void Start()
    {
        //Init();
        StartCoroutine(GetCameraPermission());
    }


    private void Init()
    {

        myMainCamera = Camera.main;

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
        rawCamImage.texture = webCamTexture;
        rawCamImage.material.mainTexture = webCamTexture;

        webCamTexture.Play();
        //webCamTexture.Pause();

        if (resultImage != null && resultImage.gameObject.activeSelf)
        {
            resultImage.gameObject.SetActive(false);
        }        
    }

    IEnumerator GetCameraPermission()
    {
        string[] permissions = { Permission.Camera };

#if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
        {
            UnityEngine.Android.Permission.RequestUserPermissions(permissions);

            while (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
            {
                yield return null;
            }
        }
#endif
        Init();
    }


    /*public void CropInsideMask()
    {
        if (webCamTexture == null || !webCamTexture.isPlaying)
        {
            Debug.LogWarning("Webcam not ready.");
            return;
        }

        resultImage.gameObject.SetActive(true);

        // Get RawImage and mask rects in world space
        Rect rawRect = GetRawImageRect(rawCamImage);
        Rect maskRect = GetRect(maskImage);

        // --- Clip mask inside RawImage ---
        float maskLeft = Mathf.Max(maskRect.x, rawRect.x);
        float maskRight = Mathf.Min(maskRect.x + maskRect.width, rawRect.x + rawRect.width);
        float maskBottom = Mathf.Max(maskRect.y, rawRect.y);
        float maskTop = Mathf.Min(maskRect.y + maskRect.height, rawRect.y + rawRect.height);

        float clippedWidth = maskRight - maskLeft;
        float clippedHeight = maskTop - maskBottom;

        if (clippedWidth <= 0 || clippedHeight <= 0)
        {
            Debug.LogWarning("Mask is outside the RawImage bounds.");
            return;
        }

        // --- Convert clipped rect to texture coordinates ---
        float texX = ((maskLeft - rawRect.x) / rawRect.width) * webCamTexture.width;
        float texY = ((maskBottom - rawRect.y) / rawRect.height) * webCamTexture.height;
        float texW = (clippedWidth / rawRect.width) * webCamTexture.width;
        float texH = (clippedHeight / rawRect.height) * webCamTexture.height;

        int cx = Mathf.RoundToInt(texX);
        int cy = Mathf.RoundToInt(texY);
        int cw = Mathf.RoundToInt(texW);
        int ch = Mathf.RoundToInt(texH);

        // Safety check
        cw = Mathf.Clamp(cw, 1, webCamTexture.width - cx);
        ch = Mathf.Clamp(ch, 1, webCamTexture.height - cy);

        // --- Get pixels from webcam texture ---
        Color[] pixels = webCamTexture.GetPixels(cx, cy, cw, ch);

        // Handle vertical mirroring (if front-facing webcam)
        if (webCamTexture.videoVerticallyMirrored)
        {
            Color[] flipped = new Color[pixels.Length];
            for (int y = 0; y < ch; y++)
            {
                for (int x = 0; x < cw; x++)
                {
                    flipped[y * cw + x] = pixels[(ch - y - 1) * cw + x];
                }
            }
            pixels = flipped;
        }

        // --- Create cropped texture ---
        Texture2D croppedTex = new Texture2D(cw, ch, TextureFormat.RGBA32, false);
        croppedTex.SetPixels(pixels);
        croppedTex.Apply();

        resultImage.texture = croppedTex;

        maskImage.gameObject.SetActive(false);
    }*/

    /*Rect GetRawImageRect(RawImage img)
    {
        return GetRect(img.rectTransform);
    }

    Rect GetRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
        );
    }*/

    public void TakeSnapshot()
    {
        if(!webCamTexture.isPlaying || webCamTexture.width <= 16 || webCamTexture.height <= 16)
        {
            Debug.LogWarning("Webcam not ready yet!");
            return;
        }

        //ReadPixels();

        // Wait for end of frame to capture
        StartCoroutine(CaptureMaskedArea());
    }

    private Texture2D finalImage;    

    private IEnumerator CaptureMaskedArea()
    {
        // Wait for the frame to render
        yield return new WaitForEndOfFrame();

        if (!webCamTexture.isPlaying || webCamTexture.width <= 16)
        {
            Debug.LogWarning("Webcam not ready yet!");
            yield break;
        }

        // Get the corners of the mask in screen space
        Vector3[] corners = new Vector3[4];
        maskImage.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = corners[0].y;
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        // Flip Y for ReadPixels (bottom-left origin)
        y = Screen.height - y - height;

        // Clip rectangle to screen bounds
        x = Mathf.Clamp(x, 0, Screen.width);
        y = Mathf.Clamp(y, 0, Screen.height);
        width = Mathf.Clamp(width, 0, Screen.width - x);
        height = Mathf.Clamp(height, 0, Screen.height - y);

        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("Mask area is not visible on screen.");
            yield break;
        }

        // Read pixels from screen
        Texture2D snapshot = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
        snapshot.ReadPixels(new Rect(x, y - 201, width, height), 0, 0);
        snapshot.Apply();

        maskImage.gameObject.SetActive(false);
        resultImage.gameObject.SetActive(true);

        resultImage.texture = snapshot;

        // Assign to RawImage (optional)
        if (resultImage != null)
        {
            resultImage.texture = snapshot;
            resultImage.rectTransform.sizeDelta = new Vector2(width, height);
        }

        // Save PNG
        gameManager.finalImageBytes = snapshot.EncodeToPNG();        
        
    }

    public void SaveScreenShot()
    {
        string path = Path.Combine(Application.persistentDataPath, "MaskedSnapshot.png");
        File.WriteAllBytes(path, gameManager.finalImageBytes);

        Debug.Log("Saved snapshot to: " + path);

        gameManager.SendPNG();

    }

    public void Retake()
    {
        resultImage.texture = null;
        resultImage.gameObject.SetActive(false);
        maskImage.gameObject.SetActive(true);

        webCamTexture.Play();
    }

    void OnDisable()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
