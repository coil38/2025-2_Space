using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTimeUI : MonoBehaviour
{
    [SerializeField] int skillNumber;

    private Slider skillSlider;
    void Start()
    {
        skillSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (skillNumber)
        {
            case 1:
                skillSlider.value = PlayerTimeSystem.skill1CoolTimeRate;
                break;
            case 2:
                skillSlider.value = PlayerTimeSystem.skill2CoolTimeRate;
                break;
            case 3:
                skillSlider.value = PlayerTimeSystem.skill3CoolTimeRate;
                break;
        }
    }
}
