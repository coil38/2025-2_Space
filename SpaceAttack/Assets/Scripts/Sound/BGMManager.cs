using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public void Initialized()  //사운드 등록용 초기설정
    {
        GameSceneManager.sceneTypeChanged += PlayBGMSound;    //씬 타입 변경 이벤트 구독
        //모든 BGM구독처리
        SoundManager.instance.RegisterGameObjectBySoundType(gameObject, SoundType.BGM);
        PlayBGMSound(GameSceneManager.instance.currentScene);
    }

    private void OnDisable()
    {
        GameSceneManager.sceneTypeChanged -= PlayBGMSound;    //씬 타입 변경 이벤트 구독 헤제
    }

    private void PlayBGMSound(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.StartGameScene:
                SoundManager.instance.StopPlayedAllSound();  //모든 사운드 종료
                SoundManager.instance.PlayBGMOrUISound(1004, SoundType.BGM);
                //시작화면 브금 재생
                break;
            case SceneType.LobbyScene:
                SoundManager.instance.PlayBGMOrUISound(1004, SoundType.BGM);
                break;

            case SceneType.ChipsetSelectScene:
                SoundManager.instance.PlayBGMOrUISound(1004, SoundType.BGM);
                break;

            case SceneType.BattleScene:
                SoundManager.instance.PlayBGMOrUISound(1001, SoundType.BGM);
                break;
        }
    }
}
