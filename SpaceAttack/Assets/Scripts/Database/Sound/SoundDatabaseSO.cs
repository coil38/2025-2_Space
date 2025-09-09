using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Sound/Database")]
public class SoundDatabaseSO : ScriptableObject
{
    public List<SoundSO> sounds = new List<SoundSO>();

    private Dictionary<int, SoundSO> soundById;         //사운드ID로 사운드 찾기 위한 캐싱

    public void Initialize()
    {
        soundById = new Dictionary<int, SoundSO>();
        foreach (var sound in sounds)
            soundById[sound.soundID] = sound;
    }
    public SoundSO GetSoundById(int soundID)  //t사운드ID로 사운드 찾기
    {
        if (soundById == null)
        {
            Initialize();
        }
        if (soundById.TryGetValue(soundID, out SoundSO sound))
            return sound;

        return null;
    }
}
