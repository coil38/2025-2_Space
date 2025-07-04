using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private PlayerStatus playerState;
    private Rigidbody2D rb;
    private Animator animator;

    private bool m_FacingRight = true;
    private Vector2 currentDir = Vector2.zero;
    private Vector3 velocity;

    private void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Move()  //플레이어 이동
    {
        float horizotal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 dir = new Vector2(horizotal, vertical).normalized;

        if (dir != Vector2.zero) currentDir = dir.normalized;  //현재 방향값이 0이 아닐 때만 전달
        else currentDir = Vector2.down;

        if ((horizotal < 0 && m_FacingRight) || (horizotal > 0 && !m_FacingRight)) //(입력 - 좌, 캐릭터 - 오) || (입력 - 우, 캐릭터 - 좌) --> 반전
        {
            Flip();
        }

        animator.SetFloat("Horizontal", horizotal);               //애니메이션 파리미터 전달
        animator.SetFloat("Vertical", vertical);

        rb.MovePosition(rb.position + dir * playerState.m_speed * Time.deltaTime);   //플레이어 이동

    }

    private void Flip()  //좌우반전 로직
    {
        m_FacingRight = !m_FacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void Dash()  //대쉬
    {
        Debug.Log("실행된다");

        animator.SetBool("IsDashing", true);
        rb.velocity = currentDir * playerState.m_DashForce;   //바라보는 방향으로 순간 이동
    }

    private IEnumerator PlayerDash()
    {
        yield return null;
    }
}
