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
