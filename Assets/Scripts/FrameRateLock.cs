using UnityEngine;

public class FrameRateLock : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 59;
    }
}