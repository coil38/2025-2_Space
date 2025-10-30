using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum UIType
{
    StartSceneUI,     //시작화면UI
    PauseUI,          //일시정지UI
    SelectChipsetUI,  //칩셋선택UI
    SelectStageUI,    //스테이지 선택UI
    None,             //잘모를 때, 쓰는 타입(자동으로 알맞은 타입으로 바꿔줌)
}

public class UIESCSystem : MonoBehaviour  //ESC교통정리용 시스템
{
    private static UIType currentUIType;
    private static Stack<(GameObject,UnityAction)> depths = new Stack<(GameObject, UnityAction)>();
    
    private static UnityAction onPauseUI;

    private Timer ESCPauseTimer = new Timer(1f);

    private void Awake()
    {
        GameSceneManager.sceneTypeChanged += ChangeUITypeByScene;    //씬타입 변경 이벤트 구독
    }

    private void OnDisable()
    {
        GameSceneManager.sceneTypeChanged -= ChangeUITypeByScene;    //씬타입 변경 이벤트 구독 헤제
    }

    private static void ChangeUITypeByScene(SceneType sceneType)          //씬에 따라서 UIType을 바꾸어주는 함수
    {
        if (sceneType == SceneType.StartGameScene)
        {
            ChangeUIType(UIType.StartSceneUI);
        }
        else
        {
            ChangeUIType(UIType.PauseUI);            //칩셋선택UI가 아닐경우,
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CleanDepth();

            switch (currentUIType)
            {
                case UIType.StartSceneUI:
                    //LogUtil.Log("시작UI임");
                    Return();
                break;

                case UIType.SelectChipsetUI:
                    //LogUtil.Log("칩셋선택UI임");
                    Return();
                break;

                case UIType.PauseUI:
                    //LogUtil.Log("일시정지UI임");
                    if (depths.Count <= 0)
                    {
                        onPauseUI?.Invoke();
                        ESCPauseTimer.Start();
                    }
                    else if  (depths.Count == 1 && !ESCPauseTimer.IsRunning())  //일시정지 시작 ~ 취소 사이 텀을 둔다
                    {
                        Return();
                    }
                    else Return();
                break;
            }
        }

        ESCPauseTimer.Update();
    }

    public static void ChangeUIType(UIType type)
    {
        //LogUtil.Log($"UI타입이 {type}으로 변경됨");
        depths.Clear();                               //초기화

        if (type == UIType.None)
            ChangeUITypeByScene(GameSceneManager.instance.currentScene);
        else currentUIType = type;
    }

    public static void SetUIDepth(UIType type, UnityAction action, GameObject obj)
    {
        if (currentUIType != type) return;   //본인 타입이 아닐 경우, 반환처리
        if (depths.TryPeek(out var result))
            if (result.Item1 == obj) return; //제일 위의 오브젝트와 같을 경우, 반환처리
        depths.Push((obj, action));

        //LogUtil.Log($"{obj.name}가 뎁스에 추가됨");
        //DebugFuc();
    }

    private static void DebugFuc()
    {
        LogUtil.Log($"11---------------------------------------------------11");
        foreach (var depth in depths)
        {
            LogUtil.Log($"{depth.Item1.name}가 뎁스에 있음");
        }
        LogUtil.Log($"33---------------------------------------------------33");
    }

    private static void DebugFuc2(Stack<(GameObject, UnityAction)> temps)
    {
        LogUtil.Log($"11----------------------222222222-----------------------------11");
        foreach (var depth in temps)
        {
            LogUtil.Log($"{depth.Item1.name}가 뎁스에 있음");
        }
        LogUtil.Log($"33---------------------22222222222-----------------------------33");
    }

    private void Return()
    {
        if (depths.Count <= 0) return;
        GameObject obj = depths.Peek().Item1;
        UnityAction action = depths.Peek().Item2;
        action?.Invoke();

        //LogUtil.Log($"{obj.name}가 뎁스에서 나감");
        //DebugFuc();
    }

    private void CleanDepth()    //뎁스 청소용
    {
        bool HaveToClean = false;   //청소 필요 여부 체크
        Stack<(GameObject, UnityAction)> temp = new Stack<(GameObject, UnityAction)>();

        foreach (var depth in depths)
        {
            if (depth.Item1.activeSelf)
            {
                temp.Push(depth);
            }
            else HaveToClean = true;   //비활성화가 하나라도 있으면 청소할 필요 있음
        }

        if (HaveToClean)
        {
            //DebugFuc2(temp);
            depths = new Stack<(GameObject, UnityAction)>(temp);
            //DebugFuc2(depths);
        }
    }

    public static void SetPauseUI(UnityAction action)   //일시정지UI 전용 - UI활성화용 함수
    {
        onPauseUI = action;
    }

    public static UIType GetCurrentUIType()             //UI타입 반환 함수
    {
        return currentUIType;
    }
}
