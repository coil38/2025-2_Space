using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private LayerMask interationLayer;

    //대쉬용 변수
    private bool startDash;
    private Vector3 currentPos;
    private Vector3 targetPos;
    private float dashDis;          //예외처리용

    //유물용
    private bool isFloatingTextOn;
    private bool isPopUpUIOn;
    private BaseRelic currentRelic;
    private void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody>();
        movementAniController = GetComponent<PlayerMovementAnimationController>();
        inventory = GetComponent<InventoryManager>();

        wallLayer |= 1 << LayerMask.NameToLayer("Wall");
        itemLayer |= 1 << LayerMask.NameToLayer("Item");
        interationLayer |= 1 << LayerMask.NameToLayer("InteractionObj");
    }

    private void FixedUpdate()
    {
        if (!startDash) return;
        PlayerDash();
    }

    public void Move()  //플레이어 이동
    {
        float horizontal = PlayerInputController.horizontalValue;
        float vertical = PlayerInputController.verticalValue;

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

        rb.MovePosition(rb.position + dir * PlayerStatus.m_speed * Time.deltaTime);   //플레이어 이동
    }

    public void Dash()  //대쉬
    {
        isMoving = false;
        playerState.m_Particle.Stop();    //이동이 멈추면 파티클 종료
        playerState.d_Particle.Play();    //대쉬할 경우, 파티클 재생

        PlayerSoundManager.PlayPlayerMoveSound();
        PlayerSoundManager.PlayPlayerDashSound();

        dashDis = PlayerStatus.m_DashDistance;

        movementAniController.PlayAnimation("Dash", 0, 0, PlayerTimeSystem.m_DashTime + 0.5f); //대쉬 애니메이션 재생

        if (Physics.Raycast(transform.position, currentDir, out RaycastHit hit, dashDis, wallLayer))   //벽이 있을 경우의 예외처리(이동거리, 이동시간)
        {
            dashDis = Vector3.Distance(hit.point, transform.position) - 0.55f;
            dashDis = Mathf.Max(dashDis, 0f);
        }

        //LogUtil.Log(dashDis);

        PlayerTimeSystem.c_dashTimer.Start();   //대쉬 쿨타임 시작
        PlayerTimeSystem.w_dashTimer.Start();   //대쉬 대기 시간(0.25 초 동안)
        PlayerTimeSystem.deshTimer.Start();     //대쉬 타이머 시작  (0.2 초 동안)

        SetDashInfo();     //대쉬 위치 설정
        if(dashDis > 0) startDash = true;  //대쉬 시작
    }
    private void SetDashInfo()
    {
        currentPos = transform.position;
        targetPos = currentPos + currentDir.normalized * dashDis;
    }
    private void PlayerDash()
    {
        Timer dashTimer = PlayerTimeSystem.deshTimer;
        float dashTime = PlayerTimeSystem.m_DashTime;

        float timer = dashTimer.GetRemainingTime() / dashTime;

        Vector3 move = Vector3.Lerp(currentPos, targetPos, 1 - timer);
        rb.MovePosition(move);

        if (timer <= 0.1f || PlayerStatus.Instance.isStuned) 
            startDash = false;
    }

    private void ChangeCurrentDir()
    {
        currentDir = - Vector3.forward;
    }

    public void CheckItem()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, playerState.itemDetectDistance, itemLayer);

        float minDistance = 5f;
        float currentDis = 5f;
        Collider selectedItem = null;
        foreach (var item in items)
        {
            minDistance = Mathf.Min(minDistance, Vector3.Distance(transform.position, item.transform.position));
            if (currentDis > minDistance)
            {
                selectedItem = item;
            }

            currentDis = minDistance;
        }

        if (selectedItem == null) return;   //주변에 아이템이 없을 시, 반환처리

        ChipSetType chipset = selectedItem.gameObject.GetComponent<ChipSetType>();

        if (chipset != null) // 감지 대상이 칩셋이면 칩셋받기
        {
            inventory.chipSet = chipset;
            LogUtil.Log("아이템 획득");
        }

        //BaseRelic relic = selectedItem.gameObject.GetComponent<BaseRelic>();
        //if (relic != null)
        //{
        //    inventory.relic = relic;
        //    LogUtil.Log("유물획득");
        //}
    }

    public bool isRelicNearByPlayer()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, playerState.itemDetectDistance, itemLayer);
        return items.Length > 0;
    }

    public BaseRelic GetRelic()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, playerState.itemDetectDistance, itemLayer);

        float minDistance = 5f;
        float currentDis = 5f;
        Collider selectedItem = null;
        foreach (var item in items)
        {
            minDistance = Mathf.Min(minDistance, Vector3.Distance(transform.position, item.transform.position));
            if (currentDis > minDistance)
            {
                selectedItem = item;
            }

            currentDis = minDistance;
        }
        if (selectedItem == null) return null;   //주변에 아이템이 없을 시, 반환처리

        BaseRelic relic = selectedItem.gameObject.GetComponent<BaseRelic>();
        if (relic != null)
        {
            return relic;
        }
        return null;
    }

    public void SetRelicFloatingText(bool onFloatingText, BaseRelic relic = null)
    {
        if (onFloatingText && !isFloatingTextOn || currentRelic != relic)
        {
            isFloatingTextOn = true;
            if (PlayerUIManager.instance != null)
                PlayerUIManager.instance.SetRelicFloatingUI(onFloatingText, relic);

            currentRelic = relic;  //최근 유물로 갱신
            //LogUtil.Log("유물 플로팅 텍스트 활성화");
        }
        else if (!onFloatingText && isFloatingTextOn)
        {
            isFloatingTextOn = false;
            if (PlayerUIManager.instance != null)
                PlayerUIManager.instance.SetRelicFloatingUI(onFloatingText);
            //LogUtil.Log("유물 플로팅 텍스트 비활성화");
        }
    }

    public void SetRelicPopUpUI(bool setUIOff = false, BaseRelic relic = null)
    {
        if (setUIOff) { isPopUpUIOn = true; }

        if (!isPopUpUIOn)
        {
            isPopUpUIOn = true;
            //플로팅 텍스트 활성화
            //LogUtil.Log("팝업UI 활성화");
            if(PlayerUIManager.instance != null)
                PlayerUIManager.instance.SetRelicPopUpUI(true, relic);
        }
        else
        {
            isPopUpUIOn = false;
            //플로팅 텍스트 비활성화
            //LogUtil.Log("팝업UI 비활성화");
            if (PlayerUIManager.instance != null)
                PlayerUIManager.instance.SetRelicPopUpUI(false, relic);
        }
    }

    public void CheckChangeRelicPopUI(BaseRelic relic)
    {
        if (!isPopUpUIOn || currentRelic == relic || relic == null) return;
        //LogUtil.Log("팝업UI 변경");
        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.SetRelicPopUpUI(true, relic);
    }

    public bool IsRelicPopUpUIActive()
    {
        return isPopUpUIOn;
    }

    public void AquireRelic(BaseRelic relic)
    {
        if (relic == null)
        {
            LogUtil.LogError("감지된 유물이 없습니다.");
            return;
        }

        inventory.relic = relic;
    }

    public void CheckInteraction()
    {
        Collider[] interactions = Physics.OverlapSphere(transform.position, playerState.itemDetectDistance, interationLayer);

        foreach (var interaction in interactions)
        {
            ChipsetSelectObject chipsetSelect = interaction.GetComponent<ChipsetSelectObject>();

            if (chipsetSelect != null) chipsetSelect.OnChipsetSelectUI();  //칩셋 선택UI 활성화
        }
    }
}
