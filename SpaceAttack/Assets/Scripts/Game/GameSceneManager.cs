using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    None,
    StartGameScene,
    LobbyScene,
    ChipsetSelectScene,
    BattleScene
}

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;
    public static Action<SceneType> sceneTypeChanged;
    public SceneType currentScene {  get; private set; }
    public InitializationOrderer parent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            currentScene = GetSceneType(SceneManager.GetActiveScene());    //씬 타입 설정
            SceneManager.sceneLoaded += OnSceneLoaded;
            StartCoroutine(WaitUtilInitializeDon());
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;    //씬로드 이벤트 구독 헤제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)  //두번 실행되는 경우가 있음
    {
        if (InitializationOrderer.instance != parent) return;

        SceneType sceneType = GetSceneType(scene);            //씬 타입 변경 (씬이동시)

        if (sceneType != currentScene)                        //씬 타입 바뀌었을 경우, 씬 변경 이벤트 실행
            sceneTypeChanged?.Invoke(sceneType);

        if (currentScene == SceneType.StartGameScene && currentScene != sceneType)  //시작씬에서 다른 씬으로 이동할 경우
        {
            Time.timeScale = 1f;                              //일시정지 해제
            LogUtil.Log("시작씬 일시정지 취소ㅗㅗㅗㅗㅗㅗㅗㅗㅗㅗㅗ");
        }
        else if (sceneType == SceneType.StartGameScene && currentScene != sceneType) //다른씬에서 시작씬으로 이동한 경우
        {
            Time.timeScale = 0f;                              //일시정지
            LogUtil.Log("시작씬 일시정지!!!!!!!!!!!!!!!!!!!!!!");
        }

        currentScene = sceneType;
    }

    private SceneType GetSceneType(Scene scene)
    {
        switch (scene.name)
        {
            case "StartUIScene":
                LogUtil.Log("게임시작씬");
                return SceneType.StartGameScene;

            case "LobbyScene":
                LogUtil.Log("로비씬");
                return SceneType.LobbyScene;

            case "ChipsetSelectScene":
                LogUtil.Log("칩셋선택씬");
                return SceneType.ChipsetSelectScene;

            case "BattleScene":
                LogUtil.Log("전투씬");
                return SceneType.BattleScene;
        }
        LogUtil.LogWarning("알맞은 씬을 찾을 수 없습니다.");
        return SceneType.None;
    }

    public static string GetSceneNameByType(SceneType type)
    {
        switch (type)
        {
            case SceneType.StartGameScene: return "StartUIScene";
            case SceneType.LobbyScene: return "LobbyScene";
            case SceneType.ChipsetSelectScene: return "ChipsetSelectScene";
            case SceneType.BattleScene: return "BattleScene";
        }
        return "";
    }

    private IEnumerator WaitUtilInitializeDon()   //처음 한번 게임시작씬에서 일시정지 처리
    {
        yield return new WaitUntil(() => InitializationOrderer.instance != null);
        yield return new WaitUntil(() => InitializationOrderer.instance.isInitializeDon);
        LogUtil.Log("초기화 종료");
        if (currentScene == SceneType.StartGameScene)
        {
            Time.timeScale = 0f;
        }
    }
}
