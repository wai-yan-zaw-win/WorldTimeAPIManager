using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace HarioGames.WorldTimeAPIManager
{
    /// <summary>
    /// Get server time from World Time API
    /// </summary>
    /// Developed by Wai Yan Zaw Win
    /// Email : waiyanzawwinstar8@gmail.com
    /// LinkedIn : https://www.linkedin.com/in/wai-yan-zaw-win/
    /// GitHub : https://github.com/wai-yan-zaw-win/
    public class WorldTimeAPIManager : MonoBehaviour
    {
        public static WorldTimeAPIManager Instance;

        #region Time Variables
        struct WeeklyServerTime { public DayOfWeek day; public int hours, minutes, seconds; }

        struct ServerTime { public int hours, minutes, seconds; }

        struct TimeData { public string datetime; public int day_of_week; }

        //I used http instead of https cos of worldtimeapi's certificate problem
        public string API_URL = "http://worldtimeapi.org/api/timezone/America/Tijuana"; //Pacific Day Time (Standard Timezone) 

        [HideInInspector] public bool IsTimeLoaded = false;
        [HideInInspector] int _current_day_of_week;

        ServerTime dailyServerResetTime;

        WeeklyServerTime weeklyServerResetTime;

        private DateTime _currentDateTime = DateTime.Now;
        #endregion

        #region UI
        [SerializeField] GameObject loadingUI;
        #endregion

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            GetServerTime();
        }

        /// <summary>
        /// Get server time from the provided json api
        /// </summary>
        public void GetServerTime()
        {
            StartCoroutine(GetServerTimeFromAPI());
        }

        /// <summary>
        /// Get server time from the provided json api
        /// </summary>
        /// <param name="API_URL">API URL of Server Time</param>
        public void GetServerTime(string API_URL)
        {
            this.API_URL = API_URL;

            StartCoroutine(GetServerTimeFromAPI());
        }

        /// <summary>
        /// Get server time from World Time API using UnityWebRequest
        /// </summary>
        /// <returns></returns>
        IEnumerator GetServerTimeFromAPI()
        {
            if(!loadingUI.activeInHierarchy)
            {
                loadingUI.SetActive(true);
            }

            UnityWebRequest webRequest = new UnityWebRequest();

            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey(); //To handle some certificate problems

            webRequest = UnityWebRequest.Get(API_URL);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                //error
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(GetServerTimeFromAPI());
                    yield break;
                }
                else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(GetServerTimeFromAPI());
                    yield break;
                }
            }
            else
            {
                //success
                TimeData timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);

                //timeData.datetime value is : 2020-08-14T15:54:04+01:00
                _currentDateTime = ParseDateTime(timeData.datetime);
                _current_day_of_week = timeData.day_of_week;
                IsTimeLoaded = true;
                
                if(loadingUI.activeInHierarchy)
                {
                    loadingUI.SetActive(false);
                }    

                StartCoroutine(DebugServerTime());
            }

            //datetime format => 2020-08-14T15:54:04+01:00
            DateTime ParseDateTime(string datetime)
            {
                //match 0000-00-00
                string date = Regex.Match(datetime, @"^\d{4}-\d{2}-\d{2}").Value;

                //match 00:00:00
                string time = Regex.Match(datetime, @"\d{2}:\d{2}:\d{2}").Value;

                return DateTime.Parse(string.Format("{0} {1}", date, time));
            }
        }

        /// <summary>
        /// For debugging purpose
        /// </summary>
        /// <returns></returns>
        IEnumerator DebugServerTime()
        {
            print("Current Time : " + GetCurrentDateTime());
            yield return new WaitForSeconds(1f);
            yield return DebugServerTime();
        }

        #region DateTime

        #region GET
        /// <summary>
        /// Return current DateTime from server
        /// </summary>
        /// <returns></returns>
        public DateTime GetCurrentDateTime()
        {
            return _currentDateTime.AddSeconds(Time.realtimeSinceStartup);
        }

        /// <summary>
        /// Return Daily Server Reset Time
        /// </summary>
        /// <returns></returns>
        public DateTime GetDailyServerResetTime()
        {
            return new DateTime(_currentDateTime.AddDays(1).Year, _currentDateTime.AddDays(1).Month, _currentDateTime.AddDays(1).Day, dailyServerResetTime.hours, dailyServerResetTime.minutes, dailyServerResetTime.seconds);
        }

        /// <summary>
        /// Return Weekly Server Reset Time
        /// </summary>
        /// <returns></returns>
        public DateTime GetWeeklyServerResetTime()
        {
            DateTime temp = new DateTime();
            int day = 0;

            switch (weeklyServerResetTime.day)
            {
                case DayOfWeek.Sunday: day = (7 - _current_day_of_week) + 0; break;
                case DayOfWeek.Monday: day = (7 - _current_day_of_week) + 1; break;
                case DayOfWeek.Tuesday: day = (7 - _current_day_of_week) + 2; break;
                case DayOfWeek.Wednesday: day = (7 - _current_day_of_week) + 3; break;
                case DayOfWeek.Thursday: day = (7 - _current_day_of_week) + 4; break;
                case DayOfWeek.Friday: day = (7 - _current_day_of_week) + 5; break;
                case DayOfWeek.Saturday: day = (7 - _current_day_of_week) + 6; break;
            }

            if (day > 7)
                day -= 7;

            temp = new DateTime(_currentDateTime.AddDays(day).Year, _currentDateTime.AddDays(day).Month, _currentDateTime.AddDays(day).Day, weeklyServerResetTime.hours, weeklyServerResetTime.minutes, weeklyServerResetTime.seconds);

            return temp;
        }

        /// <summary>
        /// Return DateTime of the next day of current server time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime GetNextDayDateTime(DateTime dateTime)
        {
            return dateTime.AddDays(1);
        }

        /// <summary>
        /// Return DateTime of the next week of current server time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime GetNextWeekDateTime(DateTime dateTime)
        {
            return dateTime.AddDays(7);
        }

        /// <summary>
        /// Return DateTime of the next month of current server time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime GetNextMonthDateTime(DateTime dateTime)
        {
            return dateTime.AddDays(31);
        }

        /// <summary>
        /// Return DateTime of the next year of current server time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime GetNextYearDateTime(DateTime dateTime)
        {
            return dateTime.AddDays(365);
        }
        #endregion

        #region SET
        /// <summary>
        /// Set dail server reset time
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public void SetDailyServerResetTime(int hours, int minutes, int seconds)
        {
            dailyServerResetTime.hours = hours;
            dailyServerResetTime.minutes = minutes;
            dailyServerResetTime.seconds = seconds;
        }

        /// <summary>
        /// Set weekly server reset time
        /// </summary>
        /// <param name="day"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public void SetWeeklyServerResetTime(DayOfWeek day, int hours, int minutes, int seconds)
        {
            weeklyServerResetTime.day = day;
            weeklyServerResetTime.hours = hours;
            weeklyServerResetTime.minutes = minutes;
            weeklyServerResetTime.seconds = seconds;
        }
        #endregion

        #endregion
    }
}

#region Certificate
// Based on https://www.owasp.org/index.php/Certificate_and_Public_Key_Pinning#.Net
public class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
{
    // Encoded RSAPublicKey
    private static string PUB_KEY = "30818902818100C4A06B7B52F8D17DC1CCB47362" +
        "C64AB799AAE19E245A7559E9CEEC7D8AA4DF07CB0B21FDFD763C63A313A668FE9D764E" +
        "D913C51A676788DB62AF624F422C2F112C1316922AA5D37823CD9F43D1FC54513D14B2" +
        "9E36991F08A042C42EAAEEE5FE8E2CB10167174A359CEBF6FACC2C9CA933AD403137EE" +
        "2C3F4CBED9460129C72B0203010001";

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        //X509Certificate2 certificate = new X509Certificate2(certificateData);

        //string pk = certificate.GetPublicKeyString();

        //return pk.Equals(PUB_KEY);

        return true;
    }
}
#endregion
