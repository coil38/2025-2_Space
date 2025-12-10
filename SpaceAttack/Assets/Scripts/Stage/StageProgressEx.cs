using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProgressEx : MonoBehaviour
{
    public static StageProgressEx Instance;

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

    public void ClearStage(int stageNumber)
    {
        if (stageNumber > clearedStage)
            clearedStage = stageNumber;

        if (stageNumber == 1)
        {
            UnlockStage(2);
        }

        else if (stageNumber == 2)
        {
            UnlockStage(6);
            clearedStage = 5; 
        }
        else if (stageNumber == 6)
        {

        }
    }


    private void UnlockStage(int targetStage)
    {
        if (targetStage > unlockedStage)
            unlockedStage = targetStage;
    }


    public bool IsStageUnlocked(int stage)
    {
        return stage <= unlockedStage;
    }

    public void LoadProgress(int unlockedStage, int clearedStage)
    {
        this.unlockedStage = unlockedStage;
        this.clearedStage = clearedStage;
    }
}
