using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //플레이어 관련 데이터
    public Vector3 playerPos;       //플레이어 위치
    public int[] playerItems;       //플레이어 유물아이템 배열
    public string playerChipsetName;    //플레이어 칩셋 stringKey값
    public int playerLevel;         //플레이어 레벨
    public int playerDarkMatCount;  //플레이어 경험치

    public string sceneName;        //씬 이름

    //스테이지 관련 데이터
    public int unlockedStage;   //해금 안됨 스테이지
    public int clearedStage;    //클리어한 최대 스테이지

    public void SetPlayerDatas(Vector3 playerPos, string sceneName, int playerLevel, int playerDarMatCount, int[] playerItems = null, string playerChipset = null)
    {
        this.playerPos = playerPos;
        this.sceneName = sceneName;
        this.playerLevel = playerLevel;
        this.playerDarkMatCount = playerDarMatCount;

        if (playerItems != null) this.playerItems = playerItems;
        if (playerChipset != null) this.playerChipsetName = playerChipset;
    }

    public void SetStageDates(int unlockedStage, int clearedStage)
    {
        this.unlockedStage = unlockedStage;
        this.clearedStage = clearedStage;
    }
}

[System.Serializable]
public class SavedButtonData
{
    public string[] fileDatas = new string[3];      //버튼 저장
    public void SetFildDatas(Dictionary<int, string> fileDatas)
    {
        foreach (var file in fileDatas)
        {
            this.fileDatas[file.Key] = file.Value;
        }
    }
}