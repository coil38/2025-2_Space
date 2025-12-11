using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private HighLingthingButtonUI highLingthingButtonUI;
    [Header("버튼 유형")]
    public bool isEquipButton = false;

    private void OnEnable()
    {
        if(highLingthingButtonUI == null)
            highLingthingButtonUI = GetComponent<HighLingthingButtonUI>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerUIManager.instance.isInventorySlotButtonClick = true;
        LogUtil.Log("버튼 클릭 시작");

        if (highLingthingButtonUI == null) return;

        if (!isEquipButton)
        {
            if (!highLingthingButtonUI.isCanInteracting)
            {
                UISoundManager.PlayEquippedFailed();   //실행 안된 사운드
            }
            else
            {
                UISoundManager.PlayeButtonClickSound();   //버튼 클릭 사운드 재생
            }
        }
        else
        {

            if (!highLingthingButtonUI.isCanInteracting)
            {
                UISoundManager.PlayEquippedFailed();   //버튼 클릭 사운드 재생
            }
            else
            {
                UISoundManager.PlayEquippedSuccessfulSound();  //장착 성공 사운드 재생
                highLingthingButtonUI.isCanInteracting = false;   //장착 안됨 처리
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerUIManager.instance.isInventorySlotButtonClick = false;
        LogUtil.Log("버튼 클릭 종료");
    }
}
