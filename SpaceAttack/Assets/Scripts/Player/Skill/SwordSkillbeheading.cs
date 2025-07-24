using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillbeheading : SkillType     //시전시간(발사: 애니메이션 후, 실행) O | 공격시간 O | 플레이어 대기시간(쿨타임) O
{
    private LayerMask planLayer;   //바닥감지용 리이어 마스크
    private LayerMask wallLayer;   //벽감지용 레이어 마스크
    private LayerMask enemyLayer;  //적감지용 레이어 마스크

    private Vector3 f_DetectPos;     //기즈모 그리는 용
    private Vector3 f_DetectSize;
    private Quaternion detectRot;

    private Vector3 detectPos;
    private Vector3 detectSize;

    private List<GameObject> targets = new List<GameObject>();

    private Timer _s_AttackTimer;  //초기값인 s_AttackTimer의 복제본
    private WaitForFixedUpdate waitForFixedUpdate;

    public override void OnEnable()
    {
        mass = 1f;
        damage = 3f;
        attackDistance = 4f;
        attackWidth = 4f;
        attackTime = 0.6f;
        r_AttackTime = 0.2f;
        coolTime = 8f;
        coolTimer = new Timer(coolTime);
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

        if (Input.GetKeyDown(KeyCode.E))  //플레이어 입력감지
        {
            if (coolTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound("Attack");
            attackAnimator.SetBool("IsAttacking", true);      //공격 애니메이션 실행

            coolTimer.Start();         //쿨타임 시작

            isAttacking = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
            Vector3 mousePos = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
                mousePos = hit.point;
            mousePos.y = _currentPos.y;

            Vector3 attackDir = (mousePos - _currentPos).normalized;   //플레이어 기준 마우스 방향 얻기
            attackDirection = attackDir;

            Invoke("Attack", r_AttackTime);       //공격 애니메이션 후, 시전시간동안 대기
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void Attack()
    {
        float _attackDistance = attackDistance;
        float _attackTime = attackTime;

        if (Physics.Raycast(_currentPos, attackDirection, out RaycastHit hit, attackDistance, wallLayer))   //벽이 있을 경우의 예외처리(이동거리, 이동시간)
        {
            _attackDistance = Vector3.Distance(hit.point, _currentPos);

            if (attackDistance >= _attackDistance)
            {
                _attackTime *= _attackDistance / attackDistance;
                s_AttackTimer = new Timer(_attackTime);
            }
            else s_AttackTimer = _s_AttackTimer;
        }
        else
        {
            s_AttackTimer = _s_AttackTimer;
        }

        s_AttackTimer.Start();                                   //다음 공격 전 대기 체크 시작

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

        AttackInfo attackInfo = new AttackInfo(damage, attackDirection, mass);   //공격 정보 설정

        //GenerateSpriteImage();

        while (true)
        {
            float timer = s_AttackTimer.GetRemainingTimer() / _attackTime;
            Vector3 movePos = Vector3.Lerp(startPos, targetPos, 1 - timer);

            //if(spriteObj != null)
            //    spriteObj.transform.position = movePos;   //이동 위치 할당
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

        targets.Clear();
    }

    //private void GenerateSpriteImage()
    //{
    //    spriteObj = new GameObject();
    //    SpriteRenderer sprite = spriteObj.gameObject.AddComponent<SpriteRenderer>();

    //    sprite.color = Color.white;
    //    sprite.sprite = generateSprit;
    //    sprite.size = new Vector2(1, 1);
    //    spriteObj.transform.position = new Vector3(f_DetectPos.x, -0.87f, f_DetectPos.z);
    //    spriteObj.transform.localScale = detectSize;
    //    //spriteObj.transform.rotation *= Quaternion.Euler(90, 0, 0);
    //    spriteObj.transform.rotation = detectRot; //Quaternion.Euler(0, 0, detectRot.z);

    //}

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
