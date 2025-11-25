using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProgress : MonoBehaviour
{
    public static StageProgress Instance;

    [Header("현재 해금된 최고 스테이지")]
    public int unlockedStage = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsStageUnlocked(int stage)
    {
        return stage <= unlockedStage;
    }

    public void UnlockNextStage()
    {
        unlockedStage++;
        if (unlockedStage > 5) unlockedStage = 5;

        PlayerPrefs.SetInt("UnlockedStage", unlockedStage);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        unlockedStage = PlayerPrefs.GetInt("UnlockedStage", 1);
    }
}
