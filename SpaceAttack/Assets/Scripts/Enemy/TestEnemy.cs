using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TestEnemy : MonoBehaviour
{
    public float hp = 10f;
    public float attackDistance = 2f;
    public float detectAngle = 155f;
    public float damage = 1f;
    public float attackDuration = 0.5f;

    private Rigidbody rb;
    private Animator animator;

    private bool isDead;

    private Vector3 _currentPos;
    private Vector3 attackDirection;
    private LayerMask playerLayer;

    private bool isHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        playerLayer |= 1 << LayerMask.NameToLayer("Player");

        StartCoroutine(EnemyPatern());
    }

    void Update()
    {

    }

    private IEnumerator EnemyPatern()
    {
        float timer = 0f;
        float timer2 = 0f;
        float attackTime = attackDuration;

        while (true)
        {
            timer += Time.deltaTime / attackTime;
            if (timer > 1 && !isHit)
            {
                CheckAttack();
                timer = 0f;
            }

            if (isHit)
            {
                timer2 += Time.deltaTime / 0.5f;
                if (timer2 > 1)
                {
                    isHit = false;
                    timer2 = 0f;
                }
            }
            yield return null;
        }
    }

    public void CheckAttack()
    {
        _currentPos = transform.position;

        animator.SetBool("IsAttacking", true);      //공격 애니메이션 실행

        Vector3 mousePos = - transform.right;

        mousePos.y = _currentPos.y;

        Vector3 attackDir = (mousePos - _currentPos).normalized * attackDistance;   //플레이어 기준 마우스 방향 얻기

        attackDirection = attackDir;

        Attack();
    }

    public void Attack()
    {
        Collider[] playerCols = Physics.OverlapSphere(_currentPos, attackDistance, playerLayer);
        foreach (var playerCol in playerCols)
        {

            Vector3 dirToEnemy = playerCol.transform.position - _currentPos;
            dirToEnemy.y = 0f;
            if (Vector3.Angle(attackDirection, dirToEnemy) <= detectAngle / 2f)            //각도내에 적에게만 공격
            {
                playerCol.SendMessage("ApplyDamage", damage);
            }
        }
    }

    public void ApplyDamage(AttackInfo attackInfo)
    {
        if(isDead) return;                          //사망 이후 피격 안됨 처리

        isHit = true;

        float damage = attackInfo.damage;
        Vector2 dir = attackInfo.attackDirection;

        hp -= damage;
        if (hp <= 0)    //몬스터 사망처리
        {
            isDead = true;
            animator.SetBool("Dead", true);
            rb.AddForce(dir, ForceMode.Impulse);        //넉백 연산 없음(임시)
            Destroy(gameObject, 1f);
        }
        else
        {
            animator.SetTrigger("Hit");                          //피격 애니메이션
            rb.AddForce(dir * 0.5f, ForceMode.Impulse);        //넉백 연산 없음(임시)
        }
    }
}
