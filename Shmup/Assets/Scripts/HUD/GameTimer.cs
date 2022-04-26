using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float currTimeSec = 0;
    private int currTimeMin = 0;
    private string timeString = "";
    private TextMesh timeText;

    private void Start()
    {
        timeText = GetComponentInChildren<TextMesh>();
    }

    private void Update()
    {
        if(currTimeSec >= 60f)
        {
            currTimeSec = 0;
            currTimeMin++;
        }

        currTimeSec += Time.deltaTime;

        if(currTimeSec > 10f && currTimeMin > 10)
            timeString = currTimeMin.ToString() + ":" + Mathf.Round(currTimeSec).ToString();
        else if(currTimeSec >= 10f && currTimeMin < 10)
            timeString = "0" + currTimeMin.ToString() + ":" + Mathf.Round(currTimeSec).ToString();
        else if (currTimeSec < 10f && currTimeMin >= 10)
            timeString = currTimeMin.ToString() + ":0" + Mathf.Round(currTimeSec).ToString();
        else if(currTimeSec <= 10f && currTimeMin <= 10)
            timeString = "0" + currTimeMin.ToString() + ":0" + Mathf.Round(currTimeSec).ToString();

        timeText.text = timeString;
    }

    public (int, int) GetTime()
    {
        return (Mathf.RoundToInt(currTimeSec), currTimeMin);
    }

    public void SetTime((int sec, int min) time)
    {
        currTimeSec = time.sec;
        currTimeMin = time.min;
    }
}
