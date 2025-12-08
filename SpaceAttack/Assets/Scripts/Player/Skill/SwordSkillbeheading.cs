using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillbeheading : SkillType     //시전시간(발사: 애니메이션 후, 실행) O | 공격시간 O | 플레이어 대기시간(쿨타임) O
{
    [SerializeField] private SkillEffect skillEffect;

    private Vector3 f_DetectPos;     //기즈모 그리는 용
    private Vector3 f_DetectSize;
    private Quaternion detectRot;

    private Vector3 detectPos;

    private List<GameObject> targets = new List<GameObject>();

    private WaitForFixedUpdate waitForFixedUpdate;

    private const float aniSpeed = 2.5f;

    public override void OnEnable()
    {
        unLockedNumber = 2;
        attackWidth = 2f;
        waitForFixedUpdate = new WaitForFixedUpdate();

        chipsetCompID = 105;
        base.OnEnable();
    }

    public override void UpdateInfo() { base.UpdateInfo(); }

    public override void CheckUse(Vector3 currentPos)
    {
        //if (!isUnLocked) return;                                      //해금여부에 따른 스킬 사용 여부

        _currentPos = currentPos;

        if (PlayerInputController.skill2Action.triggered)       //플레이어 입력감지
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;

            if (coolTimer.IsRunning()) return;                  //다음 공격 대기 체크 실행중, 리턴

            isAttacking = true;
            //참격 사운드 재생
            PlayerAniInfo aniInfo = new PlayerAniInfo("isSkill2", AniType.Trrigger, aniSpeed);  //공격 애니메이션 실행
            PlayAniMation(aniInfo);

            coolTimer.Start();                                  //쿨타임 시작

            PlayerTimeSystem.SetChipTimer(attackTime * 0.2f, ChipAttackType.Skill);
            PlayerTimeSystem.w_SkillTimer.Start();                                    //스킬 사용 시작

            attackDirection = GetAttackDirection(currentPos);

            _attackDistance = attackDistance;
            _attackTime = attackTime;
            if (Physics.Raycast(_currentPos, attackDirection, out RaycastHit hit, attackDistance, wallLayer))   //벽이 있을 경우의 예외처리(이동거리, 이동시간)
            {
                _attackDistance = Vector3.Distance(hit.point, _currentPos);

                if (attackDistance >= _attackDistance)
                {
                    _attackTime *= _attackDistance / attackDistance;
                }
            }

            projectileMoveTime = _attackTime;
            Invoke("Use", 1.2f / aniSpeed);
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void Use()
    {
        base.Use();
        LogUtil.Log("참격 발사체 이동 시작");
        p_MoveTimer.Start();
        StartCoroutine(C_Attack(_attackDistance, _attackTime));
    }

    private IEnumerator C_Attack(float _attackDistance, float _attackTime)
    {
        f_DetectPos = _currentPos + (attackDirection * (_attackDistance / 2));
        f_DetectSize = new Vector3(_attackDistance / 2f, 1f, attackWidth / 2f);
        detectRot = Quaternion.LookRotation(attackDirection, Vector2.up) * Quaternion.Euler(0, 90f, 0);

        detectSize = new Vector3(0.2f, 1f, attackWidth / 2f);

        Vector3 startPos = _currentPos;
        Vector3 targetPos = _currentPos + attackDirection * _attackDistance;

        skillEffect.OnSkillEffect(_currentPos, attackDirection);
        OnVisualAttackRange(_currentPos, _attackDistance, attackWidth, attackDirection, _attackTime);

        while (true)
        {
            float timer = p_MoveTimer.GetRemainingTime() / _attackTime;
            Vector3 movePos = Vector3.Lerp(startPos, targetPos, 1 - timer);

            detectPos = movePos;
            skillEffect.UpdateSkillEffect(movePos);
            Collider[] cols = Physics.OverlapBox(detectPos, detectSize, detectRot, enemyLayer);   //감지 범위 내 적 감지

            foreach (Collider col in cols)
            {
                if (targets.Contains(col.gameObject)) continue;   //중복일 경우, 무시
                else targets.Add(col.gameObject);                  //중복이 아닐 경우, 체크 대상에 추가

                if (col.gameObject != null) chipset.Attack(col.gameObject, damageRate, attackDirection, addedCritChanceRate, addedCritRate, ChipAttackType.Skill);
            }

            if (timer <= 0) break;  //시간 초과될 시, 코루틴 종료

            yield return waitForFixedUpdate;
        }

        skillEffect.EndSkillEffect();
        targets.Clear();
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
