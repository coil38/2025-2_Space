using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Sound/Database")]
public class SoundDatabaseSO : ScriptableObject
{
    public List<SoundSO> sounds = new List<SoundSO>();

    //캐싱을 위한 사전
    private Dictionary<string, SoundSO> soundByKey;     //스트링키로 사운드 찾기 위한 캐싱

    public void Initialize()
    {
        soundByKey = new Dictionary<string, SoundSO>();

        foreach (var sound in sounds)
        {
            soundByKey[sound.soundKey] = sound;
        }
    }

    public SoundSO GetSound(string soundKey)  //스트링키로 사운드 찾기
    {
        if (soundByKey == null)
        {
            Initialize();
        }
        if (soundByKey.TryGetValue(soundKey, out SoundSO sound))
            return sound;

        return null;
    }
}
