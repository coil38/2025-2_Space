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

    private string[] blockedScenes = {"StartUIScene", "LoadingScene", "BattleTestScene" };
    private bool isOneTime; //처음 한번만 적용

    public void SaveFile(string filename)  //생성된 파일 저장용 함수
    {
        currentFileName = filename;   //초반에 한번 사용후, 저장
        SaveData data = new SaveData();

        if (playerInventory == null)
            playerInventory = FindObjectOfType<InventoryManager>();

        if (playerInventory != null)
        {
            int[] relicIds = new int[playerInventory._relics.Length];  //인벤토리의 유물들의 아이디들을 받기
            for(int i = 0; i < playerInventory._relics.Length; i++)
                relicIds[i] = playerInventory._relics[i].relicID;

            data.SetDatas(playerInventory.transform.position, 
                SceneManager.GetActiveScene().name, 
                PlayerCore.Level, 
                PlayerCore.DarkMaterialCount, 
                relicIds.Length > 0 ? relicIds : null,
                playerInventory.chipSet != null ? playerInventory.chipSet.chipSetName : null);
        }
        else
        {
            LogUtil.LogWarning($"저장시스템에서 playerObj와 systemPackObj를 찾을 수 없음");
        }

        SaveSystem.Save(currentFileName, data);  //저장
    }

    public void StartNewSaveFile(string fileName)  //새 파일 생성용 함수
    {
        string sceneName = GameSceneManager.GetSceneNameByType(SceneType.TutorialScene);
        InitializePlayerDatas();

        currentFileName = fileName;
        SaveData data = new SaveData();
        data.SetDatas(new Vector3(0f, 0.092f, -0.3f), sceneName, 0, 0);
        SaveSystem.Save(currentFileName, data);  //저장 새 파일 생성

        SceneLoadManager.instance.LoadScene(sceneName);
    }

    public void PlayerReset()             //플레이어 사망 후, 초기화 함수
    {
        LogUtil.Log($"플레이어 사망, 새로하기 파일 이름: {currentFileName}");
        StartNewSaveFile(currentFileName);
    }

    public void InitializePlayerDatas()  //게임 시작 전의 값으로 초기화
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<InventoryManager>();

        if (playerInventory != null)
        {
            Vector3 defualtPos = new Vector3(0f, 0.092f, -0.3f);   //플레이어 위치초기화
            playerInventory.transform.position = defualtPos;

            if (SoundManager.instance != null)
                SoundManager.instance.StopPlayedAllSound();        //모든 사운드 정지
            DataManager.instance.InitializePlayerStatus();         //플레이어 데이터 초기화
            playerInventory.InitialInventoryDatas();               //유물리스트 삭제 및 암흑물질량 초기화 및 칩셋제거
            if (PlayerUIManager.instance != null) 
                PlayerUIManager.instance.ResetHpUI();              //체력UI 갱신

            //플레이어 대쉬 쿨타임 초기화
            PlayerCore.Level = 0;                                  //레벨 초기화
            PlayerCore.DarkMaterialCount = 0;                      //경험치양 초기화
            PlayerCore.GetDarkMatter(0, true);                     //UI초기화
        }
    }

    public void LoadSaveFile(string fileName)   //저장된 파일 불러오기용 함수
    {
        currentFileName = fileName;
        SaveData data = SaveSystem.Load<SaveData>(currentFileName);  //데이터 로드

        InitializePlayerDatas();   //초기화

        if (playerInventory == null)
            playerInventory = FindObjectOfType<InventoryManager>();

        playerInventory.gameObject.transform.position = data.playerPos;  //플레이어 위치 설정

        RelicSO[] relics = new RelicSO[data.playerItems.Length];          //플레이어 유물 설정
        for (int i = 0; i < data.playerItems.Length; i++)
            relics[i] = DataManager.instance._RelicDatabase.GetRelicById(data.playerItems[i]);
        playerInventory.SetSavedRelics(relics);

        if(!string.IsNullOrEmpty(data.playerChipsetName))
        {
            GameObject chipsetPrf = DataManager.instance.GetChipsetPrfByName(data.playerChipsetName);  //플레이어 칩셋 설정
            GameObject chipset = Instantiate(chipsetPrf);
            playerInventory.chipSet = chipset.GetComponent<BaseChipset>();
        }

        PlayerCore.Level = data.playerLevel;                             //플레이어 레벨 설정
        for (int level = 1; level <= PlayerCore.Level; level++)
        {
            EventManager.playerEvent.FindCorectionValue(level);          //모든 레벨업 보정 재적용
        }
        PlayerCore.GetDarkMatter(0, true);                               //UI초기화

        SceneLoadManager.instance.LoadScene(data.sceneName);  //씬으로 이동
    }

    public void DeleteFile(string fileName)   //파일 삭제 함수
    {
        SaveSystem.Delete(fileName);   //파일 삭제
    }

    public void SaveAndLoadButtonInfo(bool isSaving)  //시작화면의 버튼 관련 정보 저장 및 불러오기용 함수
    {
        string fileName = "fileName";
        if (isSaving)
        {
            SavedButtonData saveData = new SavedButtonData();
            saveData.SetFildDatas(StartGameUI.fileDatas);
            SaveSystem.Save(fileName, saveData);

            foreach (var temp in saveData.fileDatas)
            {
                //LogUtil.Log("저장한 데이터 :" + temp);
            }
        }
        else
        {
            SavedButtonData data = SaveSystem.Load<SavedButtonData>(fileName);  //파일 데이터 로드
            if (data == null || string.IsNullOrEmpty(data.fileDatas[0]))
            {
                LogUtil.Log("data가 없음");
                return;
            }

            for (int i = 0; i < data.fileDatas.Length; i++)
                StartGameUI.fileDatas[i] = data.fileDatas[i];
        }
    }

    public void StartUpdateSave()
    {
        if (!isOneTime)  //게임 키고 처음 한번만 적용
        {
            StartCoroutine(SaveGame());
            isOneTime = true;
        }
    }

    private IEnumerator SaveGame()  //n초마다 자동세이브 + 조건: 씬 로드 중 혹은 시작씬에서는 저장하지 않음.
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

            yield return new WaitForSeconds(5f);
        }
    }

}
