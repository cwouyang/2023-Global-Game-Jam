using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timeer : MonoBehaviour
{
    private float timeValue = 90;
    public Text timerText;

    public void Init(ResourceTracker ResourceTracker)
    {
        timeValue = ResourceTracker.Time;
        ResourceTracker.ResourceValueChanged += (sender, args) =>
        {
            switch (args.NewValue)
            {
                case TimeResource T:
                    timerText.enabled = true;
                    timeValue = T.Value;
                    break;
            }
        };
    }


    //Update is called once per frame
    void Update()
    {
        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0;
        }

        DisplayTime(timeValue);
    }

    void DisplayTime(float timeToDisplay)
    {
        if (!timerText.enabled)
        {
            return;
        }
        
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }
}