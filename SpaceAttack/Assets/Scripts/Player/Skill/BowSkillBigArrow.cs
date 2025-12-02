using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class BowSkillBigArrow : SkillType
{
    private WaitForFixedUpdate waitForFixedUpdate;

    private Quaternion detectRot;
    private Vector3 detectPos;

    private List<GameObject> targets = new List<GameObject>();

    public override void OnEnable()
    {
        attackWidth = 2.5f;
        waitForFixedUpdate = new WaitForFixedUpdate();

        chipsetCompID = 110;
        base.OnEnable();
    }

    public override void UpdateInfo() { base.UpdateInfo(); }

    public override void CheckUse(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (PlayerInputController.skill3Action.triggered)
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;

            if (coolTimer.IsRunning()) return;                      //쿨타임 체크

            isAttacking = true;                                     //플레이어 입력감지

            PlayerTimeSystem.SetChipTimer(0.2f, ChipAttackType.Skill);
            PlayerTimeSystem.w_SkillTimer.Start();                 //다음 공격 전 대기 체크 시작

            attackDirection = GetAttackDirection(currentPos);

            _attackDistance = attackDistance;
            _attackTime = attackTime;

            if (Physics.Raycast(_currentPos, attackDirection, out RaycastHit hit2, attackDistance, wallLayer))
            {
                _attackDistance = Vector3.Distance(hit2.point, _currentPos);

                if (attackDistance >= _attackDistance)
                {
                    _attackTime *= _attackDistance / attackDistance;
                }
            }

            //공격 사운드 재생
            //공격 애니메이션 재생
            coolTimer.Start();         //쿨타임 시작

            projectileMoveTime = _attackTime;
            Use();
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void Use()
    {
        p_MoveTimer.Start();
        StartCoroutine(C_Attack(_attackDistance, _attackTime));
    }

    private IEnumerator C_Attack(float _attackDistance, float _attackTime)
    {
        detectRot = Quaternion.LookRotation(attackDirection, Vector2.up) * Quaternion.Euler(0, 90f, 0);
        detectSize = new Vector3(0.2f, 1f, attackWidth / 2);

        Vector3 startPos = _currentPos;
        Vector3 targetPos = _currentPos + attackDirection * (_attackDistance - 0.2f);

        OnVisualAttackRange(startPos, _attackDistance, attackWidth, attackDirection, _attackTime);

        while (true)
        {
            float timer = p_MoveTimer.GetRemainingTime() / _attackTime;
            Vector3 movePos = Vector3.Lerp(startPos, targetPos, 1 - timer);
            detectPos = movePos;

            Collider[] cols = Physics.OverlapBox(detectPos, detectSize, detectRot, enemyLayer);   //감지 범위 내 적 감지

            foreach (Collider col in cols)
            {
                if (targets.Contains(col.gameObject)) continue;   //중복일 경우, 무시
                else targets.Add(col.gameObject);                  //중복이 아닐 경우, 체크 대상에 추가

                if (col.gameObject != null) chipset.Attack(col.gameObject, damageRate, attackDirection, addedCritChanceRate, addedCritRate, ChipAttackType.Skill);
            }

            if (timer <= 0) break;  //시간 초과시, 코루틴 종료

            yield return waitForFixedUpdate;
        }

        targets.Clear();
    }
}
