using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrisBar : MonoBehaviour
{
    public FertilizerResource cris;
    public Image fillimgae;
    private Slider slider;
    private ResourceTracker _resourceTracker;

    public void Init(ResourceTracker ResourceTracker, GameSetting gameSetting)
    {
        _resourceTracker = ResourceTracker;
        slider.value = ResourceTracker.Fertilizer;
        slider.maxValue = gameSetting.FertilizeLimit;

        _resourceTracker.ResourceValueChanged += (sender, args) =>
        {
            switch (args.NewValue)
            {
                case FertilizerResource C:
                    // Update value to UI or somewhere
                    // args.Value;
                    if (args.Type == ResourceValueChangedEventArgs.ChangeType.Increase)
                    {
                        slider.value = C.Value;
                        // do something 
                    }
                    if (args.Type == ResourceValueChangedEventArgs.ChangeType.Decrease)
                    {
                        slider.value = C.Value;
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
