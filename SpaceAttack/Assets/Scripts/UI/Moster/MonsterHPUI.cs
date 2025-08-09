using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MonsterHPUI : MonoBehaviour
{
    [SerializeField] private Slider Hpslider;
    [SerializeField] private GameObject fillArea;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease animationType;

    void OnEnable()
    {
        Hpslider.value = 1f;
    }

    public void ReduceHP(float maxHp, float hp)  //몬스터 체력감소
    {
        if (hp <= 0f)
        {
            Hpslider.DOValue(0, 0.1f).SetEase(animationType).OnComplete(() => 
            {
                fillArea.SetActive(false);
                gameObject.SetActive(false);
            });
        }
        else Hpslider.DOValue(hp / maxHp, animationDuration).SetEase(animationType);
    }
}
