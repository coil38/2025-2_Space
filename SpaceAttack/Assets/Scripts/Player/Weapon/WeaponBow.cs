using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : WeaponType
{
    public GameObject arrowPrf;             //화살 프리팹

    public override void OnEnable()
    {
        chipsetCompID = 107;
        base.OnEnable();
    }

    public override void UpdateInfo() { }

    public override void CheckUse(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (Input.GetMouseButtonDown(0))  //플레이어 입력감지
        {
            if(PlayerTimeSystem.w_BaseAttackTimer != null)
                if (PlayerTimeSystem.w_BaseAttackTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            isAttacking = true;

            // 애니메이션 추가
            // 사운드 추가

            PlayerTimeSystem.w_BaseAttackTimer.Start();                                 //공격 타이머 시작

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
            Vector3 mousePos = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = hit.point;
            mousePos.y = _currentPos.y;

            attackDirection = (mousePos - _currentPos).normalized;   //플레이어 기준 마우스 방향 얻기

            Use();
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void Use()
    {
        if (arrowPrf == null) return;
        Vector3 startPos = transform.position + attackDirection * 0.3f;
        float angle = Vector3.Angle(Vector3.forward, attackDirection);
        Quaternion quaternion = arrowPrf.transform.rotation;
        if (attackDirection.x > 0)
        {
            quaternion *= Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            quaternion *= Quaternion.Euler(0f, -angle, 0f);
        }

        GameObject arrow = Instantiate(arrowPrf, startPos, quaternion);
        arrow.GetComponent<WeaponArrow>().Fire(attackDirection, 15f, damageRate, attackDistance, addedCritChanceRate, addedCritRate, chipset, ChipAttackType.Weapon);
    }
}
