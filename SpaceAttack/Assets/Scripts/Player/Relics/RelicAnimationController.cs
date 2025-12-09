using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RelicAnimationController : MonoBehaviour
{
    private float duration = 0.8f;
    private float distance = 0.3f;
    private Ease tweenEase = Ease.InOutSine;

    private void OnEnable()
    {
        Vector3 targetPos = transform.position + transform.up * distance;
        transform.DOMove(targetPos, duration).SetEase(tweenEase).SetLoops(-1, LoopType.Yoyo);
    }
}
