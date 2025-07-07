using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private PlayerStatus playerState;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 currentDir = Vector2.zero;
    private Vector3 velocity;
    private Coroutine currentCor;

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
        else
        {
            if (currentCor == null)
            {
                currentCor = StartCoroutine(e_ChangeCurrentDir());
            }
        }

        if ((horizotal < 0 && playerState.m_FacingRight) || (horizotal > 0 && !playerState.m_FacingRight)) //(입력 - 좌, 캐릭터 - 오) || (입력 - 우, 캐릭터 - 좌) --> 반전
        {
            Debug.Log($"실행된다.");
            playerState.Flip();
        }

        animator.SetFloat("Horizontal", horizotal);               //애니메이션 파리미터 전달
        animator.SetFloat("Vertical", vertical);

        rb.MovePosition(rb.position + dir * playerState.m_speed * Time.deltaTime);   //플레이어 이동

    }

    public void Dash()  //대쉬
    {

        TimeSystem.w_dashTimer.Start();   //대쉬 타이머 시작 (0.15 초 동안)
        TimeSystem.invincibilityTimer.Start();  //대쉬 후, 잠시동안 무적 시작 (0.1 동안)
        TimeSystem.deshTimer.Start();     //대쉬 대기 시간 (0.1 초 동안)

        animator.SetBool("IsDashing", true);
        StartCoroutine(PlayerDash());
    }

    private IEnumerator PlayerDash()
    {
        Timer dashTimer = TimeSystem.deshTimer;
        float dashTime = TimeSystem.m_DashTime;
        float dashDur = playerState.m_DashDruation;
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPos = currentPos + currentDir.normalized * dashDur;

        while (true)
        {
            float timer = dashTimer.GetRemainingTimer() / dashTime;

            Vector2 move = Vector2.Lerp(currentPos, targetPos, 1 - timer);
            rb.MovePosition(move);

            if (timer <= 0.1f)
            {
                break;
            }
            yield return null;
        }
    }

    private IEnumerator e_ChangeCurrentDir()
    {
        yield return new WaitForSeconds(0.1f);
        currentDir = Vector2.down;
        currentCor = null;
    }
}
