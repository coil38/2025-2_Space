using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DarkMaterialUI : MonoBehaviour
{
    private Slider m_Slider;
    public float aniDuration = 0.3f;
    public Ease aniType;

    private float currentSliderValue = 0f;
    void Start()
    {
        m_Slider = GetComponent<Slider>();
    }

    public void ChangeDarkMaterialUI(bool isAdd, float value)
    {
        if (isAdd)
        {
            if (currentSliderValue + value > 100) return;
            else
            {
                currentSliderValue = currentSliderValue + value;
            }
        }
        else
        {
            if (currentSliderValue - value < 0) return;
            else
            {
                currentSliderValue = currentSliderValue - value;
            }
        }

        m_Slider.DOValue(currentSliderValue, aniDuration).SetEase(aniType);
    }
}
