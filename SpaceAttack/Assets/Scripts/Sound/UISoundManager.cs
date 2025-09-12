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

    public static void PlayeButtonClick()  //버튼 클릭 사운드 재생
    {
        SoundManager.instance.PlayBGMOrUISound(4003, SoundType.UI);
    }
    public static void PlayeOnAndOffPanel()  //버튼 클릭 사운드 재생
    {
        SoundManager.instance.PlayBGMOrUISound(4004, SoundType.UI);
    }
}
