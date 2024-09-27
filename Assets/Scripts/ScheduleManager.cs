using System;
using TMPro;
using UnityEngine;

public class ScheduleManager : MonoBehaviour
{
    public GameObject timeBarPrefab;
    public GameObject feedTimeBarHolder;
    public GameObject lightTimeBarHolder;
    public GameObject lightTimeOffBarHolder;
    private TimerBar[] timerBar;
    private TimerBar timerBar2, timerBar3;

    [Header("TimeText")]
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI timeText;


    public SetTimerPopUp feedPopUp;
    public SetTimerPopUp lightPopUp;
    public SetTimerPopUp lightOffPopUp;

    private FirebaseManager firebaseManager;
    private JsonSaveSystem jsonSaveSystem;

    private string saveTime;
    private DateTime currentDateTime;

    private void Awake()
    {
        firebaseManager = GetComponent<FirebaseManager>();
        jsonSaveSystem = GetComponent<JsonSaveSystem>();
    }

    private void Update()
    {
        if (WorldTimeAPI.Instance.IsTimeLodaed)
        {
            currentDateTime = WorldTimeAPI.Instance.GetCurrentDateTime();
            dateText.text = currentDateTime.ToLongDateString();
            timeText.text = currentDateTime.ToLongTimeString();

            if (firebaseManager.modeToggle.isOn)
            {
                UpdateData();
            }
        }
    }
    private void UpdateData()
    {
        timerBar = feedTimeBarHolder.transform.GetComponentsInChildren<TimerBar>(true);
        if(timerBar.Length > 0)
        {
            for (int i = 0; i < timerBar.Length; i++)
            {
                firebaseManager.UpdateFeedTimeBar(timerBar[i].saveTime, i);
            }
        }

        timerBar2 = lightTimeBarHolder.GetComponentInChildren<TimerBar>();
        if (timerBar2 != null)
        {
            firebaseManager.UpdateLightTimeBar(timerBar2.saveTime);
        }
        timerBar3 = lightTimeOffBarHolder.GetComponentInChildren<TimerBar>();
        if (timerBar3 != null)
        {
            firebaseManager.UpdateLightOffTimeBar(timerBar3.saveTime);
        }      
    }
    //Reload timeBar
    public void ReloadTimeBar()
    {
        for (int i = 0; i < jsonSaveSystem.data.feedTimer.Count; i++)
        {
            GameObject g = Instantiate(timeBarPrefab, feedTimeBarHolder.transform);
            g.GetComponent<TimerBar>().Save(jsonSaveSystem.data.feedTimer[i]);
            g.GetComponent<TimerBar>().feedTimer = true;
        }

        GameObject g1 = lightTimeBarHolder.GetComponentInChildren<TimerBar>().gameObject;
        g1.GetComponent<TimerBar>().Save(jsonSaveSystem.data.lightTimer);
        g1.GetComponent<TimerBar>().feedTimer = false;

        GameObject g2 = lightTimeOffBarHolder.GetComponentInChildren<TimerBar>().gameObject;
        g2.GetComponent<TimerBar>().Save(jsonSaveSystem.data.lightTimerOff);
        g2.GetComponent<TimerBar>().feedTimer = false;
    }
    //Add new timeBar
    public void CreateNewFeedTimeBars()
    {
        if (timerBar.Length < 5)
        {
            saveTime = feedPopUp.hour + ":" + feedPopUp.minText.text + " " + feedPopUp.ampmText.text;
            GameObject g = Instantiate(timeBarPrefab, feedTimeBarHolder.transform);
            g.GetComponent<TimerBar>().Save(saveTime);
            g.GetComponent<TimerBar>().feedTimer = true;
            jsonSaveSystem.data.feedTimer.Add(saveTime);
        }
    }
    public void SaveLightOnTimeBars()
    {
        saveTime = lightPopUp.hour + ":" + lightPopUp.minText.text + " " + lightPopUp.ampmText.text;
        GameObject g = lightTimeBarHolder.GetComponentInChildren<TimerBar>().gameObject;
        g.GetComponent<TimerBar>().Save(saveTime);
        g.GetComponent<TimerBar>().feedTimer = false;
        jsonSaveSystem.data.lightTimer = saveTime;
    }
    public void SaveLightOffTimeBars()
    {
        saveTime = lightOffPopUp.hour + ":" + lightOffPopUp.minText.text + " " + lightOffPopUp.ampmText.text;
        GameObject g = lightTimeOffBarHolder.GetComponentInChildren<TimerBar>().gameObject;
        g.GetComponent<TimerBar>().Save(saveTime);
        g.GetComponent<TimerBar>().feedTimer = false;
        jsonSaveSystem.data.lightTimerOff = saveTime;
    }
}
