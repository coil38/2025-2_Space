using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class HighLingthingButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool isPauseHightLight = false;
    [SerializeField] private RectTransform pauseHightLightTrans;

    private RectTransform rectTransform;
    private Vector3 defualtSize;

    private bool _dontUsehighLingth;
    private bool _isCanInteracting = true;

    public bool isCanInteracting
    {
        get { return _isCanInteracting; }
        set
        {
            if (!value)
            {
                rectTransform.localScale = defualtSize;  //크기 초기화
                _dontUsehighLingth = true;  //크기 초기화
            }
            else _dontUsehighLingth = false;  //하이라이트 가능할 때, 다시 장착가능 설정
            _isCanInteracting = value;  //값 할당
        }
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        defualtSize = rectTransform.localScale;

        _dontUsehighLingth = false;
    }
    void OnEnable()
    {
        if(rectTransform != null) rectTransform.localScale = defualtSize;  //크기 초기화
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_dontUsehighLingth)
        {
            if (isPauseHightLight && pauseHightLightTrans != null)
            {
                pauseHightLightTrans.gameObject.SetActive(true);
                pauseHightLightTrans.position = transform.position;
            }
            else
            {
                rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f).SetUpdate(true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_dontUsehighLingth)
        {
            if (isPauseHightLight && pauseHightLightTrans != null)
                pauseHightLightTrans.gameObject.SetActive(false);
            else
            {
                rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetUpdate(true);
            }
        }
    }
}
