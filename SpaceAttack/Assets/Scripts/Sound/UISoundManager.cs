using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public static void PlayeOnAndOffPanelSound()  //버튼 클릭 사운드 재생
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
}
