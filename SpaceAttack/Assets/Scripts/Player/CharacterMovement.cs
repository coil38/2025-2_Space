using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CharacterMovement : MonoBehaviour
{
    private PlayerStatus playerState;
    private Rigidbody rb;
    private Animator animator;

    private Vector3 currentDir = Vector3.zero;
    private Vector3 velocity;
    private Coroutine currentCor;

    private bool isMoving;
    private void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void Move()  //플레이어 이동
    {
        float horizotal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(horizotal, 0, vertical).normalized;

        if (dir != Vector3.zero) currentDir = dir.normalized;  //현재 방향값이 0이 아닐 때만 전달
        else
        {
            if (currentCor == null)
            {
                currentCor = StartCoroutine(e_ChangeCurrentDir());
            }
        }

        if (dir.magnitude > 0.1f && !isMoving)
        {
            isMoving = true;
            playerState.m_Particle.Play();   //이동하기 시작하면 파티클 재생

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound("Run");
        }
        else if(dir.magnitude < 0.1f && isMoving)
        {
            isMoving = false;
            playerState.m_Particle.Stop();       //이동이 멈추면 파티클 종료

            if (AudioManager.instance != null)
                AudioManager.instance.PauseSound("Run");
        }

        if ((horizotal < 0 && playerState.m_FacingRight) || (horizotal > 0 && !playerState.m_FacingRight)) //(입력 - 좌, 캐릭터 - 오) || (입력 - 우, 캐릭터 - 좌) --> 반전
        {
            //Debug.Log($"실행된다.");
            playerState.Flip();
        }

        animator.SetFloat("Horizontal", horizotal);               //애니메이션 파리미터 전달
        animator.SetFloat("Vertical", vertical);

        rb.MovePosition(rb.position + dir * playerState.m_speed * Time.deltaTime);   //플레이어 이동
    }

    public void Dash()  //대쉬
    {
        isMoving = false;
        playerState.m_Particle.Stop();    //이동이 멈추면 파티클 종료
        playerState.d_Particle.Play();    //대쉬할 경우, 파티클 재생

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PauseSound("Run");
            AudioManager.instance.PlaySound("Dash");
        }

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
        Vector3 currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPos = currentPos + currentDir.normalized * dashDur;

        while (true)
        {
            float timer = dashTimer.GetRemainingTimer() / dashTime;

            Vector3 move = Vector3.Lerp(currentPos, targetPos, 1 - timer);
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
        currentDir = - Vector3.forward;
        currentCor = null;
    }
}
