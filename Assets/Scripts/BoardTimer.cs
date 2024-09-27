using System;
using System.Collections.Generic;


public class BoardTimer
{
    public int index = 0;
    public string timer;

    public BoardTimer()
    {

    }

    public BoardTimer(string timer)
    {
        this.timer = timer;
    }

    public Dictionary<string, Object> ToDictionary()
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();
        result["timer"] = timer;

        return result;
    }
}
