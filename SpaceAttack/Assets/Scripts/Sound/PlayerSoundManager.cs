using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static GameObject thisObject;
    public void Initialized()
    {
        thisObject = gameObject;
        SoundManager.instance.RegisterGameObjectByAttribute(gameObject, "Character");
    }
    public static void PlayPlayerMoveSound()
    {
        SoundManager.instance.PlaySound(thisObject, "CharacterMove");  //일반 이동 시작
    }

    public static void StopPlayerMoveSound()
    {
        SoundManager.instance.StopSound(thisObject, "CharacterMove");  //일반 이동 정지
    }

    public static void PlayPlayerHitSound()
    {
        SoundManager.instance.PlaySound(thisObject, "CharacterHit");   //일반 피격
    }

    public static void PlayPlayerDashSound()
    {
        SoundManager.instance.PlaySound(thisObject, "CharacterDash3");   // 일반 대쉬
    }

    public static void PlayPlayerDeadSound()
    {
        SoundManager.instance.PlaySound(thisObject, "CharacterDead");   // 일반 사망
    }
    public static void PlayPlayerLevelUP()
    {
        SoundManager.instance.PlaySound(thisObject, "coreLevelUp");   // 일반 레벨업
    }

    //---------------------------------------------------------칩셋 관련 사운드--------------------------------------------
    public static void PlaySwordBaseAttack()
    {
        SoundManager.instance.PlaySound(thisObject, "SwordBasicAttack");   // 전사 기본공격
    }
    public static void PlaySwordSkillHit()
    {
        SoundManager.instance.PlaySound(thisObject, "SwordSkillHit_1");   // 전사 스킬 피격
    }
    public static void PlaySwordSkill2()
    {
        SoundManager.instance.PlaySound(thisObject, "SwordSkill_2");   // 전사 참격
    }
    public static void PlaySword1Dash()
    {
        SoundManager.instance.PlaySound(thisObject, "SwordSkill1Dash");   // 전사 스킬 1 돌진
    }
    public static void PlaySwordSkill3()
    {
        SoundManager.instance.PlaySound(thisObject, "SwordSkill3Steam");   // 전사 스킬 3
    }
    public static void PlayBowBaseAttack()
    {
        SoundManager.instance.PlaySound(thisObject, "BowSkillandBasicAttack");   // 궁수 기본 공격
    }
    public static void PlayBowSkill2()
    {
        SoundManager.instance.PlaySound(thisObject, "BowSkill2");   // 궁수 스킬2
    }
    public static void PlayBowSkill3()
    {
        SoundManager.instance.PlaySound(thisObject, "BowSkill3");   // 궁수 스킬3
    }
}
