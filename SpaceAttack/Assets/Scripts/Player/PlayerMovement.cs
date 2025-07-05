using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterMovement characterMovement;
    private PlayerStatus playerState;
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        playerState = GetComponent<PlayerStatus>();
    }

    void Update()            //플레이어 조작조건 및 예외처리
    {
        if (playerState.isDead) return;

        if (!playerState.isDashing && !playerState.isStuned && !playerState.isAttacking)  //대쉬 혹은 스턴 상태에서 이동 안됨
        {
            characterMovement.Move();  //이동

            //공격방식에 따라서 이동 방식변경 (공격--> enum 사용)
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && !playerState.isStuned && !playerState.isDashing && !playerState.isAttacking)   //스턴(피격)중에 대쉬 안됨
        {
            characterMovement.Dash();  //대쉬

            //무기에 따라서 대쉬 가능여부가 나뉨
        }
    }
}
