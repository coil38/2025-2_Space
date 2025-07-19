using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BowSkillConcentratedFIre : SkillType
{

    private LayerMask planLayer;   //바닥감지용 리이어 마스크
    private LayerMask wallLayer;   //벽감지용 레이어 마스크
    private LayerMask enemyLayer;  //적감지용 레이어 마스크

    private WaitForFixedUpdate waitForFixedUpdate;
    private WaitForSeconds waitForSeconds;

    private float detectDistance;
    private float attackCycle;

    private bool isPlayGizoms;
    private bool cannotAttacking;

    private float _detectDistance;

    private Vector3 targetPos;
    private Vector3 currentAttackPos;

    public override void OnEnable()
    {
        detectDistance = 4f;    //선택범위 반지름
        attackDistance = 2f;    //공격범위 반지름
        attackCycle = 0.5f;     //공격주기
        damage = 0.5f;          //한 주기동안의 공격력
        attackTime = 2.5f;      //공격 시간
        r_AttackTime = 0.2f;  //플레이어 대기 시간
        coolTime = 12f;         //쿨타임

        w_AttackTimer = new Timer(r_AttackTime);
        coolTimer = new Timer(coolTime);

        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        wallLayer |= 1 << LayerMask.NameToLayer("Wall");
        enemyLayer |= (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("DestructableObject"));

        waitForFixedUpdate = new WaitForFixedUpdate();
        waitForSeconds = new WaitForSeconds(attackCycle);
    }

    public override void UpdateInfo()
    {
        coolTimer.Update();
    }

    public override void CheckAttack(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (coolTimer.IsRunning() || TimeSystem.s_w_AttackTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            isAttacking = true;                 //플레이어 입력감지
            isPlayGizoms = true;

            TimeSystem.s_w_AttackTimer = w_AttackTimer;
            TimeSystem.s_w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작
        }

        if (isAttacking)
        {
            TimeSystem.s_w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
            Vector3 mousePos = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = hit.point;
            mousePos.y = _currentPos.y;

            currentAttackPos = mousePos;                                    //기즈모에 보여주기용

            Vector3 attackDir = (mousePos - _currentPos).normalized;        //플레이어 기준 마우스 방향 얻기
            float mouseDistance = Vector3.Distance(mousePos, _currentPos);  //플레이어와 마우스 사이의 거리

            //벽이 있을 경우의 예외처리(감지범위 내에 벽이 있으면 그 만큼 체크 거리 감소)---------------------------------------------------------------------------------

            if (Physics.Raycast(_currentPos, attackDir, out RaycastHit hit2, detectDistance, wallLayer))
                _detectDistance = Vector3.Distance(hit2.point, _currentPos);
            else _detectDistance = detectDistance;

            if (mouseDistance <= _detectDistance)
            {
                targetPos = mousePos;                 //공격 가능한 범위 안일 때, 공격 위치값 저장
                cannotAttacking = false;
            }
            else cannotAttacking = true;

            if (Input.GetMouseButtonDown(0))  //공격 감지 및 공격
            {
                //공격 사운드 재생
                //공격 애니메이션 재생

                TimeSystem.s_w_AttackTimer = w_AttackTimer;
                TimeSystem.s_w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

                coolTimer.Start();         //쿨타임 시작

                isAttacking = false;

                if (cannotAttacking)       //공격할 수 없을 시, 초기화
                {
                    isPlayGizoms = false;
                }
                else Attack();
            }
        }
    }

    public override void Attack()
    {
        isPlayGizoms = true;    //테스트용_범위 기즈모 활성화
        StartCoroutine(C_Attack());
    }

    private IEnumerator C_Attack()
    {
        AttackInfo attackInfo = new AttackInfo();
        attackInfo.damage = damage;

        float totalAttackTime = 0;
        while (true)
        {
            Collider[] cols = Physics.OverlapSphere(targetPos, attackDistance, enemyLayer);   //감지 범위 내 적 감지

            foreach (Collider col in cols)
            {
                attackInfo.attackDirection = (col.transform.position - targetPos).normalized; //공격 방향 설정
                if (col.gameObject != null) col.SendMessage("ApplyDamage", attackInfo);
            }

            if (totalAttackTime >= attackTime) break;       //시간 초과시, 코루틴 종료

            yield return waitForSeconds;                    //다음 공격까지 대기
            totalAttackTime += attackCycle;                 //대기 시간 누적 추가

            yield return waitForFixedUpdate;
        }
        isPlayGizoms = false;
    }

    private void OnDrawGizmos()
    {
        if (isPlayGizoms)
        {
            if (!cannotAttacking) Gizmos.color = Color.white;
            else Gizmos.color = Color.red;

            if(isAttacking) Gizmos.DrawWireSphere(currentAttackPos, attackDistance);
            else Gizmos.DrawWireSphere(targetPos, attackDistance);
        }
    }
}
