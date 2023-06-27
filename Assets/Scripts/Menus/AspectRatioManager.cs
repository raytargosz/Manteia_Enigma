using UnityEngine;

public class AspectRatioManager : MonoBehaviour
{
    private const float TargetAspectRatio = 16.0f / 9.0f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        AdjustAspectRatio();
    }

    private void AdjustAspectRatio()
    {
        // Calculate the target aspect ratio
        float windowAspect = (float)Screen.width / Screen.height;

        // Determine the scale factor
        float scaleHeight = windowAspect / TargetAspectRatio;

        // Create a new Rect for the camera viewport and adjust its size
        Rect newViewportRect = mainCamera.rect;

        if (scaleHeight < 1.0f)
        {
            newViewportRect.height = scaleHeight;
            newViewportRect.width = 1.0f;
            newViewportRect.x = 0;
            newViewportRect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            newViewportRect.width = scaleWidth;
            newViewportRect.height = 1.0f;
            newViewportRect.x = (1.0f - scaleWidth) / 2.0f;
            newViewportRect.y = 0;
        }

        mainCamera.rect = newViewportRect;
    }
}