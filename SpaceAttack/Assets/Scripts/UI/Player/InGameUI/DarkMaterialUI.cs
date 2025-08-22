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

    private float TestValue = 0.4f;
    private float currentSliderValue = 1f;
    void Start()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.value = 1f;
    }

    public void ChangeDarkMaterialUI()
    {
        currentSliderValue -= TestValue;

        m_Slider.DOValue(currentSliderValue, aniDuration).SetEase(aniType);
    }
}
