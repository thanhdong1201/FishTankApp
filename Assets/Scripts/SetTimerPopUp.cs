using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTimerPopUp : MonoBehaviour
{
    public TextMeshProUGUI hourText;
    public TextMeshProUGUI minText;
    public TextMeshProUGUI secText;
    public TextMeshProUGUI ampmText;

    public int hour = 0;
    public int min = 0;
    public int sec = 0;

    //Set time
    public void PlusHour()
    {
        hour += 1;
        ConfigHours();
    }
    public void MinusHour()
    {
        hour -= 1;
        ConfigHours();
    }
    private void ConfigHours()
    {
        if (hour > 12) hour = 0;
        if (hour < 0) hour = 12;
        hourText.text = hour.ToString();
    }
    public void PlusMin()
    {
        min += 1;
        ConfigMins();
    }
    public void MinusMin()
    {
        min -= 1;
        ConfigMins();
    }
    private void ConfigMins()
    {
        if (min > 59) min = 0;
        if (min < 0) min = 59;

        if (min >= 0 && min < 10)
        {
            minText.text = ("0" + min).ToString();
        }
        else
        {
            minText.text = min.ToString();
        }
    }
    public void PlusSec()
    {
        sec += 1;
        ConfigSecs();
    }
    public void MinusSec()
    {
        sec -= 1;
        ConfigSecs();
    }
    private void ConfigSecs()
    {
        if (sec > 59) sec = 0;
        if (sec < 0) sec = 59;

        if (sec >= 0 && sec < 10)
        {
            secText.text = ("0" + sec).ToString();
        }
        else
        {
            secText.text = sec.ToString();
        }
    }
    bool state = false;
    public void AMPM()
    {
        state = !state;
        if (state) ampmText.text = "AM";
        if (!state) ampmText.text = "PM";
    }
}
