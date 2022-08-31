# WorldTimeAPIManager

This package contains a simple manager to get server time for Unity. It uses [UnityWebRequest](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html) to get JSON data from (WorldTimeAPI)[http://worldtimeapi.org/] and then convert that JSON into (DateTime)[https://docs.microsoft.com/en-us/dotnet/api/system.datetime].

# How to use

It is pretty simple actually. 
Just drag and drop WorldTimeAPIManager Prefab from HarioGames/Examples/Prefabs/ .

![Prefab Image](https://i.imgur.com/xWMjFQ4.png)

You may need to call namespace as follow to call WorldTimeAPIManager.
```c#
using HarioGames.WorldTimeAPIManager; 
```

# Some Examples

An example script is provided in the package and here are some examples of getting time from WorldTimeAPI Manager and make it a simple daily and weekly reward system.

```c#
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarioGames.WorldTimeAPIManager;
using UnityEngine.UI;

public class ServerTime_ExampleScript : MonoBehaviour
{
    #region Server Time Variables
    [Header("Daily Server Reset Time")]
    [SerializeField] int dailyHours = 0;
    [SerializeField] int dailyMinutes = 0, dailySeconds = 0;

    [Header("Weekly Server Reset Time")]
    [SerializeField] DayOfWeek dayOfWeek;
    [SerializeField] int weeklyHours = 0, weeklyMinutes = 0, weeklySeconds = 0;

    WorldTimeAPIManager timeManager;
    #endregion

    #region UI
    [Space(10)]

    [SerializeField] Text weeklyTimeLeftText;

    [SerializeField] Text dailyTimeLeftText;

    [SerializeField] Button claimDailyButton, claimWeeklyButton;

    [SerializeField] GameObject debugUI;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (WorldTimeAPIManager.Instance != null)
            timeManager = WorldTimeAPIManager.Instance;

        timeManager.SetDailyServerResetTime(dailyHours, dailyMinutes, dailySeconds);
        timeManager.SetWeeklyServerResetTime(dayOfWeek, weeklyHours, weeklyMinutes, weeklySeconds);

        //UI
        debugUI.SetActive(false);

        claimDailyButton.onClick.AddListener(ResetDaily);

        claimWeeklyButton.onClick.AddListener(ResetWeekly);

        claimDailyButton.interactable = claimWeeklyButton.interactable = false;
    }

    private void Update()
    {
        if (timeManager != null)
        {
            if (timeManager.IsTimeLoaded)
            {
                debugUI.SetActive(true);

                #region Daily ServerTime Reset
                //Daily Server Reset
                DateTime oldDailyDateTime;
                if (PlayerPrefs.HasKey("DailyServerTime"))
                {
                    oldDailyDateTime = DateTime.Parse(PlayerPrefs.GetString("DailyServerTime"));
                }
                else
                {
                    PlayerPrefs.SetString("DailyServerTime", timeManager.GetDailyServerResetTime().ToString());
                    oldDailyDateTime = DateTime.Parse(PlayerPrefs.GetString("DailyServerTime"));
                }

                TimeSpan timeLeft = oldDailyDateTime - timeManager.GetCurrentDateTime();

                if (timeLeft.TotalSeconds <= 0)
                {
                    dailyTimeLeftText.text = "Claim Daily Reward!";

                    claimDailyButton.interactable = true;
                }
                else
                {
                    dailyTimeLeftText.text = timeLeft.ToString("hh\\:mm\\:ss");

                    claimDailyButton.interactable = false;
                }
                #endregion

                #region Weekly ServerTime Reset
                //Weekly Server Reset
                DateTime oldWeeklyDateTime;
                if (PlayerPrefs.HasKey("WeeklyServerTime"))
                {
                    oldWeeklyDateTime = DateTime.Parse(PlayerPrefs.GetString("WeeklyServerTime"));
                }
                else
                {
                    PlayerPrefs.SetString("WeeklyServerTime", timeManager.GetWeeklyServerResetTime().ToString());
                    oldWeeklyDateTime = DateTime.Parse(PlayerPrefs.GetString("WeeklyServerTime"));
                }

                timeLeft = oldWeeklyDateTime - timeManager.GetCurrentDateTime();

                if (timeLeft.TotalSeconds <= 0)
                {
                    weeklyTimeLeftText.text = "Claim Weekly Reward!";

                    claimWeeklyButton.interactable = true;
                }
                else
                {
                    weeklyTimeLeftText.text = timeLeft.ToString("dd\\:hh\\:mm\\:ss");

                    claimWeeklyButton.interactable = false;
                }
                #endregion
            }
            else
            {
                if (debugUI.activeInHierarchy)
                    debugUI.SetActive(false);
            }
        }
    }

    public void ResetDaily()
    {
        PlayerPrefs.SetString("DailyServerTime", timeManager.GetDailyServerResetTime().ToString());

        claimDailyButton.interactable = false;

        //can claim reward or do smth
    }

    public void ResetWeekly()
    {
        PlayerPrefs.SetString("WeeklyServerTime", timeManager.GetWeeklyServerResetTime().ToString());

        claimWeeklyButton.interactable = false;

        //can claim reward or do smth
    }
}
```

# Functions

WorldTimeAPI Manager Contains the following functions to get and calculate some simple server time :

```c# 
WorldTimeAPIManager.Instance.GetCurrentDateTime(); //will return current DateTime from server

WorldTimeAPIManager.Instance.GetNextDayDateTime(DateTime dateTime); //will return DateTime of the next day of current server time

WorldTimeAPIManager.Instance.GetNextWeekDateTime(DateTime dateTime); //will return DateTime of the next week of current server time

WorldTimeAPIManager.Instance.GetNextMonthDateTime(DateTime dateTime); // will return DateTime of the next month of current server time

WorldTimeAPIManager.Instance.GetNextYearDateTime(DateTime dateTime); //will return DateTime of the next year of current server time

//For Reward and Mission System

WorldTimeAPIManager.Instance.GetDailyServerResetTime(); //will return Daily Server Reset Time

WorldTimeAPIManager.Instance.SetDailyServerResetTime(int hours, int minutes, int seconds); //will set Daily Server Reset Time

WorldTimeAPIManager.Instance.GetWeeklyServerResetTime(); //will return Weekly Server Reset Time

WorldTimeAPIManager.Instance.SetWeeklyServerResetTime(DayOfWeek day, int hours, int minutes, int seconds); // will set Weekly Server Reset Time
```




# Contacts : 

You can contact me via waiyanzawwinstar8@gmail.com.

## Social Medias :

[<img align="left" alt="Wai Yan Zaw Win | Facebook" width="28px" src="https://img.icons8.com/ios-glyphs/30/1778f2/facebook-new.png" />][facebook]
[<img align="left" alt="Wai Yan Zaw Win | Instagram" width="28px" src="https://img.icons8.com/material-outlined/24/aaaaaa/instagram-new--v1.png" />][instagram]
[<img align="left" alt="Wai Yan Zaw Win | LinkedIn" width="28px" src="https://img.icons8.com/fluent-systems-filled/50/0077b5/linkedin.png" />][linkedin]

<br />

[facebook]: https://www.facebook.com/WaiYanZawWin.Leo/
[instagram]: https://www.instagram.com/waiyanzawwin0_0/
[linkedin]: https://www.linkedin.com/in/wai-yan-zaw-win/
