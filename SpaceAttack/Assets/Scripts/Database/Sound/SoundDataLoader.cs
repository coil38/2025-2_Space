#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Audio;

public class SoundDataLoader : EditorWindow
{
    private static string outputFolder = "Assets/ScriptableObjects/sounds";             //출력 SO 파일 경로 - 각각의 데이터에 따라서 다르게 할당
    private static string AudioMixerPath = "Assets/Materials/Sound/AudioMixer.mixer";   //AudioMixer 현재 경로
    private static string AudioPath = "Assets/Materials/Sound";                        //모든 사운드가 존재하는 경로 주소 앞 부분
    public static string jsonFilePath {  get; set; }
    public static bool createDatabase {  get; set; }
    public static void ConvertJsonToScriptableObjects(SoundType soundType)
    {
        //폴더 생성
        if (!Directory.Exists(outputFolder))   //폴더 위치를 확인하고 없으면 생성한다
        {
            Directory.CreateDirectory(outputFolder);
        }

        //JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);

        try
        {
            //JSON 파싱
            List<SoundData> soundDataList = JsonConvert.DeserializeObject<List<SoundData>>(jsonText);

            List<SoundSO> createdSounds = new List<SoundSO>();    //SoundSO 리스트 생성

            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(AudioMixerPath);   //UnityEditor 전용

            foreach (var soundData in soundDataList)
            {
                SoundSO soundSO = ScriptableObject.CreateInstance<SoundSO>();

                //데이터 복사
                soundSO.soundID = soundData.soundID;
                soundSO.soundKey = soundData.soundKey;
                soundSO.soundName = soundData.name;
                soundSO.soundAttribute = soundData.attribute;
                soundSO.volume = soundData.volume;
                soundSO.pitch = soundData.pitch;
                soundSO.loop = soundData.isLoop == 1;  //1이면 true, 0이면 false
                soundSO.is3D = soundData.is3D;
                soundSO.soundType = soundType;

                //사운드 타입에 따라서 오디오믹서 할당
                if (mixer != null)
                {
                    AudioMixerGroup[] mixerGroups = mixer.FindMatchingGroups(soundType.ToString());   //해당 사운드 타입과 맞는 그룹 찾기
                    soundSO.mixerGroup = mixerGroups[0];                                          //오디오 믹서 할당   
                }
                else Debug.LogWarning("오디오 믹서가 존재하지 않는다");

                //사운드 클립 로드
                if (!string.IsNullOrEmpty(soundData.soundClipPath))
                {
                    soundSO.clip = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AudioPath}/{soundData.soundClipPath}");
           
                    if (soundSO.clip == null)
                    {
                        Debug.LogWarning($"사운드 '{soundData.name}'의 사운드 클립을 찾을 수 없습니다. : {soundData.soundClipPath}");
                    }
                }

                //SO 저장 - ID를 4자리 숫자로 포맷딩
                string assetPath = $"{outputFolder}/Sound_{soundData.name}.asset";
                AssetDatabase.CreateAsset(soundSO, assetPath);

                //에셋 이름 저장
                soundSO.name = $"Sound_{soundData.name}";
                createdSounds.Add(soundSO);

                EditorUtility.SetDirty(soundSO);
            }

            //사운드 데이터 베이스 생성 (여러가지의 사운드 JSON이 하나의 사운드 데이터 베이스를 공유)
            if (createDatabase && createdSounds.Count > 0)
            {
                SoundDatabaseSO currentDatabase = AssetDatabase.LoadAssetAtPath<SoundDatabaseSO>($"{outputFolder}/SoundDatabase.asset");
                if (currentDatabase != null)
                {
                    for (int i = 0; i < createdSounds.Count; i++)
                    {
                        bool hasSound = false;
                        for (int j = 0; j < currentDatabase.sounds.Count; j++)
                        {
                            if (currentDatabase.sounds[j].soundID == createdSounds[i].soundID)  //데이터 베이스에 이미 사운드SO가 있을 경우, 변경처리
                            {
                                Debug.Log($"{createdSounds[i].soundName}가 중복확인되었다");
                                currentDatabase.sounds[j] = createdSounds[i];
                                hasSound = true;
                                break;
                            }
                        }
                        if (!hasSound)
                        {
                            Debug.Log($"{createdSounds[i].soundName}으로 새로운 사운드 등록한다");
                            currentDatabase.sounds.Add(createdSounds[i]);  //데이터 베이스에 이미 존재하는 사운드SO가 없을 경우, 추가
                        }
                    }
                }
                else
                {
                    SoundDatabaseSO database = ScriptableObject.CreateInstance<SoundDatabaseSO>();  //SoundDatabaseSO 생성
                    database.sounds = createdSounds;

                    AssetDatabase.CreateAsset(database, $"{outputFolder}/SoundDatabase.asset");
                    EditorUtility.SetDirty(database);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdSounds.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류: {e}");
        }
    }
}
#endif