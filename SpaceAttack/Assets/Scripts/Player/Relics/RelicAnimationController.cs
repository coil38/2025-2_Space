using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RelicAnimationController : MonoBehaviour
{
    private float duration = 0.8f;
    private float distance = 0.3f;
    private Ease tweenEase = Ease.InOutSine;

    private bool isGround;
    private Rigidbody rb;
    private LayerMask planLayer;
    private void OnEnable()
    {
        StartCoroutine(WaitUntilOnGround());
    }

    private IEnumerator WaitUntilOnGround()
    {
        rb = GetComponent<Rigidbody>();
        planLayer = 1 << LayerMask.NameToLayer("Plan");

        gameObject.layer = 0;
        yield return new WaitUntil(() => isGround);
        Vector3 targetPos = transform.position + transform.up * distance;
        transform.DOMove(targetPos, duration).SetEase(tweenEase).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        if (!isGround) CheckGround();
    }

    private void CheckGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.6f, planLayer)) //바닥감지 레이캐스트
        {
            gameObject.layer = LayerMask.NameToLayer("Item");  //아이템 레이어 할당
            isGround = true;
            Destroy(rb);      //리지드바디 삭제
        }
    }
}
