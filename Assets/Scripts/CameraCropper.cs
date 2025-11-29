using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCropper : MonoBehaviour
{
    [Header("Camera Feed")]
    public RawImage rawImage;

    [Header("UI Mask")]
    public RectTransform maskRect; // The area to crop

    private WebCamTexture camTexture;
    public Material mat;

    void Start()
    {
        if (rawImage == null)
        {
            Debug.LogError("RawImage not assigned!");
            return;
        }

        if (maskRect == null)
        {
            Debug.LogError("Mask RectTransform not assigned!");
            return;
        }

        camTexture = new WebCamTexture();

        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        Camera.main.targetTexture = rt; // Render the camera to this texture

        mat.SetTexture("_MainTex", rt);

        camTexture.Play();

        // Create material with universal shader
        //Shader shader = Shader.Find("WebcamUniversal");
        //if (shader == null)
        //{
        //    Debug.LogError("Shader 'WebcamUniversal' not found! Make sure the shader exists and name matches exactly.");
        //    return;
        //}

        //mat = new Material(shader);
        //rawImage.material = mat;
        rawImage.texture = camTexture;
    }

    void Update()
    {
        if (camTexture == null || rawImage == null || maskRect == null || mat == null)
            return;

        if (camTexture.width < 100) return;

        UpdateCropRect();
    }

    void UpdateCropRect()
    {
        // 1. RawImage rect in world space
        Vector3[] rawCorners = new Vector3[4];
        rawImage.rectTransform.GetWorldCorners(rawCorners);
        Rect rawRect = new Rect(
            rawCorners[0].x,
            rawCorners[0].y,
            rawCorners[2].x - rawCorners[0].x,
            rawCorners[2].y - rawCorners[0].y
        );

        // 2. Mask rect in world space
        Vector3[] maskCorners = new Vector3[4];
        maskRect.GetWorldCorners(maskCorners);
        Rect mask = new Rect(
            maskCorners[0].x,
            maskCorners[0].y,
            maskCorners[2].x - maskCorners[0].x,
            maskCorners[2].y - maskCorners[0].y
        );

        // 3. Mask relative to RawImage (normalized 0-1)
        float x = (mask.x - rawRect.x) / rawRect.width;
        float y = (mask.y - rawRect.y) / rawRect.height;
        float w = mask.width / rawRect.width;
        float h = mask.height / rawRect.height;

        mat.SetVector("_CropRect", new Vector4(x, y, w, h));
    }

    void OnDestroy()
    {
        if (camTexture != null && camTexture.isPlaying)
            camTexture.Stop();
    }

}
