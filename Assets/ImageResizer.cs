using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageResizer : MonoBehaviour
{
    public Texture2D ResizeImage(Texture2D sourceTexture, int targetWidth, int targetHeight)
    {
        // Calculate the scaling factor based on the smaller dimension
        float scaleFactor = Mathf.Min((float)targetWidth / sourceTexture.width, (float)targetHeight / sourceTexture.height);

        // Calculate the new width and height while maintaining the aspect ratio
        int newWidth = Mathf.RoundToInt(sourceTexture.width * scaleFactor);
        int newHeight = Mathf.RoundToInt(sourceTexture.height * scaleFactor);

        // Create a RenderTexture with the target dimensions
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;

        // Set the RenderTexture as the target for rendering
        RenderTexture.active = rt;

        // Draw the source texture to the RenderTexture
        Graphics.Blit(sourceTexture, rt);

        // Create a new Texture2D with the desired size
        Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);

        // Copy the RenderTexture to the new Texture2D
        resizedTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resizedTexture.Apply();

        // Release the RenderTexture
        RenderTexture.ReleaseTemporary(rt);

        return resizedTexture;
    }
}
