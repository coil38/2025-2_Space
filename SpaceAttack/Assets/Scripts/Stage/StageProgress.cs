using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProgress : MonoBehaviour
{
    public static StageProgress Instance;

    [Header("현재 해금된 최고 스테이지")]
    public int unlockedStage = 1;

    [Header("현재까지 클리어한 최고 스테이지")]
    public int clearedStage = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 스테이지를 클리어했을 때 호출
    /// </summary>

    public void ClearStage(int stageNumber)    //unlockedStage, clearedStage 변수를 높여주는 함수
    {
        if (stageNumber > clearedStage)
        {
            clearedStage = stageNumber;
        }

        // 다음 스테이지 해금 처리 (해금은 cleared 기준)
        if (stageNumber >= unlockedStage)
        {
            unlockedStage = stageNumber + 1;
            if (unlockedStage > 6) unlockedStage = 6;   
        }
    }

    public bool IsStageUnlocked(int stage)
    {
        return stage <= unlockedStage;
    }

    public void LoadProgress(int unlockedStage, int clearedStage)    //저장 시스템에서 로드
    {
        this.unlockedStage = unlockedStage;
        this.clearedStage = clearedStage;
    }
}
