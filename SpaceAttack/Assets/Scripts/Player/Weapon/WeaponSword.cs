using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSword : WeaponType
{
    //검사용 변수들
    private float detectAngle = 155f;

    //공격 이동을 위한 변수들
    private Timer attackMoveTimer;
    private bool isDetected;       //적 감지 여부
    private float moveDistance = 0.7f;
    private Vector3 targetPos;

    //범위공격용
    private Queue<GameObject> targets = new Queue<GameObject>();
    public override void OnEnable()
    {
        chipsetCompID = 103;
        attackMoveTimer = new Timer(0.1f);         //공격 이동 속도 설정
        base.OnEnable();
    }

    public override void UpdateInfo()
    {
        isAttackMoving = attackMoveTimer.IsRunning(); //이동여부 설정
        attackMoveTimer.Update();

        if (isAttackMoving)
        {
            float timer = attackMoveTimer.GetRemainingTime() / 0.1f;
            Vector3 movePos = Vector3.Lerp(_currentPos, targetPos, 1 - timer);
            attackMovePos = movePos;   //이동 위치 할당

            CheckUse2();  //공격이동 중, 계속 공격처리

            if (targets.Count > 0)  //피격 대상이 있을 경우, 공격 이동 종료
                attackMoveTimer.Reset();
        }

        if (isAttacking && !isDetected)  //공격 중, 적이 감지되지 않을 시, 공격 이동 시작
        {
            attackMoveTimer.Start();
            targetPos = _currentPos + attackDirection * moveDistance;
        }
    }

    public override void CheckUse(Vector3 currentPos)
    {
        _currentPos = currentPos;

        if (Input.GetMouseButtonDown(0))  //마우스 클릭시, 공격
        {
            if (PlayerTimeSystem.w_BaseAttackTimer.IsRunning()) return;   //다음 공격 대기 체크 실행중, 리턴

            isAttacking = true;

            ChipsetSoundManager.PlayPlayerAttackSound();                  //사운드 재생
            
            PlayerAniInfo aniInfo = new PlayerAniInfo("isAttacking", AniType.Trrigger, 1f / attackTime);  //공격 애니메이션 실행
            PlayAniMation(aniInfo);

            PlayerTimeSystem.w_BaseAttackTimer.Start();                   //공격 타이머 시작

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //마우스 위치 받기
            Vector3 mousePos = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = hit.point;
            mousePos.y = _currentPos.y;

            attackDirection = (mousePos - _currentPos).normalized * attackDistance;   //플레이어 기준 마우스 방향 얻기

            CheckUse2();
        }
        else
        {
            isAttacking = false;
        }
    }

    private void CheckUse2()
    {
        isDetected = false;
        targets.Clear();

        Collider[] enemyCols = Physics.OverlapSphere(_currentPos, attackDistance, enemyLayer);
        foreach (var enemyCol in enemyCols)
        {
            Vector3 dirToEnemy = enemyCol.transform.position - _currentPos;
            dirToEnemy.y = 0f;
            if (Vector3.Angle(attackDirection, dirToEnemy) <= detectAngle / 2f)            //각도내에 적에게만 공격
            {
                isDetected = true;   //적 확인
                targets.Enqueue(enemyCol.gameObject);
                //Debug.Log(enemyCol.gameObject.name + "을 감지했습니다.");
            }
        }
        if(enemyCols.Length > 0) Invoke("Use", readyAttackTime);
    }

    public override void Use()
    {
        if (PlayerTimeSystem.stunTimer.IsRunning()) return;  //플레이어 피격받는 중일 경우, 공격취소 처리 (예외처리)

        foreach(var target in targets)
            chipset.Attack(target, damageRate, attackDirection, addedCritChanceRate, addedCritRate, ChipAttackType.Weapon);

        targets.Clear();
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
