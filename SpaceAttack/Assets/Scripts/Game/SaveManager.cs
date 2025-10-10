using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public string currentFileName;  //사용할 저장공간 이름

    private InventoryManager playerInventory;

    private string[] blockedScenes = {"StartUIScene", "LoadingScene", "CutScene" };
    private bool isOneTime; //처음 한번만 적용

    public void SaveFile(string filename)
    {
        currentFileName = filename;   //초반에 한번 사용후, 저장
        SaveData data = new SaveData();

        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<InventoryManager>();
        }

        if (playerInventory != null)
        {
            data.SetDatas(playerInventory.transform.position, 
                SceneManager.GetActiveScene().name, 
                PlayerCore.Level, 
                PlayerCore.DarkMaterialCount, 
                playerInventory.relicIDs.Count > 0 ? playerInventory.relicIDs.ToArray() : null, 
                playerInventory.chipSet != null ? playerInventory.chipSet.chipSetName : null);
        }
        else
        {
            LogUtil.LogWarning($"저장시스템에서 playerObj와 systemPackObj를 찾을 수 없음");
        }

        SaveSystem.Save(currentFileName, data);  //저장
    }

    public void StartNewSaveFile(string fileName)
    {
        SceneLoadManager.instance.LoadScene("LobbyScene");

        currentFileName = fileName;
        SaveData data = new SaveData();
        SaveSystem.Save(currentFileName, data);  //저장 새 파일 생성

        if (!isOneTime)  //게임 키고 처음 한번만 적용
        {
            StartCoroutine(SaveGame());
            isOneTime = true;
        }
    }

    public void LoadSaveFile(string fileName)
    {
        currentFileName = fileName;
        SaveData data = SaveSystem.Load<SaveData>(currentFileName);  //데이터 로드

        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<InventoryManager>();
        }

        playerInventory.gameObject.transform.position = data.playerPos;  //플레이어 위치 설정
        //foreach (int itemID in data.playerItems)                         //플레이어 유물 설정
        //{

        //}
        //playerInventory.relic =                                        //플레이어 칩셋 설정
        PlayerCore.Level = data.playerLevel;                             //플레이어 레벨 설정
        PlayerCore.DarkMaterialCount = data.playerDarkMatCount;          //플레이어 암흑물질 설정

        SceneLoadManager.instance.LoadScene(data.sceneName);  //씬으로 이동
    }

    public void SaveAndLoadButtonInfo(bool isSaving)
    {
        string fileName = "fileName";
        if (isSaving)
        {
            SaveData saveData = new SaveData();
            saveData.SetFildDatas(StartGameUI.fileDatas);
            SaveSystem.Save(fileName, saveData);

            foreach (var temp in saveData.fileDatas)
            {
                //LogUtil.Log("저장한 데이터 :" + temp);
            }
        }
        else
        {
            SaveData data = SaveSystem.Load<SaveData>(fileName);  //파일 데이터 로드
            if (data == null)
            {
                LogUtil.Log("data가 없음");
                return;
            }

            for (int i = 0; i < StartGameUI.fileDatas.Count; i++)
                StartGameUI.fileDatas[i] = data.fileDatas[i];
        }
    }

    private IEnumerator SaveGame()  //10초마다 자동세이브 + 조건: 씬 로드 중 혹은 시작씬에서는 저장하지 않음.
    {
        while (true)
        {
            bool cannotSave = false;

            if (SceneLoadManager.instance.isSceneLoading)  //로드중인지 체크
                cannotSave = true;

            foreach (var name in blockedScenes)  //씬체크
            {
                if (SceneManager.GetActiveScene().name == name)
                    cannotSave = true;
            }

            if (!cannotSave)
            {
                if (!string.IsNullOrEmpty(currentFileName))
                    SaveFile(currentFileName);

                LogUtil.Log("현재는 자동 저장 중....");
            }
            else
            {
                LogUtil.Log("현재는 자동저장을 할 수 없는 상태입니다.");
            }

            yield return new WaitForSeconds(2f);
        }
    }

}
