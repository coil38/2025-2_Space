using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAttackRange : MonoBehaviour
{
    [SerializeField] private GameObject spriteObject;

    private Timer lifeTimer;

    private void Start()
    {
        //spriteObject.SetActive(false);
    }

    private void Update()
    {
        if (lifeTimer == null) return;

        lifeTimer.Update();
        if (lifeTimer.IsEndTimer())
        {
            spriteObject.SetActive(false);
        }
    }

    public void OnAttackRange(Vector3 genPos, float distance, float width, Vector3 dir, float lifeTime)
    {
        if (spriteObject == null)
        {
            LogUtil.Log("공격범위의 변경될 스프라이트 대상이 할당되지 않음");
            return;
        }

        spriteObject.transform.localScale = new Vector3(width, distance, 0.1f);     //크기 조정
        spriteObject.transform.localPosition = new Vector3(0f, 0.5f, distance / 2f);  //위치 조정
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);              //방향 조정
        transform.position = new Vector3(genPos.x, transform.position.y, genPos.z); //위치 조정

        if (lifeTimer == null) lifeTimer = new Timer(lifeTime);
        else lifeTimer.ChangeDuration(lifeTime);

        if (lifeTimer.IsRunning()) return;

        lifeTimer.Start();
        spriteObject.SetActive(true);
    }
}
