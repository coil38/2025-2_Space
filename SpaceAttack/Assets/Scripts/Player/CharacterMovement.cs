using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private PlayerStatus playerState;
    private InventoryManager inventory;
    private Rigidbody rb;
    private Animator animator;

    private Vector3 currentDir = Vector3.zero;

    private bool isMoving;
    private LayerMask wallLayer;
    private LayerMask itemLayer;

    //대쉬용 변수
    private bool startDash;
    private Vector3 currentPos;
    private Vector3 targetPos;
    private float dashDur;          //예외처리용
    private void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<InventoryManager>();

        wallLayer |= 1 << LayerMask.NameToLayer("Wall");
        itemLayer |= 1 << LayerMask.NameToLayer("Item");
    }

    private void FixedUpdate()
    {
        if (!startDash) return;
        PlayerDash();
    }

    public void Move()  //플레이어 이동
    {
        float horizotal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(horizotal, 0, vertical).normalized;

        if (dir != Vector3.zero) currentDir = dir.normalized;  //현재 방향값이 0이 아닐 때만 전달
        else
        {
            Invoke("ChangeCurrentDir", 0.1f);  //0.1초 실행
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

        animator.SetBool("IsDashing", true);

        dashDur = playerState.m_DashDruation;

        if (Physics.Raycast(transform.position, currentDir, out RaycastHit hit, dashDur, wallLayer))   //벽이 있을 경우의 예외처리(이동거리, 이동시간)
        {
            dashDur = Vector3.Distance(hit.point, transform.position) - 0.55f;
            dashDur = Mathf.Max(dashDur, 0f);
        }

        Debug.Log(dashDur);

        TimeSystem.w_dashTimer.Start();   //대쉬 대기 시간(0.15 초 동안)
        TimeSystem.deshTimer.Start();     //대쉬 타이머 시작  (0.1 초 동안)

        SetDashInfo();     //대쉬 위치 설정
        if(dashDur > 0) startDash = true;  //대쉬 시작
    }
    private void SetDashInfo()
    {
        currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        targetPos = currentPos + currentDir.normalized * dashDur;
    }
    private void PlayerDash()
    {
        Timer dashTimer = TimeSystem.deshTimer;
        float dashTime = TimeSystem.m_DashTime;

        float timer = dashTimer.GetRemainingTimer() / dashTime;

        Vector3 move = Vector3.Lerp(currentPos, targetPos, 1 - timer);
        rb.MovePosition(move);

        if (timer <= 0.1f) startDash = false;
    }

    private void ChangeCurrentDir()
    {
        currentDir = - Vector3.forward;
    }

    public void CheckItem()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, playerState.itemDetectDistance, itemLayer);

        foreach (var item in items)
        {
            ChipSetType chipset = item.gameObject.GetComponent<ChipSetType>();

            if (chipset != null) // 감지 대상이 칩셋이면 칩셋받기
            {
                inventory.chipSet = chipset;
                Debug.Log("아이템 획득");

                break;                  //획득 종료(한번에 하나만 획득)
            }
        }
    }
}
