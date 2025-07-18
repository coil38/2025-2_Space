using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSkillBigArrow : SkillType
{
    private Timer _s_AttackTimer;  //초기값인 s_AttackTimer의 복제본

    private LayerMask planLayer;   //바닥감지용 리이어 마스크
    private LayerMask wallLayer;   //벽감지용 레이어 마스크
    private LayerMask enemyLayer;  //적감지용 레이어 마스크

    private WaitForFixedUpdate waitForFixedUpdate;

    private Vector3 f_DetectPos;     //기즈모 그리는 용
    private Vector3 f_DetectSize;
    private Quaternion detectRot;

    private Vector3 detectPos;
    private Vector3 detectSize;

    private float _attackDistance;
    private float _attackTime;

    private List<GameObject> targets = new List<GameObject>();

    private bool isPlayGizoms;

    public override void OnEnable()
    {
        damage = 3f;
        attackDistance = 6f;
        attackWidth = 4f;
        attackTime = 0.6f;
        playerWaitTime = 0.2f;
        coolTime = 8f;
        coolTimer = new Timer(coolTime);
        w_AttackTimer = new Timer(playerWaitTime);
        s_AttackTimer = new Timer(attackTime);                 //임시
        _s_AttackTimer = s_AttackTimer;

        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        wallLayer |= 1 << LayerMask.NameToLayer("Wall");
        enemyLayer |= (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("DestructableObject"));

        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    public override void UpdateInfo()
    {
        coolTimer.Update();
        s_AttackTimer.Update();
    }

    public override void CheckAttack(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (coolTimer.IsRunning() || TimeSystem.s_w_AttackTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            isAttacking = true;                 //플레이어 입력감지
            lineRenderer.enabled = true;

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

            Vector3 attackDir = (mousePos - _currentPos).normalized;   //플레이어 기준 마우스 방향 얻기

            attackDirection = attackDir;

            //벽이 있을 경우의 예외처리(이동거리, 이동시간)----------------------------------------------------------------------------------------------------

            _attackDistance = attackDistance;
            _attackTime = attackTime;

            if (Physics.Raycast(_currentPos, attackDirection, out RaycastHit hit2, attackDistance, wallLayer))
            {
                _attackDistance = Vector3.Distance(hit2.point, _currentPos);

                if (attackDistance >= _attackDistance)
                {
                    _attackTime *= _attackDistance / attackDistance;
                    s_AttackTimer = new Timer(_attackTime);
                }
                else s_AttackTimer = _s_AttackTimer;
            }
            else s_AttackTimer = _s_AttackTimer;

            Vector3 startPos = _currentPos;
            Vector3 targetPos = _currentPos + attackDirection * (_attackDistance - 0.2f);

            //라인랜더러 설정----------------------------------------------------------------------------------------------------------------------------------
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = attackWidth;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, targetPos);

            if (Input.GetMouseButtonDown(0))  //공격 감지 및 공격
            {
                //공격 사운드 재생
                //공격 애니메이션 재생

                TimeSystem.s_w_AttackTimer = w_AttackTimer;
                TimeSystem.s_w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

                coolTimer.Start();         //쿨타임 시작

                Attack();
                isAttacking = false;

                //라인랜더러값 초기화
                lineRenderer.enabled = false;
            }
        }
    }

    public override void Attack()
    {
        isPlayGizoms = true;    //테스트용_범위 기즈모 활성화

        s_AttackTimer.Start();                                   //다음 공격 전 대기 체크 시작
        StartCoroutine(C_Attack(_attackDistance, _attackTime));
    }

    private IEnumerator C_Attack(float _attackDistance, float _attackTime)
    {
        f_DetectPos = _currentPos + (attackDirection * (_attackDistance / 2));
        f_DetectSize = new Vector3(_attackDistance / 1.5f, 1f, attackWidth / 2);
        detectRot = Quaternion.LookRotation(attackDirection, Vector2.up) * Quaternion.Euler(0, 90f, 0);

        detectSize = new Vector3(0.2f, 1f, attackWidth / 2);

        AttackInfo attackInfo = new AttackInfo();
        attackInfo.damage = damage;
        attackInfo.attackDirection = attackDirection;

        Vector3 startPos = _currentPos;
        Vector3 targetPos = _currentPos + attackDirection * (_attackDistance - 0.2f);

        while (true)
        {
            float timer = s_AttackTimer.GetRemainingTimer() / _attackTime;
            Vector3 movePos = Vector3.Lerp(startPos, targetPos, 1 - timer);
            detectPos = movePos;

            Collider[] cols = Physics.OverlapBox(detectPos, detectSize, detectRot, enemyLayer);   //감지 범위 내 적 감지

            foreach (Collider col in cols)
            {
                if (targets.Contains(col.gameObject)) continue;   //중복일 경우, 무시
                else targets.Add(col.gameObject);                  //중복이 아닐 경우, 체크 대상에 추가

                if (col.gameObject != null) col.SendMessage("ApplyDamage", attackInfo);
            }

            if (timer <= 0) break;  //시간 초과시, 코루틴 종료

            yield return waitForFixedUpdate;
        }
        isPlayGizoms = false;

        targets.Clear();
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
