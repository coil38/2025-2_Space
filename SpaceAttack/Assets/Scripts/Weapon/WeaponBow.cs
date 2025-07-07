using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : WeaponType
{
    private float attackWidth = 0.2f;     //차지 너비
    private float maxAttackDistance = 6f; //최대 차지 거리
    private float chargeTime = 1.1f;      //최대 차지까지 소요 시간
    private float w_attackTime = 20f;     //공격 대기 시간

    // 화살 정보
    private float damage = 3f;            //공격력
    private float attackSpeed = 5f;       //피사체 속도

    public GameObject arrowPrefab;        //화살 프리팹
    private LineRenderer lineRenderer;    //공격 범위 표시
    private Timer chargeTimer;

    private LayerMask planLayer;   //바닥감지용 리이어 마스크
    private LayerMask enemyLayer;  //적감지용 레이어 마스크
    private float currentChargeDistance; //현재 차지 거리


    public override void OnEnable()
    {
        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        enemyLayer |= 1 << LayerMask.NameToLayer("Enemy");

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = attackWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        chargeTimer = new Timer(chargeTime);
        w_AttackTimer = new Timer(w_attackTime);
    }

    public override void UpdateInfo()
    {
        chargeTimer.Update();
    }

    public override void CheckAttack(Vector2 currentPos)
    {
        if (Input.GetMouseButtonDown(0)) //클릭 감지
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, currentPos);

            _currentPos = currentPos;
            currentChargeDistance = 0f;
            chargeTimer.Start();
            TimeSystem.w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

            isAttacking = true;   //공격 여부 활성화(좌우 전환용)
        }
        else if (Input.GetMouseButton(0))  //클릭 중 감지
        {

            //공격 차지 초기화
            if (TimeSystem.w_AttackTimer.GetRemainingTimer() < 2)
                TimeSystem.w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

            //공격 차지
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
            Vector2 mousePos = Vector2.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = (Vector2)hit.point;

            Vector2 attackDir = (mousePos - _currentPos).normalized * currentChargeDistance;   //플레이어 기준 마우스 방향 얻기
            Vector2 chargePos = _currentPos + attackDir;
            attackDirection = attackDir;

            lineRenderer.SetPosition(1, chargePos);

            if (currentChargeDistance >= maxAttackDistance)   //현재 차직 거리가 최대 일때
            {
                currentChargeDistance = maxAttackDistance;
                lineRenderer.SetPosition(0, _currentPos);
                lineRenderer.SetPosition(1, chargePos);
            }
            else                                              //최대 거리까지 거리 늘리기
            {
                float time = 1 - chargeTimer.GetRemainingTimer() / chargeTime;

                currentChargeDistance = time * maxAttackDistance;
            }
        }
        else if (Input.GetMouseButtonUp(0))  //클릭 놓을 시
        {
            isAttacking = false;   //공격 여부 비 활성화(좌우 전환용)

            lineRenderer.SetPosition(0, _currentPos);
            lineRenderer.SetPosition(1, _currentPos);
            lineRenderer.enabled = false;
            TimeSystem.w_AttackTimer.Reset();                 //공격(조준) 대기 취소
            Attack();
        }
    }

    public override void Attack()
    {
        float dot = Vector2.Dot(Vector2.up, attackDirection.normalized);  //내적 계산
        float degree = Mathf.Acos(dot) * Mathf.Rad2Deg;

        float dot2 = Vector2.Dot(Vector2.right, attackDirection.normalized);  //내적 계산
        float degree2 = Mathf.Acos(dot2) * Mathf.Rad2Deg;

        degree = degree2 < 90 ? - degree : degree;

        Quaternion rotate = Quaternion.Euler(0, 0, degree);

        GameObject arrow = Instantiate(arrowPrefab, _currentPos + attackDirection * 0.2f, rotate);
        WeaponArrow temp2 = arrow.GetComponent<WeaponArrow>();

        if (temp2 != null)
            temp2.Fire(_currentPos, attackDirection, attackSpeed, damage, currentChargeDistance); //이동 위치, 이동방향, 이동 속도, 공격력
        else
        {
            Debug.Log("없음 2");
        }

    }
}
