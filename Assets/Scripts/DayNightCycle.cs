using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    // Speed of the day-night cycle (in degrees per second)
    public float cycleSpeed = 10f;

    // Current time of day (0 to 24, where 0 and 24 are midnight)
    [Range(0f, 24f)]
    public float currentTime = 12f; // Start at noon

    private Light directionalLight;

    void Start()
    {
        directionalLight = GetComponent<Light>();
    }

    void Update()
    {
        // Update the time of day based on the cycle speed
        currentTime += Time.deltaTime * cycleSpeed / 360f;

        // Keep time within 24-hour range
        if (currentTime >= 24f)
        {
            currentTime -= 24f;
        }

        // Calculate rotation angle based on current time of day
        float angle = currentTime / 24f * 360f;

        // Rotate the directional light
        directionalLight.transform.rotation = Quaternion.Euler(angle, 0f, 0f);
    }
}
