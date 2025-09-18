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
    private PlayerSoundManager playerSoundManager; //SoundManager.instance관련
    private PlayerUIManager playerUIManager; //PlayerUIManager.instance관련
    private RelicEffectManager relicEffectManager; //아무관련 없음

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            StartCoroutine(InitializeDatas());

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
        playerSoundManager = FindAnyObjectByType<PlayerSoundManager>();
        playerUIManager = FindAnyObjectByType<PlayerUIManager>();
        relicEffectManager = FindAnyObjectByType<RelicEffectManager>();

        if (DataManager == null || EventManager == null || playerStatus == null || playerCore == null 
            || soundManager == null || bgmManager == null || uISoundManager == null || playerSoundManager == null|| playerUIManager == null
            || relicEffectManager == null)
        {
            if (DataManager == null) LogUtil.LogError("DataManger가 존재하지 않음.");
            if (EventManager == null) LogUtil.LogError("EventManager가 존재하지 않음.");
            if (playerStatus == null) LogUtil.LogError("playerStatus가 존재하지 않음.");
            if (playerCore == null) LogUtil.LogError("playerCore가 존재하지 않음.");
            if (soundManager == null) LogUtil.LogError("soundManager가 존재하지 않음.");
            if (bgmManager == null) LogUtil.LogError("bgmManager가 존재하지 않음.");
            if (uISoundManager == null) LogUtil.LogError("uISoundManager가 존재하지 않음.");
            if (playerSoundManager == null) LogUtil.LogError("playerSoundManager가 존재하지 않음.");
            if (playerUIManager == null) LogUtil.LogError("playerUIManager가 존재하지 않음.");
            if (relicEffectManager == null) LogUtil.LogError("relicEffectManager가 존재하지 않음.");

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
        relicEffectManager.Initialize(); //초기화

        yield return new WaitUntil(() => SoundManager.instance != null && SoundManager.instance.endInitialize);  //SoundManager인스턴스 존재 할 시, 넘어감
        bgmManager.Initialized();        //SoundManager에서 BGM등록
        uISoundManager.Initialized();    //SoundManager에서 UI등록
        playerSoundManager.Initialized(); //SoundManager에서 playerSound등록
    }
}
