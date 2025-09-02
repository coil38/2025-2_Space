using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HighLingthingButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector3 defualtSize;

    private bool _dontUsehighLingth;
    public bool dontUsehighLingth
    {
        get
        {
            return _dontUsehighLingth;
        }
        set
        {
            if (value)
            {
                rectTransform.localScale = defualtSize;  //크기 초기화
            }
            _dontUsehighLingth = value;  //값 할당
        }
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        defualtSize = rectTransform.localScale;

        dontUsehighLingth = false;
    }
    void OnEnable()
    {
        if(rectTransform != null) rectTransform.localScale = defualtSize;  //크기 초기화
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_dontUsehighLingth)
        {
            rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f).SetUpdate(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_dontUsehighLingth)
        {
            rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetUpdate(true);
        }
    }
}
