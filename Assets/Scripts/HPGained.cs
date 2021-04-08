using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPGained : MonoBehaviour
{
    public Slider slider;

    public void SetMaxSanityLoss(int limit)
    {
        slider.maxValue = limit;
        slider.value = limit;
    }

    public void SetSanityLimit(int limit)
    {
        slider.value = limit;
    }
}
