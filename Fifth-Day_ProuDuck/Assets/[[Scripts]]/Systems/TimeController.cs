using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float timeMultiplier;

    [SerializeField]
    private float startHour;

    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private Light sunLight;

    [SerializeField]
    private float sunriseHour;

    [SerializeField]
    private float sunsetHour;

    [SerializeField]
    private Color dayAmbientLight;

    [SerializeField]
    private Color nightAmbientLight;

    [SerializeField]
    private AnimationCurve lightChangeCurve;

    [SerializeField]
    private float maxSunLightIntensity;

    [SerializeField]
    private Light moonLight;

    [SerializeField]
    private float maxMoonLightIntensity;

    private DateTime currentTime;

    private DateTime dayResetTime;

    private TimeSpan sunriseTime;

    private TimeSpan sunsetTime;

    private bool checkDateTimeSame;

    [HideInInspector]
    public int daysPassed;

    [HideInInspector]
    public int dayNight;

    public int currentDayNightCondition;

    [HideInInspector]
    public float sunLightRotation;


    private QuestManager _questManager;
    private TerrainGenerator _terrainGenerator;


    // Start is called before the first frame update
    void Start()
    {
        _questManager = FindObjectOfType<QuestManager>();
        _terrainGenerator = FindObjectOfType<TerrainGenerator>();
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        dayResetTime = currentTime;
        daysPassed = 1;

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseDays();
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        UpdateMonsterMode();
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }

    }

    private void RotateSun()
    {
        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);

            dayNight = 1;
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);

            dayNight = 2;
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }

    private void IncreaseDays()
    {
        checkDateTimeSame = Equals(currentTime.Date, dayResetTime.Date);

        if (checkDateTimeSame)
        {
            
        }
        else
        {
            daysPassed++;
            dayResetTime = currentTime;
            if (daysPassed > _questManager.currentDayQuests)
            {
                _terrainGenerator.ClearTerrainChunks();;
                SceneManager.LoadScene("GameOverLoseScene 1");
                
            }
        }
    }

    private void UpdateMonsterMode()
    {
        if(currentDayNightCondition != dayNight)
        {
            currentDayNightCondition = dayNight;

            if (dayNight == 1)
            {
                FindObjectOfType<MonsterManager>().SwitchMonstersMode(MonsterMode.DayMode);
            }
            if (dayNight == 2)
            {
                FindObjectOfType<MonsterManager>().SwitchMonstersMode(MonsterMode.NightMode);
            }
        }
     
    }
}
