using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickButtonUI : MonoBehaviour, IPointerClickHandler
{
    private HighLingthingButtonUI highLingthingButtonUI;
    public bool isEquipButton = false;

    private void OnEnable()
    {
        if(highLingthingButtonUI == null)
            highLingthingButtonUI = GetComponent<HighLingthingButtonUI>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEquipButton)
        {
            UISoundManager.PlayeButtonClickSound();   //버튼 클릭 사운드 재생
        }
        else
        {
            if (highLingthingButtonUI == null) return;

            if (!highLingthingButtonUI.isCanEquiping)
            {
                UISoundManager.PlayEquippedFailed();   //버튼 클릭 사운드 재생
            }
            else
            {
                UISoundManager.PlayEquippedSuccessfulSound();  //장착 성공 사운드 재생
                highLingthingButtonUI.isCanEquiping = false;   //장착 안됨 처리
            }
        }
    }
}
