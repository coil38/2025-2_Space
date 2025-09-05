using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public static int Level {  get; private set; }   //레벨
    public static int DarkMaterialCount;             //경험치량
    private static int maxEXP = 100;                 //최대 경험치량
    private static int nextMaxEXP = 100;                //다음 최대 경험치량
    private static int maxLevel = 0;                 //최대 레벨

    [Header("레벨 데이터베이스")]
    [SerializeField] private LevelDatabaseSO levelDatabase;

    private void OnEnable()  //델리게이트 체인 구독
    {
        EventManager.f_CorrectionValueEvent.levelEventHandler += GetMaxExpValue;

        if (levelDatabase != null) EventManager.f_CorrectionValueEvent.levelDatabase = levelDatabase;  //데이터 베이스 할당
        else Debug.LogError("레벨데이터 베이스가 PlayerCore시스템에 할당되지 않았습니다.");
        EventManager.f_CorrectionValueEvent.FindMaxExpValue(Level);  //현재 레벨의 최대 경험치량 할당 이벤트 실행
        maxLevel = EventManager.f_CorrectionValueEvent.levelDatabase.maxLevel;  //최대 레벨 갱신
        StartCoroutine(C_Initialize()); //초기화
    }

    private void OnDisable()  //델리게이트 체인 구독 해지
    {
        EventManager.f_CorrectionValueEvent.levelEventHandler -= GetMaxExpValue;
    }

    void Update() 
    {
        //테스용
        if (Input.GetKeyDown(KeyCode.Y))  //최대 경험치양 이상의 경험치 얻지 못하게 함 ( 예외처리 )
        {
            GetDarkMatter(15);
        }
    }

    private IEnumerator C_Initialize()
    {
        yield return new WaitUntil(() => PlayerUIManager.instance != null);
        GetDarkMatter(0, true);  //UI 갱신
    }

    public static void GetDarkMatter(int exp, bool isInitial = false)     //암흑물질(경험치) 획득
    {
        if (PlayerUIManager.instance == null)     //예외처리
        {
            Debug.LogError("플레이어 메인 UI를 찾을 수 없습니다.");
            return;
        }

        if (maxLevel <= Level) return;  //최대 레벨에 도달시, 반환 처리 ( 예외 처리 )

        ExpInfo expInfo = new ExpInfo(DarkMaterialCount, DarkMaterialCount + exp, maxEXP, Level, nextMaxEXP, isInitial);  //경험치업에 필요한 정보생성
        PlayerUIManager.instance.UpdatePlayerEXP(expInfo);     //UI 갱신 및 연출 재생

        DarkMaterialCount += exp;

        if (DarkMaterialCount >= maxEXP)                      //레벨업 조건에 만족했을 때
        {
            int remainExpCount = DarkMaterialCount - maxEXP;

            EventManager.f_CorrectionValueEvent.FindMaxExpValue(Level + 1);  //다음 레벨의 최대 경험치량을 찾는 이벤트 실행
            EventManager.f_CorrectionValueEvent.FindCorectionValue(Level + 1);  //레벨 보정 여부 판단 이벤트 실행

            DarkMaterialCount = remainExpCount;

            Level++;  //레벨업

            if (remainExpCount > 0) GetDarkMatter(0);  //남은 경험치 재획득처리
        }

    }

    public void GetMaxExpValue(object obj, FindCorectionValueEvent e)
    {
        maxEXP = e.maxEXP;  //최대 경험치 갱신
        nextMaxEXP = e.nextMaxEXP;  //다음 최대 경험치 갱신
        //Debug.Log($"이벤트 실행됨. 최대경험치량: {e.maxEXP}, 다음 최대경험치량: {e.nextMaxEXP}");
    }
}
