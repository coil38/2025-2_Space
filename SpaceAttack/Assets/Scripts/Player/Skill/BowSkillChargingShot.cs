using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSkillChargingShot : SkillType
{
    private WaitForFixedUpdate waitForFixedUpdate;

    private Vector3 f_DetectPos;     //기즈모 그리는 용
    private Vector3 f_DetectSize;
    private Quaternion detectRot;

    private Vector3 detectPos;

    private List<GameObject> targets = new List<GameObject>();

    private bool isPlayGizoms;

    private int gaugeCount = 0;
    private const int maxGaugeCount = 3;

    public override void OnEnable()
    {
        chipsetCompID = 110;
        base.OnEnable();

        gaugeCount = 0;
        SetGaugeData(gaugeCount);

        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void SetGaugeData(int _gaugeCount)
    {
        damageRate = damageRates[_gaugeCount];
        addedCritChanceRate = addedCritChanceRates[_gaugeCount];
        addedCritRate = addedCritRates[_gaugeCount];
        attackDistance = attackDistances[_gaugeCount];
        attackWidth = attackDistances[_gaugeCount] * 0.25f;
        attackTime = attackTimes[_gaugeCount];
        coolTime = coolTimes[_gaugeCount];
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();

        if (isAttacking)           //차징
        {
            bool isCharging = PlayerTimeSystem.w_SkillTimer.IsRunning();
            if (isCharging)
            {
                //차징 중..
            }
            else
            {
                gaugeCount++;

                if (gaugeCount < maxGaugeCount)
                {
                    SetGaugeData(gaugeCount);
                    PlayerTimeSystem.SetChipTimer(attackTime, ChipAttackType.Skill);
                    PlayerTimeSystem.w_SkillTimer.Start();                           //스킬 사용 타이머 시작
                }
            }
        }
    }

    public override void CheckUse(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (PlayerInputController.skill3Action.triggered)
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;

            if (coolTimer.IsRunning()) return;                  //쿨타임 처리

            gaugeCount = 0;
            SetGaugeData(gaugeCount);

            PlayerTimeSystem.SetChipTimer(attackTime, ChipAttackType.Skill);
            PlayerTimeSystem.w_SkillTimer.Start();                           //스킬 사용 타이머 시작

            isAttacking = true;                                 //플레이어 입력감지
            lineRenderer.enabled = true;
        }

        if (isAttacking)
        {
            attackDirection = GetAttackDirection(currentPos);

            _attackDistance = attackDistance;
            _attackTime = attackTime;

            if (Physics.Raycast(_currentPos, attackDirection, out RaycastHit hit2, attackDistance, wallLayer))
            {
                _attackDistance = Vector3.Distance(hit2.point, _currentPos);

                if (attackDistance >= _attackDistance)
                    _attackTime *= _attackDistance / attackDistance;
            }

            Vector3 startPos = _currentPos;
            Vector3 targetPos = _currentPos + attackDirection * (_attackDistance - 0.2f);

            //라인랜더러 설정
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = attackWidth;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, targetPos);

            LogUtil.Log(gaugeCount);
            if(gaugeCount >= maxGaugeCount) PlayerTimeSystem.w_SkillTimer.Start();

            if (Input.GetMouseButtonDown(0))  //공격 감지 및 공격
            {
                //공격 사운드 재생
                //공격 애니메이션 재생

                LogUtil.Log("작작ㄱ동한다ㅏ");

                PlayerTimeSystem.SetChipTimer(attackTime, ChipAttackType.Skill);
                PlayerTimeSystem.w_SkillTimer.Start();                             //스킬 사용 처리

                coolTimer.Start();                                                 //쿨타임 시작

                projectileMoveTime = _attackTime;                                  //발사체시간 설정

                Use();

                isAttacking = false;
                //라인랜더러값 초기화
                lineRenderer.enabled = false;
            }
        }
    }

    public override void Use()
    {
        isPlayGizoms = true;    //테스트용_범위 기즈모 활성화

        p_MoveTimer.Start();                                   //다음 공격 전 대기 체크 시작
        StartCoroutine(C_Attack(_attackDistance, _attackTime));
    }

    private IEnumerator C_Attack(float _attackDistance, float _attackTime)
    {
        f_DetectPos = _currentPos + (attackDirection * (_attackDistance / 2));
        f_DetectSize = new Vector3(_attackDistance / 1.5f, 1f, attackWidth / 2);
        detectRot = Quaternion.LookRotation(attackDirection, Vector2.up) * Quaternion.Euler(0, 90f, 0);

        detectSize = new Vector3(0.2f, 1f, attackWidth / 2);

        Vector3 startPos = _currentPos;
        Vector3 targetPos = _currentPos + attackDirection * (_attackDistance - 0.2f);

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

        isPlayGizoms = false;
        targets.Clear();

        gaugeCount = 0;
        SetGaugeData(gaugeCount);
    }

    private void OnDrawGizmos()
    {
        if (isPlayGizoms)
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
}
