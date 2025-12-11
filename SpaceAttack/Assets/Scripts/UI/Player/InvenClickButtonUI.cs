using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvenClickButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerUIManager.instance.isInventorySlotButtonClick = true;
        LogUtil.Log("버튼 클릭 시작");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerUIManager.instance.isInventorySlotButtonClick = false;
        LogUtil.Log("버튼 클릭 시작");
    }
}
