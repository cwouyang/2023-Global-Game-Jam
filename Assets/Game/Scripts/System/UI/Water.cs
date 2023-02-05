using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    public WaterResource water;
    public Image fillimgae;
    private Slider slider;
    private ResourceTracker _resourceTracker;

    public void Init(ResourceTracker ResourceTracker, GameSetting gameSetting)
    {
        _resourceTracker = ResourceTracker;
        slider.value = ResourceTracker.Water;
        slider.maxValue = gameSetting.WaterLimit;

            _resourceTracker.ResourceValueChanged += (sender, args) =>
        {
            switch (args.NewValue)
            {
                case WaterResource w:
                    // Update value to UI or somewhere
                    // args.Value;
                    if (args.Type == ResourceValueChangedEventArgs.ChangeType.Increase)
                    {
                        slider.value = w.Value;
                        // do something 
                    }
                    if (args.Type == ResourceValueChangedEventArgs.ChangeType.Decrease)
                    {
                        slider.value = w.Value;
                        // do something 
                    }
                    break;
            }
        };
    }

    void Awake()
    {
        slider = GetComponent<Slider>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value <= slider.minValue)
        {
            fillimgae.enabled = false;
        }

        if (slider.value > slider.maxValue && !fillimgae.enabled)
        {
            fillimgae.enabled = true;
        }
        //float fillvalue = 
        //slider.value = fillvalue;
    }
}
