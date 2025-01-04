using System;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float timeMultiplier = 100f;  // Percepatan waktu
    [SerializeField] private float startHour = 12f;       // Jam awal
    [SerializeField] private TextMeshProUGUI timeText;    // UI untuk menampilkan waktu

    [Header("Light Settings")]
    [SerializeField] private Light sunLight;              // Cahaya matahari
    [SerializeField] private Light moonLight;             // Cahaya bulan
    [SerializeField] private float maxSunLightIntensity = 1f;
    [SerializeField] private float maxMoonLightIntensity = 0.5f;
    [SerializeField] private AnimationCurve lightChangeCurve;

    [Header("Skybox Settings")]
    [SerializeField] private Material morningSkybox;  // Skybox untuk pagi
    [SerializeField] private Material daySkybox;      // Skybox untuk siang
    [SerializeField] private Material eveningSkybox;  // Skybox untuk sore
    [SerializeField] private Material nightSkybox;    // Skybox untuk malam

    [Header("Time Periods")]
    [SerializeField] private float morningHour = 7f;   // Pagi
    [SerializeField] private float noonHour = 12f;     // Siang
    [SerializeField] private float eveningHour = 17f;  // Sore
    [SerializeField] private float nightHour = 20.5f;  // Malam

    private DateTime currentTime;

    void Start()
    {
        // Atur waktu awal
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
    }

    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        UpdateSkybox();
    }

    private void UpdateTimeOfDay()
    {
        // Perbarui waktu berdasarkan multiplier
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void RotateSun()
    {
        // Rotasi matahari berdasarkan waktu (0-360 derajat dalam sehari)
        float sunLightRotation = Mathf.Lerp(0, 360, (float)currentTime.TimeOfDay.TotalMinutes / 1440);
        sunLight.transform.rotation = Quaternion.Euler(sunLightRotation - 90, 0, 0);
    }

    private void UpdateLightSettings()
    {
        // Sesuaikan intensitas cahaya matahari dan bulan
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));

        // Update ambient light
        RenderSettings.ambientIntensity = lightChangeCurve.Evaluate(dotProduct);
    }

    private void UpdateSkybox()
    {
        float currentHour = (float)currentTime.TimeOfDay.TotalHours;

        // Logika perubahan skybox
        if (currentHour >= morningHour && currentHour < noonHour) // Pagi
        {
            RenderSettings.skybox = morningSkybox;
        }
        else if (currentHour >= noonHour && currentHour < eveningHour) // Siang
        {
            RenderSettings.skybox = daySkybox;
        }
        else if (currentHour >= eveningHour && currentHour < nightHour) // Sore
        {
            RenderSettings.skybox = eveningSkybox;
        }
        else // Malam
        {
            RenderSettings.skybox = nightSkybox;
        }

        // Perbarui lingkungan global illumination
        DynamicGI.UpdateEnvironment();
    }
}
