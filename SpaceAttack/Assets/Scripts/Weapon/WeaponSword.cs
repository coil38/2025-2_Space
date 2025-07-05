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

    private LayerMask planLayer;   //바닥감지용 리이어 마스크
    private LayerMask enemyLayer;  //적감지용 레이어 마스크

    private Vector2 attackdirection = Vector2.zero;  //기즈모 검사용
    private void Start()
    {
        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        enemyLayer |= 1 << LayerMask.NameToLayer("Enemy");
    }
    public override void CheckAttack(Vector2 currentPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
        Vector2 mousePos = Vector2.zero;

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
            mousePos = (Vector2)hit.point;

        Vector2 attackDir = (mousePos - currentPos).normalized * attackDistance;   //플레이어 기준 마우스 방향 얻기

        attackdirection = attackDir;

        if (Input.GetMouseButtonDown(0))  //마우스 클릭시, 공격
        {
            if (TimeSystem.w_AttackTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            attackAnimator.SetBool("IsAttacking", true);      //공격 애니메이션 실행
            TimeSystem.w_swordTimer.Start();                  //검 공격 준비 체크 시작
            TimeSystem.w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

            Collider2D[] enemyCols = Physics2D.OverlapCircleAll(currentPos, attackDistance, enemyLayer, -2f, 2f);  //원모양 범위내로 모든 적 감지
            foreach (var enemyCol in enemyCols)
            {
                Vector2 dirToEnemy = (Vector2)enemyCol.transform.position - currentPos;
                if (Vector2.Angle(attackDir, dirToEnemy) <= detectAngle / 2f)            //각도내에 적에게만 공격
                {
                    Attack(enemyCol.gameObject);
                }
            }
        }
    }

    public override void Attack(GameObject target)
    {
        StartCoroutine(C_Attack(target));
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
        attackInfo.attackDirection = attackdirection;

        target.SendMessage("ApplyDamage", attackInfo);
        Debug.Log("검 공격");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawAttackLine(transform.position, attackDistance, attackdirection, detectAngle);

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
