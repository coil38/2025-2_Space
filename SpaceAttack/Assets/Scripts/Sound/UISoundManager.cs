using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public void Initialized()
    {
        SoundManager.instance.RegisterGameObjectBySoundType(gameObject, SoundType.UI);
    }

    public static void PlayeButtonClickSound()  //버튼 클릭 사운드 재생
    {
        SoundManager.instance.PlayBGMOrUISound(4003, SoundType.UI);
    }
    public static void PlayeOnAndOffPanelSound()  //패널 열고 닫는 사운드 재생
    {
        SoundManager.instance.PlayBGMOrUISound(4004, SoundType.UI);
    }
    public static void PlayEquippedSuccessfulSound() //버튼_장착 성공 사운드 재생
    {
        SoundManager.instance.PlayBGMOrUISound(4001, SoundType.UI);
    }
    public static void PlayEquippedFailed() //버튼_장착 실패 사운드 재생
    { 
        SoundManager.instance.PlayBGMOrUISound(4002, SoundType.UI);
    }

    //--------------------------------------------------------------------오브젝트(플레이어X, 몬스터X)-----------------------------------------------------------------------

    public static void PlaySuccessExchange()     //교환 성공 사운드
    {
        SoundManager.instance.PlayBGMOrUISound(4006, SoundType.UI);
    }

    public static void PlayFailExchange()       //교환 실패 사운드
    {
        SoundManager.instance.PlayBGMOrUISound(4007, SoundType.UI);
    }

    public static void PlayDropItem()      //아이템 드랍 사운드
    {
        SoundManager.instance.PlayBGMOrUISound(4010, SoundType.UI);
    }

    public static void PlayOpenRewardChast()   //상자 열기 사운드
    {
        SoundManager.instance.PlayBGMOrUISound(4011, SoundType.UI);
    }

    public static void PlayDisableRewardChast()    //상자 사라지는 사운드
    {
        SoundManager.instance.PlayBGMOrUISound(4005, SoundType.UI);
    }

}
