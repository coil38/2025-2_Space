using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillSting : SkillType     //시전시간(발사: 애니메이션 후, 실행) O | 공격시간 O | 플레이어 대기시간(쿨타임) O
{
    private Vector3 f_DetectPos;     //기즈모 그리는 용
    private Vector3 f_DetectSize;
    private Quaternion detectRot;

    private Vector3 detectPos;

    private WaitForFixedUpdate waitForFixedUpdate;

    private const float aniSpeed = 2.5f;

    public override void OnEnable()
    {
        unLockedNumber = 1;
        attackDistance = 3.2f;
        attackWidth = 2f;
        waitForFixedUpdate = new WaitForFixedUpdate();

        chipsetCompID = 104;
        base.OnEnable();
    }

    public override void UpdateInfo() { base.UpdateInfo(); }

    public override void CheckUse(Vector3 currentPos)
    {
        //if (!isUnLocked) return;                                      //해금여부에 따른 스킬 사용 여부

        if (PlayerInputController.skill1Action.triggered)           //플레이어 입력감지
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;

            if (coolTimer.IsRunning()) return;                      //쿨타임 중 실행불가처리

            //LogUtil.Log($"스킬 쿨타임: {coolTime}");

            _currentPos = currentPos;
            isAttacking = true;

            PlayerTimeSystem.SetChipTimer(_attackTime, ChipAttackType.Skill);    //스킬 사용 타임 설정
            PlayerTimeSystem.w_SkillTimer.Start();                               //스킬 사용 시작

            coolTimer.Start();                                       //쿨타임 시작
            attackDirection = GetAttackDirection(currentPos);        //공격 방향 설정

            _attackDistance = attackDistance;
            _attackTime = attackTime;

            if (Physics.Raycast(_currentPos, attackDirection, out RaycastHit hit, attackDistance, wallLayer))   //벽이 있을 경우의 예외처리(이동거리, 이동시간)
            {
                _attackDistance = Vector3.Distance(hit.point, _currentPos);

                if (attackDistance >= _attackDistance)
                    _attackTime *= _attackDistance / attackDistance;
            }
            projectileMoveTime = _attackTime;

            //찌르기 사운드 재생
            PlayerAniInfo aniInfo = new PlayerAniInfo("isSkill1", AniType.Trrigger, 1.3f / ((_attackTime + readyAttackTime) * 1.3f));  //공격 애니메이션 실행
            PlayAniMation(aniInfo);

            Invoke("Use", readyAttackTime);             //시전 애니메니션 시작 후, 시전시간동안 대기
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void Use()
    {
        base.Use();
        p_MoveTimer.Start();
        StartCoroutine(C_Attack(_attackDistance, _attackTime));
    }

    private IEnumerator C_Attack(float _attackDistance, float _attackTime)
    {
        f_DetectPos = _currentPos + (attackDirection * (_attackDistance / 2));
        f_DetectSize = new Vector3(_attackDistance / 2f, 1f, attackWidth / 2f);
        detectRot = Quaternion.LookRotation(attackDirection, Vector2.up) * Quaternion.Euler(0, 90f, 0);

        detectSize = new Vector3(0.2f, 1f, attackWidth / 2);
        Vector3 _detectSize = new Vector3(1f, 1f, 1f);

        Vector3 startPos = _currentPos;
        Vector3 targetPos = _currentPos + attackDirection * _attackDistance;

        OnVisualAttackRange(_currentPos, _attackDistance, attackWidth, attackDirection, _attackTime);

        isAttackMoving = true;

        while (true)
        {
            float timer = p_MoveTimer.GetRemainingTime() / _attackTime;
            Vector3 movePos = Vector3.Lerp(startPos, targetPos, 1 - timer);
            attackMovePos = movePos;                                                              //이동 위치 할당

            detectPos = movePos;

            Collider[] cols = Physics.OverlapBox(detectPos, detectSize, detectRot, enemyLayer);   //감지 범위 내 적 감지

            foreach (Collider col in cols)
            {
                if (col.gameObject.CompareTag("DestructableObject"))
                {
                    if (col.gameObject != null)
                        chipset.Attack(col.gameObject, damageRate, attackDirection, addedCritChanceRate, addedCritRate, ChipAttackType.Skill);
                    continue;
                }

                Collider[] cols2 = Physics.OverlapBox(col.gameObject.transform.position, _detectSize, detectRot, enemyLayer);   //첫 타 후, 감지 범위 내 모든 적 감지
                foreach (var col2 in cols2)
                {
                    if (col2.gameObject != null)
                        chipset.Attack(col2.gameObject, damageRate, attackDirection, addedCritChanceRate, addedCritRate, ChipAttackType.Skill);
                }
                isAttackMoving = false;
                yield break;

            }

            if (timer <= 0) break;  //시간 초과시, 코루틴 종료

            yield return waitForFixedUpdate;
        }
        isAttackMoving = false;
    }

    private void OnDrawGizmos()
    {
        Vector3 temp = attackDirection;
        if (temp.magnitude < 0.1) temp = Vector3.forward;
        Quaternion _detectRot = Quaternion.LookRotation(temp, Vector2.up);
        _detectRot *= Quaternion.Euler(0, 90f, 0);

        Gizmos.matrix = Matrix4x4.TRS(f_DetectPos, _detectRot, Vector3.one);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector3.zero, f_DetectSize);

        Gizmos.matrix = Matrix4x4.TRS(detectPos, _detectRot, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, detectSize);
    }
}
