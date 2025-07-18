using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerMovement : MonoBehaviour
{
    private CharacterMovement characterMovement;
    private PlayerStatus playerState;

    private bool isAttacking;
    private bool isUsingSkill;
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        playerState = GetComponent<PlayerStatus>();
    }

    void Update()            //플레이어 조작조건 및 예외처리
    {
        if (playerState.isDead) return;

        if (!playerState.isDashing && !TimeSystem.stunTimer.IsRunning())  //대쉬 혹은 스턴 상태에서 이동 안됨
        {
            if (TimeSystem.w_w_AttackTimer != null)
                if (!TimeSystem.w_w_AttackTimer.IsRunning())
                    isAttacking = false;
                else isAttacking = true;
            else isAttacking = false;                            //임시로 활성화 시킴


            if (TimeSystem.s_w_AttackTimer != null)
                if (!TimeSystem.s_w_AttackTimer.IsRunning())
                    isUsingSkill = false;
                else isUsingSkill = true;
            else isUsingSkill = false;                            //임시로 활성화 시킴


            if (!isAttacking && !isUsingSkill) characterMovement.Move();  //이동

            //공격방식에 따라서 이동 방식변경 (공격--> enum 사용)
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && !playerState.isStuned && !playerState.isDashing && !playerState.isAttacking && !playerState.isUsingSkill)//스턴(피격)중에 대쉬 안됨
        {
            characterMovement.Dash();  //대쉬

            //무기에 따라서 대쉬 가능여부가 나뉨
        }

        if (Input.GetKeyDown(KeyCode.F))  //아이템 줍기
        {
            characterMovement.CheckItem();
        }
    }
}
