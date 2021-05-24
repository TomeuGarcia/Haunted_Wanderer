using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public int bar;
    public int limiter;

    public Slider sliderBar;
    public Slider sliderLimiter;
    public Image fill;
    public Gradient gradient;

    public void SetMaxHealth()
    {
        sliderBar.maxValue = bar;
        sliderBar.value = bar;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth()
    {
        sliderBar.value = bar;

        fill.color = gradient.Evaluate(sliderBar.normalizedValue);
    }

    public void SetStartLimiter()
    {
        sliderLimiter.maxValue = limiter * 5;
        sliderLimiter.value = limiter;
    }

    public void SetLimiter()
    {
        sliderLimiter.value = limiter;
    }
}
