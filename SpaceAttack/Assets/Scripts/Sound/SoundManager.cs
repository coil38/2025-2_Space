using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    Dictionary<string, SoundSO[]> soundLibrary = new Dictionary<string, SoundSO[]>();              //<속성명, 사운드 데이터>
    Dictionary<GameObject, SoundInfo[]> audioRegistry = new Dictionary<GameObject, SoundInfo[]>(); //<인스턴스주소, 오디오소스>

    List<SoundSO> BGMOrUISoundLibrary = new List<SoundSO>();      //BGM과 UI용 사운드 저장소
    Dictionary<int, SoundInfo> BGMRegistry = new Dictionary<int, SoundInfo>();
    Dictionary<int, SoundInfo> UISFXRegistry = new Dictionary<int, SoundInfo>();

    List<SoundInfo> playedSound = new List<SoundInfo>();  //현재 재생중인 사운드들
    SoundInfo playedBGM;                                  //현재 재생중인 브금

    private List<SoundInfo> RegisterTemp = new List<SoundInfo>();    //사운드 등록용

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        SoundDatabaseSO database = DataManager.instance._soundDatabase;    //사운드 데이터 베이스 받기

        if (database == null)
        {
            LogUtil.Log("사운드 데이터 베이스가 존재하지 않다");
            return;
        }

        Dictionary<string, List<SoundSO>> temp = new Dictionary<string, List<SoundSO>>();   //속성 별로 분류하기 위한 선언
        foreach (var sound in database.sounds)
        {
            if (sound == null)
            {
                LogUtil.Log("sound가 존재하지 않습니다.");
                return;
            }

            if (sound.soundType == SoundType.BGM || sound.soundType == SoundType.UI)  //배경음 혹은 UI사운드 별도로 저장
            {
                BGMOrUISoundLibrary.Add(sound);
                continue;
            }

            if (temp.ContainsKey(sound.soundAttribute))
            {
                temp[sound.soundAttribute].Add(sound);                 //사운드SO 리스트 추가
            }
            else
            {
                temp.Add(sound.soundAttribute, new List<SoundSO>());   //사운드SO 리스트 생성
                temp[sound.soundAttribute].Add(sound);                 //사운드SO 리스트 추가
            }
        }

        foreach (var sound in temp)
            soundLibrary.Add(sound.Key, sound.Value.ToArray());        //필드로 변환
    }

    public void RegisterGameObjectBySoundType(GameObject obj, SoundType soundType)  //BGM과 UISound용 등록함수
    {
        if(soundType == SoundType.BGM) BGMRegistry.Clear();   //초기화
        else if (soundType == SoundType.UI) UISFXRegistry.Clear();

        foreach (var sound in BGMOrUISoundLibrary)
        {
            if (sound.soundType != soundType) continue;

                AudioSource source = obj.AddComponent<AudioSource>();  //오디오소스 생성 및 값 할당
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.spatialBlend = sound.is3D;
            float maxDistance = 5f;
            if (sound.is3D == 1)  //3차원 사운드일 경우
            {
                source.minDistance = 0f;
                source.maxDistance = maxDistance;

                //사운드 그래프 설정
                source.rolloffMode = AudioRolloffMode.Custom;

                AnimationCurve curve = new AnimationCurve(
                    new Keyframe(0f, 1f),       //0m -> 볼륨: 1
                    new Keyframe(0.5f, 0.5f),   //0.5m -> 볼륨: 0.5
                    new Keyframe(0.3f, 0.1f),    //0.1m -> 볼륨: 0.1
                    new Keyframe(maxDistance, 0f)
                    );

                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
            }
            source.clip = sound.clip;
            source.outputAudioMixerGroup = sound.mixerGroup;

            SoundInfo soundInfo = obj.AddComponent<SoundInfo>();   //SoundInfo에 정보 할당
            soundInfo.soundID = sound.soundID;
            soundInfo.soundName = sound.soundName;
            soundInfo.audioSource = source;

            if (soundType == SoundType.BGM)
            {
                BGMRegistry.TryAdd(sound.soundID, soundInfo);
            }
            else if (soundType == SoundType.UI)
            {
                UISFXRegistry.TryAdd(sound.soundID, soundInfo);
            }
        }
    }

    public void RegisterGameObjectByAttribute(GameObject obj, string attribute, SoundType soundType)   //해당 오브젝트가 필요한 오디오소스 할당 및 인스턴스주소 등록
    {
        if (soundLibrary.TryGetValue(attribute, out var sounds))
        {
            foreach (var sound in sounds) 
            {
                AudioSource source = obj.AddComponent<AudioSource>();  //오디오소스 생성 및 값 할당
                source.volume = sound.volume;
                source.pitch = sound.pitch;
                source.loop = sound.loop;
                source.spatialBlend = sound.is3D;
                float maxDistance = 5f;
                if (sound.is3D == 1)  //3차원 사운드일 경우
                {
                    source.minDistance = 0f;
                    source.maxDistance = maxDistance;

                    //사운드 그래프 설정
                    source.rolloffMode = AudioRolloffMode.Custom;

                    AnimationCurve curve = new AnimationCurve(
                        new Keyframe(0f, 1f),       //0m -> 볼륨: 1
                        new Keyframe(0.5f, 0.5f),   //0.5m -> 볼륨: 0.5
                        new Keyframe(0.3f, 0.1f),    //0.1m -> 볼륨: 0.1
                        new Keyframe(maxDistance, 0f)
                        );

                    source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
                }
                source.clip = sound.clip;
                source.outputAudioMixerGroup = sound.mixerGroup;

                SoundInfo soundInfo = obj.AddComponent<SoundInfo>();   //SoundInfo에 정보 할당
                soundInfo.soundID = sound.soundID;
                soundInfo.soundName = sound.soundName;
                soundInfo.audioSource = source;

                RegisterTemp.Add(soundInfo);
            }

            audioRegistry.TryAdd(obj, RegisterTemp.ToArray());    //해당 속성0인 객체가 등록한다.
            RegisterTemp.Clear();
        }
        else
        {
            LogUtil.LogError($"{attribute}는 확인할 수 없는 속성입니다. 다시 체크해주세요");
        }
    }

    public void PlayBGMOrUISound(int soundID, SoundType soundType)
    {
        if (soundType == SoundType.SFX) return;
        if (soundType == SoundType.BGM)
        {
            if (BGMRegistry.TryGetValue(soundID, out SoundInfo sound))
            {
                if (playedBGM != null)
                {
                    LogUtil.Log("재생취소");
                    StopBGMOrUISound(playedBGM.soundID, SoundType.BGM);  //이미 재생중이던 브금 재생종료
                }
                LogUtil.Log("재생");
                playedBGM = sound;
                playedSound.Add(sound);
                sound.audioSource.Play();
            }
        }
        else if (soundType == SoundType.UI)
        {
            if (UISFXRegistry.TryGetValue(soundID, out SoundInfo sound))
            {
                playedSound.Add(sound);
                sound.audioSource.Play();
            }
        }
    }

    public void StopBGMOrUISound(int soundID, SoundType soundType)
    {
        if (soundType == SoundType.SFX) return;

        if (soundType == SoundType.BGM)
        {
            if (BGMRegistry.TryGetValue(soundID, out SoundInfo sound))
            {
                playedSound.Remove(sound);
                sound.audioSource.Stop();
            }
        }
        else if (soundType == SoundType.UI)
        {
            if (UISFXRegistry.TryGetValue(soundID, out SoundInfo sound))
            {
                playedSound.Remove(sound);
                sound.audioSource.Stop();
            }
        }
    }

    public void StopPlayedAllSound()  //현재 재생중인 모든 사운드 종료
    {
        foreach(var sound in playedSound)
            sound.audioSource?.Stop();
    }

    public void PlaySound(GameObject obj, string soundName)   //등록된 대상만 사용가능 및 사운드이름이 정확해야함
    {
        if (audioRegistry.ContainsKey(obj))
        {
            SoundInfo[] sounds = audioRegistry[obj];
            foreach (var sound in sounds)
            {
                if (sound.soundName == soundName)    //사운드 이름에 해당하는 오디오소스 재생
                {
                    sound.audioSource.Play();               //사운드 재생
                    playedSound.Add(sound);

                    LogUtil.Log($"{sound.soundName} 재생");
                }
            }
        }
        else
        {
            LogUtil.LogError($"{obj}에 해당하는 등록정보를 확인할 수 없다(해당 오브젝트가 등록을 안한 듯).");
        }
    }

    public void StopSound(GameObject obj, string soundName)   //등록된 대상만 사용가능 및 사운드이름이 정확해야함
    {
        if (audioRegistry.ContainsKey(obj))
        {
            SoundInfo[] sounds = audioRegistry[obj];
            foreach (var sound in sounds)
            {
                if (sound.soundName == soundName)    //사운드 이름에 해당하는 오디오소스 재생
                {
                    sound.audioSource.Stop();               //사운드 재생
                    playedSound.Remove(sound);
                }
            }
        }
        else
        {
            LogUtil.LogError($"{obj}에 해당하는 등록정보를 확인할 수 없다(해당 오브젝트가 등록을 안한 듯).");
        }
    }
}
