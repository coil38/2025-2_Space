using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationOrderer : MonoBehaviour
{
    private static InitializationOrderer instance;

    private DataManager DataManager;
    private EventManager EventManager;
    private PlayerStatus playerStatus;     //EventManger -> f_CorrectionValueEvent 관련
    private PlayerCore playerCore;        //EventManger -> f_CorrectionValueEvent, DataManger -> LevelDatabase 관련
    private SoundManager soundManager;    //DataManager -> soundDatabase관련
    private BGMManager bgmManager;        //SoundManager.instance관련
    private UISoundManager uISoundManager; //SoundManager.instance관련
    private PlayerUIManager playerUIManager; //PlayerUIManager.instance관련

    private void Awake()
    {
        StartCoroutine(InitializeDatas());

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);  //해당 오브젝트 파괴불가 처리
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator InitializeDatas()
    {
        DataManager = FindAnyObjectByType<DataManager>();
        EventManager = FindAnyObjectByType<EventManager>();
        playerCore = FindAnyObjectByType<PlayerCore>();
        playerStatus = FindAnyObjectByType<PlayerStatus>();
        soundManager = FindAnyObjectByType<SoundManager>();
        bgmManager = FindAnyObjectByType<BGMManager>();
        uISoundManager = FindAnyObjectByType<UISoundManager>();
        playerUIManager = FindAnyObjectByType<PlayerUIManager>();

        if (DataManager == null || EventManager == null || playerStatus == null || playerCore == null 
            || soundManager == null || bgmManager == null || uISoundManager == null || playerUIManager == null)
        {
            if (DataManager == null) Debug.LogError("DataManger가 존재하지 않음.");
            if (EventManager == null) Debug.LogError("EventManager가 존재하지 않음.");
            if (playerStatus == null) Debug.LogError("playerStatus가 존재하지 않음.");
            if (playerCore == null) Debug.LogError("playerCore가 존재하지 않음.");
            if (soundManager == null) Debug.LogError("soundManager가 존재하지 않음.");
            if (bgmManager == null) Debug.LogError("bgmManager가 존재하지 않음.");
            if (uISoundManager == null) Debug.LogError("uISoundManager가 존재하지 않음.");
            if(playerUIManager == null) Debug.LogError("playerUIManager가 존재하지 않음.");

            yield break;
        }
        //DataManager과 EventManager의 싱글톤이 존재할때, 넘어감
        yield return new WaitUntil(() =>
        {
            return DataManager.instance != null 
            && EventManager.f_CorrectionValueEvent != null
            && PlayerUIManager.instance != null;
        });

        playerStatus.InitializeEvent();  //EventManger -> f_CorrectionValueEven 체인처리
        playerCore.InitializeEvent();    //EventManger -> f_CorrectionValueEvent, DataManger -> Level 관리, PlayerUIManager 인스턴스
        soundManager.Initialize();       //DataManager -> soundDatabase 받기

        yield return new WaitUntil(() => SoundManager.instance != null);  //SoundManager인스턴스 존재 할 시, 넘어감
        bgmManager.Initialized();        //SoundManager에서 BGM등록
        uISoundManager.Initialized();    //SoundManager에서 UI등록
    }
}
