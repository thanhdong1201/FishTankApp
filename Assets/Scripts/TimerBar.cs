using UnityEngine;
using TMPro;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI saveTimeText;
    public string saveTime = "0";
    public bool feedTimer = true;

    private JsonSaveSystem jsonSaveSystem;

    private void Awake()
    {
        jsonSaveSystem = FindObjectOfType<JsonSaveSystem>();
    }

    public void DeleteTimer()
    {
        if (feedTimer)
        {
            for (int i = 0; i < jsonSaveSystem.data.feedTimer.Count; i++)
            {
                if (saveTime == jsonSaveSystem.data.feedTimer[i])
                {
                    FindObjectOfType<JsonSaveSystem>().data.RemoveFeedTimer(i);
                    FindObjectOfType<FirebaseManager>().RemoveSetFeedTimer();
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (saveTime == jsonSaveSystem.data.lightTimer)
            {
                FindObjectOfType<JsonSaveSystem>().data.RemoveLightTimer();
                FindObjectOfType<FirebaseManager>().RemoveSetLightTimer();
                Destroy(gameObject);
            }
        }
    }
    public void Save(string data)
    {
        saveTime = data;
        saveTimeText.text = data;
    }
}
