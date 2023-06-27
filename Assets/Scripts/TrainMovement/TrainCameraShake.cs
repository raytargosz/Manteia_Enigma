using UnityEngine;
using System.Collections;

public class TrainCameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("Amplitude of the shake.")]
    [SerializeField] private float shakeAmount = 0.5f;

    [Tooltip("Frequency of the shake.")]
    [SerializeField] private float shakeFrequency = 1f;

    [Tooltip("How much the shake effect should lerp back towards the original position.")]
    [SerializeField] private float smoothFactor = 0.1f;

    [Header("Bump Settings")]
    [Tooltip("Amplitude of the bump.")]
    [SerializeField] private float bumpHeight = 0.2f;

    [Tooltip("Duration of one full bump cycle.")]
    [SerializeField] private float bumpCycleDuration = 2f;

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        // Apply Perlin noise for shake effect
        float shakeX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0.0f) - 0.5f);
        float shakeY = (Mathf.PerlinNoise(0.0f, Time.time * shakeFrequency) - 0.5f);

        Vector3 shakeOffset = new Vector3(shakeX, shakeY, 0) * shakeAmount;

        // Apply sine wave for bump effect
        float bumpOffset = Mathf.Sin(Time.time * (2 * Mathf.PI / bumpCycleDuration)) * bumpHeight;

        // Calculate new camera position
        Vector3 newPos = originalPos + shakeOffset + new Vector3(0, bumpOffset, 0);

        // Smoothly transition to this position
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, smoothFactor);
    }
}