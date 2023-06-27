using UnityEngine;

public class LanternSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [Tooltip("The amplitude of the sway.")]
    [SerializeField] private float swayAmount = 0.1f;

    [Tooltip("The frequency of the sway.")]
    [SerializeField] private float swayFrequency = 0.5f;

    [Tooltip("How much the sway should lerp back towards the original rotation.")]
    [SerializeField] private float smoothFactor = 0.1f;

    [Tooltip("The pivot point that the lantern will sway around.")]
    [SerializeField] private Transform pivotPoint;

    [Header("Audio Settings")]
    [Tooltip("Audio source to play sound effects")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Sound effects to play when lantern sways")]
    [SerializeField] private AudioClip[] swaySounds;

    [Tooltip("Minimum and maximum pitch when playing sound effects")]
    [SerializeField] private Vector2 pitchRange = new Vector2(0.8f, 1.2f);

    [Tooltip("Minimum delay between sound effects")]
    [SerializeField] private float soundDelay = 1f;

    private float lastSoundTime;
    private Quaternion originalRot;
    private Vector3 randomOffset;

    void Start()
    {
        originalRot = transform.localRotation;
        randomOffset = new Vector3(Random.value, Random.value, Random.value);
        lastSoundTime = -soundDelay;
    }

    void Update()
    {
        // Apply Perlin noise for sway effect
        float swayX = (Mathf.PerlinNoise(randomOffset.x + Time.time * swayFrequency, 0.0f) - 0.5f) * 2;
        float swayY = (Mathf.PerlinNoise(0.0f, randomOffset.y + Time.time * swayFrequency) - 0.5f) * 2;
        float swayZ = (Mathf.PerlinNoise(randomOffset.z + Time.time * swayFrequency, randomOffset.z) - 0.5f) * 2;

        Quaternion swayRot = Quaternion.Euler(new Vector3(swayX * swayAmount, swayY * swayAmount, swayZ * swayAmount));

        // Calculate new lantern rotation
        Quaternion newRot = originalRot * swayRot;

        // Smoothly transition to this rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRot, smoothFactor);

        PlaySoundEffect();
    }

    private void PlaySoundEffect()
    {
        if (Time.time >= lastSoundTime + soundDelay)
        {
            lastSoundTime = Time.time;
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.PlayOneShot(swaySounds[Random.Range(0, swaySounds.Length)]);
        }
    }
}
