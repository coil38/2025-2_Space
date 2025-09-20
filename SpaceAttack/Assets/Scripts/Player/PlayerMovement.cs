using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterMovement characterMovement;
    private PlayerStatus playerState;

    private bool isAttacking;
    private bool isUsingSkill;

    private bool isOneTime;
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        playerState = GetComponent<PlayerStatus>();
    }

    void Update()            //플레이어 조작조건 및 예외처리
    {
        if (playerState.isDead || Time.timeScale == 0) return;

        if (!playerState.isDashing && !PlayerTimeSystem.stunTimer.IsRunning())  //대쉬 혹은 스턴 상태에서 이동 안됨
        {
            isAttacking = false;
            isUsingSkill = false;

            if (PlayerTimeSystem.w_BaseAttackTimer != null)
                if (PlayerTimeSystem.w_BaseAttackTimer.IsRunning()) isAttacking = true;

            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) isUsingSkill = true;


            if (!isAttacking && !isUsingSkill) characterMovement.Move();  //이동

            //공격방식에 따라서 이동 방식변경 (공격--> enum 사용)
        }
        
        if (PlayerInputController.dashAction.triggered && !PlayerTimeSystem.stunTimer.IsRunning())//스턴(피격)중에 대쉬 안됨
        {
            isAttacking = false;
            isUsingSkill = false;

            if (PlayerTimeSystem.w_BaseAttackTimer != null)
                if (PlayerTimeSystem.w_BaseAttackTimer.IsRunning()) isAttacking = true;

            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) isUsingSkill = true;

            if (PlayerTimeSystem.c_dashTimer != null)
            {
                if (!PlayerTimeSystem.c_dashTimer.IsRunning())
                {
                    if (!isAttacking && !isUsingSkill) characterMovement.Dash();  //대쉬
                }
            }
        }

        if (PlayerInputController.interactionAction.triggered)  //아이템 줍기
        {
            characterMovement.CheckItem();
            characterMovement.CheckInteraction();
        }

        if (characterMovement.isRelicNearByPlayer())
        {
            BaseRelic currentRelic = characterMovement.GetRelic();
            if (currentRelic != null) isOneTime = true;
            else
            {
                LogUtil.LogError("감지된 유물이 없습니다.");
                return;
            }

            characterMovement.CheckChangeRelicPopUI(currentRelic);      //팝업UI교체 체크용
            characterMovement.SetRelicFloatingText(true, currentRelic); //플로팅 텍스트 활성화

            if (PlayerInputController.interactionAction.triggered)
            {
                characterMovement.AquireRelic(currentRelic);       //획득처리
            }
            else if (PlayerInputController.subInteractionAction.triggered)
            {
                characterMovement.SetRelicPopUpUI(false, currentRelic);        //팝업창 활성화 및 비활성화
            }
        }
        else
        {
            if (isOneTime)
            {
                isOneTime = false;
                characterMovement.SetRelicFloatingText(false);          //플로팅 텍스트 비활성화
                characterMovement.SetRelicPopUpUI(true);                //팝업창 비활성화
            }
        }
    }
}
