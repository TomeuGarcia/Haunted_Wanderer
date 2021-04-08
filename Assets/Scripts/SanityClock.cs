using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanityClock : MonoBehaviour
{
    public Slider slider;

    public void SetMaxSanity2(int sanity2)
    {
        slider.maxValue = sanity2;
        slider.value = sanity2;
    }

    public void SetSanity2(int sanity2)
    {
        slider.value = sanity2;
    }
}
