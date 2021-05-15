using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxSanity(int sanity)
    {
        slider.maxValue = sanity;
        slider.value = sanity;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int sanity)
    {
        slider.value = sanity;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}