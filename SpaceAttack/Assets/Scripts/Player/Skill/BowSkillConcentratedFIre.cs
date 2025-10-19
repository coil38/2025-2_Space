using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSkillConcentratedFire : SkillType
{
    private WaitForFixedUpdate waitForFixedUpdate;
    private WaitForSeconds waitForSeconds;

    private float attackCycle;

    private bool isPlayGizoms;
    private bool cannotAttacking;

    private Vector3 targetPos;
    private Vector3 currentAttackPos;

    public override void OnEnable()
    {
        attackCycle = 0.5f;     //공격주기

        waitForFixedUpdate = new WaitForFixedUpdate();
        waitForSeconds = new WaitForSeconds(attackCycle);

        chipsetCompID = 109;
        base.OnEnable();
    }

    public override void UpdateInfo() { base.UpdateInfo(); }

    public override void CheckUse(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (PlayerInputController.skill2Action.triggered)
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;

            if (coolTimer.IsRunning()) return;                               //쿨타임 처리

            isAttacking = true;                                              //플레이어 입력감지
            isPlayGizoms = true;

            PlayerTimeSystem.SetChipTimer(attackTime, ChipAttackType.Skill);
            PlayerTimeSystem.w_SkillTimer.Start();                           //다음 공격 전 대기 체크 시작
        }

        if (isAttacking)
        {
            PlayerTimeSystem.w_SkillTimer.Start();                           //다음 공격 전 대기 체크 시작

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);     //마우스 위치 받기
            Vector3 mousePos = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = hit.point;
            mousePos.y = _currentPos.y;

            currentAttackPos = mousePos;                                    //기즈모에 보여주기용

            Vector3 attackDir = (mousePos - _currentPos).normalized;        //플레이어 기준 마우스 방향 얻기
            float mouseDistance = Vector3.Distance(mousePos, _currentPos);  //플레이어와 마우스 사이의 거리

            //벽이 있을 경우의 예외처리(감지범위 내에 벽이 있으면 그 만큼 체크 거리 감소)---------------------------------------------------------------------------------

            if (Physics.Raycast(_currentPos, attackDir, out RaycastHit hit2, attackDistance, wallLayer))
                _attackDistance = Vector3.Distance(hit2.point, _currentPos);
            else _attackDistance = attackDistance;

            if (mouseDistance <= _attackDistance)
            {
                targetPos = mousePos;                 //공격 가능한 범위 안일 때, 공격 위치값 저장
                cannotAttacking = false;
            }
            else cannotAttacking = true;

            if (Input.GetMouseButtonDown(0))  //공격 감지 및 공격
            {
                //공격 사운드 재생
                //공격 애니메이션 재생

                PlayerTimeSystem.SetChipTimer(attackTime, ChipAttackType.Skill);
                PlayerTimeSystem.w_SkillTimer.Start();                 //다음 공격 전 대기 체크 시작

                coolTimer.Start();         //쿨타임 시작

                isAttacking = false;

                if (cannotAttacking)       //공격할 수 없을 시, 초기화
                {
                    isPlayGizoms = false;
                }
                else Use();
            }
        }
    }

    public override void Use()
    {
        isPlayGizoms = true;    //테스트용_범위 기즈모 활성화
        StartCoroutine(C_Attack());
    }

    private IEnumerator C_Attack()
    {
        float totalAttackTime = 0;
        while (true)
        {
            Collider[] cols = Physics.OverlapSphere(targetPos, attackDistance * 0.5f, enemyLayer);   //감지 범위 내 적 감지

            foreach (Collider col in cols)
            {
                Vector3 _attackDirection = (col.transform.position - targetPos).normalized; //공격 방향 설정
                if (col.gameObject != null) chipset.Attack(col.gameObject, damageRate, _attackDirection, addedCritChanceRate, addedCritRate, ChipAttackType.Skill);
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

            if(isAttacking) Gizmos.DrawWireSphere(currentAttackPos, attackDistance * 0.5f);
            else Gizmos.DrawWireSphere(targetPos, attackDistance * 0.5f);
        }
    }
}
