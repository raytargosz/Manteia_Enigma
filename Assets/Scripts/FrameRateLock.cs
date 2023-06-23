using UnityEngine;

public class FrameRateLock : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 1; 
        Application.targetFrameRate = 60;
    }
}