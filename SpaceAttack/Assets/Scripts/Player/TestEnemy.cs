using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public float hp = 10f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void ApplyDamage(AttackInfo attackInfo)
    {
        if(isDead) return;                          //사망 이후 피격 안됨 처리

        float damage = attackInfo.damage;
        Vector2 dir = attackInfo.attackDirection;

        hp -= damage;
        if (hp <= 0)    //몬스터 사망처리
        {
            isDead = true;
            animator.SetBool("Dead", true);
            rb.AddForce(dir, ForceMode2D.Impulse);        //넉백 연산 없음(임시)
            Destroy(gameObject, 1f);
        }
        else
        {
            animator.SetTrigger("Hit");                          //피격 애니메이션
            rb.AddForce(dir * 0.5f, ForceMode2D.Impulse);        //넉백 연산 없음(임시)
        }
    }
}
