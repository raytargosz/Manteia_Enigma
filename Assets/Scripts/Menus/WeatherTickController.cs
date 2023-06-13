using UnityEngine;
using DistantLands.Cozy;

public class WeatherTickController : MonoBehaviour
{
    public CozyWeather weatherController;
    public float minTickValue = 0;
    public float maxTickValue = 100;
    public float tickSpeed = 1;

    private bool increasing = true;

    void Update()
    {
        if (weatherController != null)
        {
            // Update current ticks based on the increasing/decreasing state
            weatherController.currentTicks += (increasing ? tickSpeed : -tickSpeed) * Time.deltaTime;

            // Check if ticks reached the min or max value, and change the direction
            if (weatherController.currentTicks >= maxTickValue)
            {
                increasing = false;
            }
            else if (weatherController.currentTicks <= minTickValue)
            {
                increasing = true;
            }
        }
    }
}
