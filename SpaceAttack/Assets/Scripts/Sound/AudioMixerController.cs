using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider UISoundSlider;

    [Header("설정UI")]
    [SerializeField] private SettingUIManager settingUIManager;

    private bool isOneTime = true;

    private void OnEnable()
    {
        if (isOneTime)
        {
            MasterSlider.onValueChanged.AddListener(SetMasterVolume);
            BGMSlider.onValueChanged.AddListener(SetBGMVolume);
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);
            UISoundSlider.onValueChanged.AddListener(SetUIVolume);

            //사운드값에 맞게 스라이더 초기화
            ResetVolumeSettings();
            isOneTime = false;
        }

        settingUIManager.cancelSaveEvent += CancelSavedVolums;
        settingUIManager.saveEvent += SaveVolumeSettings;
        settingUIManager.resetEvent += ResetVolumeSettings;
    }

    private void OnDisable()
    {
        settingUIManager.cancelSaveEvent -= CancelSavedVolums;
        settingUIManager.saveEvent -= SaveVolumeSettings;
        settingUIManager.resetEvent -= ResetVolumeSettings;
    }

    private void CancelSavedVolums()
    {
        SetMasterVolume(settingUIManager.savedVolumes["Master"]);
        SetBGMVolume(settingUIManager.savedVolumes["BGM"]);
        SetSFXVolume(settingUIManager.savedVolumes["SFX"]);
        SetUIVolume(settingUIManager.savedVolumes["UI"]);

        MasterSlider.value = settingUIManager.savedVolumes["Master"];
        BGMSlider.value = settingUIManager.savedVolumes["BGM"];
        SFXSlider.value = settingUIManager.savedVolumes["SFX"];
        UISoundSlider.value = settingUIManager.savedVolumes["UI"];
    }

    private void SaveVolumeSettings()  //사운드 설정값 저장
    {
        settingUIManager.savedVolumes["Master"] = MasterSlider.value;
        settingUIManager.savedVolumes["BGM"] = BGMSlider.value;
        settingUIManager.savedVolumes["SFX"] = SFXSlider.value;
        settingUIManager.savedVolumes["UI"] = UISoundSlider.value;
    }

    private void ResetVolumeSettings()  //사운드 설정값 초기화
    {
        //사운드값에 맞게 스라이더 초기화
        float value = Mathf.Pow(10, -3f / 20);   //슬라이더값 초기화

        if (settingUIManager.savedVolumes.Count <= 0)  //초기값이 없을 경우, 실행 ( 게임시작하고 한번만 실행 )
        {
            settingUIManager.savedVolumes.Add("Master", value);
            settingUIManager.savedVolumes.Add("BGM", value);
            settingUIManager.savedVolumes.Add("SFX", value);
            settingUIManager.savedVolumes.Add("UI", value);
        }
        else
        {
            settingUIManager.savedVolumes["Master"] = value;
            settingUIManager.savedVolumes["BGM"] = value;
            settingUIManager.savedVolumes["SFX"] = value;
            settingUIManager.savedVolumes["UI"] = value;

        }

        MasterSlider.value = value;
        BGMSlider.value = value;
        SFXSlider.value = value;
        UISoundSlider.value = value;

        audioMixer.SetFloat("Master", 0f);   //오디오믹서값 초기화
        audioMixer.SetFloat("BGM", 0f);
        audioMixer.SetFloat("SFX", 0f);
        audioMixer.SetFloat("UI", 0f);
    }

    public void SetMasterVolume(float volume)  //마스터 사운드 설정
    {
        if (volume == 0) audioMixer.SetFloat("Master", -80f);
        else audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20f + 3f);
        //Debug.Log($"현재 슬라이드값: {volume}, 실제 볼륨값: {Mathf.Log10(volume) * 20f}");

        //변경사항 체크
        if(!isOneTime) settingUIManager.isChanged.Enqueue(true);
    }

    public void SetBGMVolume(float volume)  //브금 설정
    {
        if (volume == 0) audioMixer.SetFloat("BGM", -80f);
        else audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20f + 3f);

        //변경사항 체크
        if (!isOneTime) settingUIManager.isChanged.Enqueue(true);
    }

    public void SetSFXVolume(float volume)  //효과음 설정
    {
        if (volume == 0) audioMixer.SetFloat("SFX", -80f);
        else audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20f + 3f);

        //변경사항 체크
        if (!isOneTime) settingUIManager.isChanged.Enqueue(true);
    }

    public void SetUIVolume(float volume)  //UI 사운드 설정
    {
        if (volume == 0) audioMixer.SetFloat("UI", -80f);
        else audioMixer.SetFloat("UI", Mathf.Log10(volume) * 20f + 3f);

        //변경사항 체크
        if (!isOneTime) settingUIManager.isChanged.Enqueue(true);
    }
}
