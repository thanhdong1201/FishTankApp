using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSaveSystem : MonoBehaviour
{
    public Data data = new Data();

    private void Start()
    {
        LoadFromJson();
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Tab))
    //    {
    //        SaveToJson();
    //    }
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        LoadFromJson();
    //    }
    //}
    public void SaveToJson()
    {
        string saveData = JsonUtility.ToJson(data);
        string filePath = Application.persistentDataPath + "/SaveData.json";
        Debug.Log(filePath);
        File.WriteAllText(filePath, saveData);
    }
    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/SaveData.json";
        string saveData = File.ReadAllText(filePath);
        data = JsonUtility.FromJson<Data>(saveData);
    }

}
[System.Serializable]
public class Data
{
    public bool isLogged;
    public string email;
    public string password;
    public List<string> feedTimer;
    public string lightTimer;
    public string lightTimerOff;

    public void SaveAccount(string emailData, string passwordData)
    {
        email = emailData;
        password = passwordData;
    }
    public void SaveFeedTimer(string timerData)
    {
        feedTimer.Add(timerData);
    }
    public void RemoveFeedTimer(int index)
    {
        feedTimer.RemoveAt(index);
    }
    public void SaveLightTimer(string timerData)
    {
        lightTimer = timerData;
    }
    public void SaveLightTimerOff(string timerData)
    {
        lightTimerOff = timerData;
    }
    public void RemoveLightTimer()
    {
        lightTimer = "";
    }
    public void RemoveLightTimerOff()
    {
        lightTimerOff = "";
    }
}