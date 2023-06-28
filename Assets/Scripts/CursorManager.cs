using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Tooltip("Custom texture for the cursor.")]
    public Texture2D cursorTexture;

    [Tooltip("Should the cursor be initially visible?")]
    public bool cursorVisible = true;

    [Tooltip("Should the cursor be confined to the game window?")]
    public bool confineCursor = true;

    // Use this for initialization
    void Start()
    {
        SetCustomCursor(cursorTexture);
        SetCursorVisibility(cursorVisible);
        SetCursorConfined(confineCursor);
    }

    // Update is called once per frame
    void Update()
    {
        // Keep the cursor state maintained every frame
        SetCursorVisibility(cursorVisible);
        SetCursorConfined(confineCursor);
    }

    public void SetCustomCursor(Texture2D texture)
    {
        // Sets the custom cursor texture with its hotspot at the center
        Cursor.SetCursor(texture, new Vector2(texture.width / 2, texture.height / 2), CursorMode.Auto);
    }

    public void SetCursorVisibility(bool visible)
    {
        // Changes the visibility of the cursor
        Cursor.visible = visible;
        // If cursor is not visible, lock it to the center of the view
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetCursorConfined(bool confine)
    {
        // Changes whether the cursor is confined to the game window
        Cursor.lockState = confine ? CursorLockMode.Confined : CursorLockMode.None;
    }
}
