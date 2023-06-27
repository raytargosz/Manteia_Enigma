using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light lightSource;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float minFlickerSpeed = 0.05f;
    public float maxFlickerSpeed = 0.15f;

    private float targetIntensity;
    private float flickerSpeed;

    public bool IsFlickering { get; set; } = true;

    private void Start()
    {
        if (lightSource == null)
        {
            lightSource = GetComponent<Light>();
        }
        flickerSpeed = Random.Range(minFlickerSpeed, maxFlickerSpeed);
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    private void Update()
    {
        if (!IsFlickering) return;

        lightSource.intensity = Mathf.Lerp(lightSource.intensity, targetIntensity, flickerSpeed * Time.deltaTime);

        if (Mathf.Abs(lightSource.intensity - targetIntensity) < 0.1f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            flickerSpeed = Random.Range(minFlickerSpeed, maxFlickerSpeed);
        }
    }
}