using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("Custom Cursor Settings")]
    [Tooltip("The custom cursor texture")]
    [SerializeField] private Texture2D cursorTexture;

    [Tooltip("The cursor hotspot")]
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private void OnEnable()
    {
        ApplyCustomCursor();
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void ApplyCustomCursor()
    {
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogError("CustomCursor: cursorTexture not assigned in the inspector!");
        }
    }
}