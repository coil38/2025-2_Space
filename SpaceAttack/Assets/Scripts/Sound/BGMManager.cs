using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    private Scene currentScene;

    public void Initialized()  //사운드 등록용 초기설정
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.sceneLoaded += OnSceneLoaded;    //씬로드 이벤트 구독

        //모든 BGM구독처리
        SoundManager.instance.RegisterGameObjectBySoundType(gameObject, SoundType.BGM);

        PlayBGMSound(currentScene);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;    //씬로드 이벤트 구독 헤제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        currentScene = scene;
        PlayBGMSound(scene);
        //LogUtil.Log("작동한다");
    }

    private void PlayBGMSound(Scene scene)
    {
        switch (scene.name)
        {
            case "StartUIScene":
                SoundManager.instance.StopPlayedAllSound();  //모든 사운드 종료
                //시작화면 브금 재생
                break;
            case "LobbyScene":
                //SoundManager.instance.PlayBGMOrUISound(1004, SoundType.BGM);
                break;

            case "ChipsetSelectScene":
                SoundManager.instance.PlayBGMOrUISound(1004, SoundType.BGM);
                break;

            case "BattleScene":
                SoundManager.instance.PlayBGMOrUISound(1001, SoundType.BGM);
                break;
        }
    }
}
