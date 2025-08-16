using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CharacterMovement : MonoBehaviour
{
    private PlayerStatus playerState;
    private InventoryManager inventory;
    private Rigidbody rb;
    private PlayerMovementAnimationController movementAniController;

    private Vector3 currentDir = Vector3.zero;

    private bool isMoving;
    private LayerMask wallLayer;
    private LayerMask itemLayer;

    //대쉬용 변수
    private bool startDash;
    private Vector3 currentPos;
    private Vector3 targetPos;
    private float dashDis;          //예외처리용
    private void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody>();
        movementAniController = GetComponent<PlayerMovementAnimationController>();
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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(horizontal, 0, vertical).normalized;

        if (dir != Vector3.zero) currentDir = dir.normalized;  //현재 방향값이 0이 아닐 때만 전달
        else
        {
            Invoke("ChangeCurrentDir", 0.1f);  //0.1초 실행
        }

        if (dir.magnitude > 0.1f && !isMoving)
        {
            isMoving = true;
            playerState.m_Particle.Play();   //이동하기 시작하면 파티클 재생
        }
        else if(dir.magnitude < 0.1f && isMoving)
        {
            isMoving = false;
            playerState.m_Particle.Stop();       //이동이 멈추면 파티클 종료
        }

        movementAniController.UpdateMoveDirection(horizontal, vertical);  //애니메이션 이동방향 갱신
        movementAniController.PlayAnimation("Move", horizontal, vertical); //이동 애니메이션 재생

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

        dashDis = playerState.m_DashDistance;

        movementAniController.PlayAnimation("Dash", 0, 0, PlayerTimeSystem.w_DashTime); //대쉬 애니메이션 재생

        if (Physics.Raycast(transform.position, currentDir, out RaycastHit hit, dashDis, wallLayer))   //벽이 있을 경우의 예외처리(이동거리, 이동시간)
        {
            dashDis = Vector3.Distance(hit.point, transform.position) - 0.55f;
            dashDis = Mathf.Max(dashDis, 0f);
        }

        //Debug.Log(dashDis);

        PlayerTimeSystem.w_dashTimer.Start();   //대쉬 대기 시간(0.15 초 동안)
        PlayerTimeSystem.deshTimer.Start();     //대쉬 타이머 시작  (0.1 초 동안)

        SetDashInfo();     //대쉬 위치 설정
        if(dashDis > 0) startDash = true;  //대쉬 시작
    }
    private void SetDashInfo()
    {
        currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        targetPos = currentPos + currentDir.normalized * dashDis;
    }
    private void PlayerDash()
    {
        Timer dashTimer = PlayerTimeSystem.deshTimer;
        float dashTime = PlayerTimeSystem.m_DashTime;

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
