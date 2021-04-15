using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public void SetMaxSanity(int sanity)
    {
        slider.maxValue = sanity;
        slider.value = sanity;
    }

    public void SetHealth(int sanity)
    {
        slider.value = sanity;
    }
}