using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : WeaponType     //시전시간(발사: 애니메이션 후, 실행) O | 공격시간 O | 플레이어 대기시간 O
{
    private Timer _m_AttackTimer;

    private float attackWidth = 0.2f;     //차지 너비
    private float attackDistance = 6f;    //최대 차지 거리
    private float w_attackTime = 0.55f;     //공격 대기 시간

    private float mass = 0.7f;              //무기 질량
    private float damage = 1f;              //공격력
    private float attackTime = 0.3f;        //이동 시간
    private float r_AttackTime = 0.2f;      //화살 발사 시간

    private LayerMask planLayer;   //바닥감지용 리이어 마스크
    private LayerMask enemyLayer;  //적감지용 레이어 마스크
    private LayerMask wallLayer;   //벽감지용 레이어 마스크

    private Vector3 f_DetectPos;     //기즈모 그리는 용
    private Vector3 f_DetectSize;
    private Quaternion detectRot;

    private Vector3 detectPos;
    private Vector3 detectSize;

    float _attackDistance;          //공격범위 예외처리용 복사본 변수
    float _attackTime;

    Vector3 startPos;
    Vector3 targetPos;

    AttackInfo attackInfo = new AttackInfo();

    bool startAttack;

    public override void OnEnable()
    {
        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        enemyLayer |= (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("DestructableObject"));
        wallLayer |= 1 << LayerMask.NameToLayer("Wall");

        w_AttackTimer = new Timer(w_attackTime);
        m_AttackTimer = new Timer(attackTime);
        _m_AttackTimer = m_AttackTimer;
        r_AttackTimer = new Timer(r_AttackTime);
    }

    public override void UpdateInfo()
    {
        m_AttackTimer.Update();

        if (!startAttack) return;
        _Attack();
    }
    public override void CheckAttack(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (Input.GetMouseButtonDown(0))  //플레이어 입력감지
        {
            if (TimeSystem.s_w_AttackTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            // 애니메이션 추가
            // 사운드 추가

            TimeSystem.s_w_AttackTimer = w_AttackTimer;
            TimeSystem.s_w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

            isAttacking = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
            Vector3 mousePos = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = hit.point;
            mousePos.y = _currentPos.y;

            Vector3 attackDir = (mousePos - _currentPos).normalized;   //플레이어 기준 마우스 방향 얻기

            attackDirection = attackDir;

            Attack();
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void Attack()
    {
        _attackDistance = attackDistance;
        _attackTime = attackTime;

        if (Physics.Raycast(_currentPos, attackDirection, out RaycastHit hit, attackDistance, wallLayer))   //벽이 있을 경우의 예외처리(이동거리, 이동시간)
        {
            _attackDistance = Vector3.Distance(hit.point, _currentPos);

            if (attackDistance >= _attackDistance)
            {
                _attackTime *= _attackDistance / attackDistance;
                m_AttackTimer = new Timer(_attackTime);
            }
            else
            {
                m_AttackTimer = _m_AttackTimer;
            }
        }
        else
        {
            m_AttackTimer = _m_AttackTimer;
        }

        SetAttackRange();
    }

    private void SetAttackRange()
    {
        f_DetectPos = _currentPos + (attackDirection * (_attackDistance / 2));
        f_DetectSize = new Vector3(_attackDistance / 1.5f, 1f, attackWidth / 2);
        detectRot = Quaternion.LookRotation(attackDirection, Vector2.up) * Quaternion.Euler(0, 90f, 0);

        detectSize = new Vector3(0.2f, 1f, attackWidth / 2);

        startPos = _currentPos;
        targetPos = _currentPos + attackDirection * (_attackDistance - 0.2f);

        attackInfo.SetAttackInfo(damage, attackDirection, mass);  //공격정보 설정

        Invoke("StartAttack", r_AttackTime);   //공격준비시간동안 대기
    }
    private void StartAttack()
    {
        m_AttackTimer.Start();                 //공격 이동 타이머 시작
        startAttack = true;
    }
    private void _Attack()
    {
        float timer = m_AttackTimer.GetRemainingTimer() / _attackTime;
        Vector3 movePos = Vector3.Lerp(startPos, targetPos, 1 - timer);

        detectPos = movePos;

        Collider[] cols = Physics.OverlapBox(detectPos, detectSize, detectRot, enemyLayer);   //감지 범위 내 적 감지

        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                if (col.gameObject != null)
                    col.SendMessage("ApplyDamage", attackInfo);
                startAttack = false;                                           //단일 타격기 이기 때문에 적 한명 피격후, 종료
            }
            else if (col.gameObject.CompareTag("DestructableObject"))
            {
                if (col.gameObject != null)
                    col.SendMessage("ApplyDamage", attackInfo);
            }
        }

        if (timer <= 0) startAttack = false;  //시간 초과시, 공격 취소
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
