using UnityEngine;

public class BobbingParent : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [Tooltip("Speed of the bobbing.")]
    public float bobbingSpeed = 1f;
    [Tooltip("Height range of the bobbing.")]
    public float bobbingHeight = 0.5f;

    private Vector3[] originalPositions;

    void Start()
    {
        originalPositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            originalPositions[i] = transform.GetChild(i).position;
        }
    }

    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 position = child.position;
            float newY = originalPositions[i].y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            child.position = new Vector3(position.x, newY, position.z);
        }
    }
}