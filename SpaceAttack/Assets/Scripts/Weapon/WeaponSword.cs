using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSword : WeaponType
{
    //임시로 플레이어에 할당

    private float attackDistance = 2f;
    private float damage = 4f;
    private float w_attack = 0.2f;
    private float mass = 1f;
    private float detectAngle = 155f;
    public float w_attackTime = 0.4f;    //검 공격 대기 시간

    private LayerMask planLayer;   //바닥감지용 리이어 마스크
    private LayerMask enemyLayer;  //적감지용 레이어 마스크

    private Timer attackMoveTimer;
    private bool isDetected;       //적 감지 여부
    private float moveDuration;
    private Vector2 targetPos;

    public override void OnEnable()
    {
        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        enemyLayer |= 1 << LayerMask.NameToLayer("Enemy");

        attackMoveTimer = new Timer(0.1f);         //공격 이동 속도 설정
        w_AttackTimer = new Timer(w_attackTime);   //공격 대기 시간 설정
    }

    public override void UpdateInfo()
    {
        isAttackMoving = attackMoveTimer.IsRunning(); //이동여부 설정
        attackMoveTimer.Update();

        if (isAttackMoving)
        {
            float timer = attackMoveTimer.GetRemainingTimer() / 0.1f;
            Vector2 movePos = Vector2.Lerp(_currentPos, targetPos, 1 - timer);
            attackMovePos = movePos;   //이동 위치 할당
        }

        if (!isAttacking) return;

        if (!isDetected) moveDuration = 0.7f;  //적 감지 안됨
        else moveDuration = 0.1f;              //적 감지 됨

        attackMoveTimer.Start();
        targetPos = _currentPos + attackDirection * moveDuration;
    }

    public override void CheckAttack(Vector2 currentPos)
    {
        _currentPos = currentPos;

        if (Input.GetMouseButtonDown(0))  //마우스 클릭시, 공격
        {
            if (TimeSystem.w_AttackTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            attackAnimator.SetBool("IsAttacking", true);      //공격 애니메이션 실행
            TimeSystem.w_swordTimer.Start();                  //검 공격 준비 체크 시작
            TimeSystem.w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

            isAttacking = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
            Vector2 mousePos = Vector2.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = (Vector2)hit.point;

            Vector2 attackDir = (mousePos - _currentPos).normalized * attackDistance;   //플레이어 기준 마우스 방향 얻기

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
        isDetected = false;

        Collider2D[] enemyCols = Physics2D.OverlapCircleAll(_currentPos, attackDistance, enemyLayer, -2f, 2f);  //원모양 범위내로 모든 적 감지
        foreach (var enemyCol in enemyCols)
        {
            isDetected = true;   //적 확인

            Vector2 dirToEnemy = (Vector2)enemyCol.transform.position - _currentPos;
            if (Vector2.Angle(attackDirection, dirToEnemy) <= detectAngle / 2f)            //각도내에 적에게만 공격
            {
                StartCoroutine(C_Attack(enemyCol.gameObject));
            }
        }
    }

    private IEnumerator C_Attack(GameObject target)
    {
        while (true)
        {
            if (!TimeSystem.w_swordTimer.IsRunning()) break;

            yield return null;
        }

        AttackInfo attackInfo = new AttackInfo();
        attackInfo.damage = damage;
        attackInfo.attackDirection = attackDirection;

        target.SendMessage("ApplyDamage", attackInfo);
        Debug.Log("검 공격");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawAttackLine(transform.position, attackDistance, attackDirection, detectAngle);

        Gizmos.color = Color.blue;
        DrawCilcle(transform.position, attackDistance, 40);
    }

    private void DrawAttackLine(Vector2 center, float redius, Vector2 forward, float angle)
    {
        forward = forward.normalized;

        Quaternion l_rotate = Quaternion.Euler(0, 0, - angle / 2f);
        Quaternion r_rotate = Quaternion.Euler(0, 0, angle / 2f);

        Vector2 leftRay = l_rotate * forward;
        Vector2 rightRay = r_rotate * forward;

        Gizmos.DrawRay(center, forward);
        Gizmos.DrawRay(center, leftRay * redius);
        Gizmos.DrawRay(center, rightRay * redius);

    }

    private void DrawCilcle(Vector2 center, float radius, float segement)
    {
        float angle = 0f;

        Vector2 prePos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        for (int i = 1; i <= segement; i++)
        {
            angle = i * Mathf.PI * 2f / segement;

            Vector2 newPos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            Gizmos.DrawLine(newPos, prePos);

            prePos = newPos;
        }
    }
}
