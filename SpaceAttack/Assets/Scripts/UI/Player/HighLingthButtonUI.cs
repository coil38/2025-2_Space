using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HighLingthingButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector3 defualtSize;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        defualtSize = rectTransform.localScale;
    }
    void OnEnable()
    {
        if(rectTransform != null) rectTransform.localScale = defualtSize;  //크기 초기화
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetUpdate(true);
    }
}
