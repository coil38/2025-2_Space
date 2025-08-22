using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HighLingthingButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
    }
}
