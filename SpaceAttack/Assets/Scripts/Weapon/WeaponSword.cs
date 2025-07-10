using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSword : WeaponType
{
    //임시로 플레이어에 할당

    private float attackDistance = 2f;
    private float damage = 4f;
    //private float w_attack = 0.2f;
    //private float mass = 1f;
    private float detectAngle = 155f;
    private float w_attackTime = 0.4f;    //검 공격 대기 시간

    private LayerMask planLayer;   //바닥감지용 리이어 마스크
    private LayerMask enemyLayer;  //적감지용 레이어 마스크

    private Timer attackMoveTimer;
    private bool isDetected;       //적 감지 여부
    private float moveDuration;
    private Vector3 targetPos;

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
            Vector3 movePos = Vector3.Lerp(_currentPos, targetPos, 1 - timer);
            attackMovePos = movePos;   //이동 위치 할당
        }

        if (!isAttacking) return;

        if (!isDetected) moveDuration = 0.7f;  //적 감지 안됨
        else moveDuration = 0.1f;              //적 감지 됨

        attackMoveTimer.Start();
        targetPos = _currentPos + attackDirection * moveDuration;
    }

    public override void CheckAttack(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (Input.GetMouseButtonDown(0))  //마우스 클릭시, 공격
        {
            if (TimeSystem.w_AttackTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound("Attack");

            attackAnimator.SetBool("IsAttacking", true);      //공격 애니메이션 실행
            TimeSystem.w_swordTimer.Start();                  //검 공격 준비 체크 시작
            TimeSystem.w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

            isAttacking = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
            Vector3 mousePos = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = hit.point;
            mousePos.y = _currentPos.y;

            Vector3 attackDir = (mousePos - _currentPos).normalized * attackDistance;   //플레이어 기준 마우스 방향 얻기

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

        Collider[] enemyCols = Physics.OverlapSphere(_currentPos, attackDistance, enemyLayer);
        foreach (var enemyCol in enemyCols)
        {
            Vector3 dirToEnemy = enemyCol.transform.position - _currentPos;
            dirToEnemy.y = 0f;
            if (Vector3.Angle(attackDirection, dirToEnemy) <= detectAngle / 2f)            //각도내에 적에게만 공격
            {
                isDetected = true;   //적 확인

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

        if (target.gameObject == null) yield break;   //적이 없을 경우, 코루틴 종료

        target.SendMessage("ApplyDamage", attackInfo);
        Camera.main.GetComponent<CameraFallow>().CameraShack();  //카메라 흔들림 연출
        Debug.Log("검 공격");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawAttackLine(transform.position, attackDistance, attackDirection, detectAngle);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    private void DrawAttackLine(Vector3 center, float redius, Vector3 forward, float angle)
    {
        forward = forward.normalized;

        Quaternion l_rotate = Quaternion.Euler(0, -angle / 2f, 0);
        Quaternion r_rotate = Quaternion.Euler(0, angle / 2f, 0);

        Vector3 leftRay = l_rotate * forward;
        Vector3 rightRay = r_rotate * forward;

        Gizmos.DrawRay(center, forward);
        Gizmos.DrawRay(center, leftRay * redius);
        Gizmos.DrawRay(center, rightRay * redius);

    }
}
