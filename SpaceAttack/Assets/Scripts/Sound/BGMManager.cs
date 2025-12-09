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
        //AudioSource source = GetComponent<AudioSource>();
        //if (source != null) LogUtil.Log("오디오 소스가 존재한다.");
        //else LogUtil.Log("오디오 소스가 존재하지 않는다.");

        switch (sceneType)
        {
            case SceneType.StartGameScene:
                //if(SoundManager.instance != null) 
                 SoundManager.instance.StopPlayedAllSound();  //모든 사운드 종료
                //SoundManager.instance.PlayBGMOrUISound(1004, SoundType.BGM);
                //시작화면 브금 재생
                break;
            case SceneType.LobbyScene:
                //SoundManager.instance.StopAllPlayedBGM();     //이전의 모든 브금 종료
                //SoundManager.instance.PlayBGMOrUISound(1004, SoundType.BGM);
                break;

            case SceneType.ChipsetSelectScene:
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.StopAllPlayedBGM();     //이전의 모든 브금 종료
                    PlayChipsetSelectSound();
                }
                break;

            case SceneType.BattleScene:
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.StopAllPlayedBGM();     //이전의 모든 브금 종료
                    PlayBattleSound();
                }
                break;

            case SceneType.MiddleBossScene:
                if (SoundManager.instance != null)
                    SoundManager.instance.StopAllPlayedBGM();     //이전의 모든 브금 종료
                break;

            case SceneType.BattleTestNormalScene:
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.StopAllPlayedBGM();     //이전의 모든 브금 종료
                    SoundManager.instance.PlayBGMOrUISound(1001, SoundType.BGM);
                }
                break;
            case SceneType.BattleTestScene:
                if (SoundManager.instance != null)
                    SoundManager.instance.StopAllPlayedBGM();     //이전의 모든 브금 종료
                break;
        }
    }

    public static void PlayMiddleBossSound()        //중간 보스1
    {
        if (SoundManager.instance != null) SoundManager.instance.PlayBGMOrUISound(1011, SoundType.BGM);
    }

    public static void PlayMiddleBossFildSound()     //중간 보스2
    {
        if (SoundManager.instance != null) SoundManager.instance.PlayBGMOrUISound(1007, SoundType.BGM);
    }
    public static void PlayBattleSound()           //전투 사운드
    {
        if (SoundManager.instance != null) SoundManager.instance.PlayBGMOrUISound(1001, SoundType.BGM);
    }

    public static void PlayChipsetSelectSound()     //칩셋 선택씬 배경
    {
        if (SoundManager.instance != null) SoundManager.instance.PlayBGMOrUISound(1004, SoundType.BGM);
    }
    public static void PlayGameOver()     //게임 오버 배경
    {
        if (SoundManager.instance != null) SoundManager.instance.PlayBGMOrUISound(1006, SoundType.BGM);
    }

    public static void PlaySaveZone()     //안전 구역 배경
    {
        SoundManager.instance.StopAllPlayedBGM();     //재생 중인 브금을 끔 (예외)
        if (SoundManager.instance != null) SoundManager.instance.PlayBGMOrUISound(1012, SoundType.BGM);
    }

    public static void PlayTutorial()     //튜토리얼
    {

    }
    public static void PlayStartScene()     //시작화면
    {

    }
}
