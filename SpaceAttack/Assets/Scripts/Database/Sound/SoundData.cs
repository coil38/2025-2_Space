using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SoundData
{
    public string soundKey;
    public string name;
    public string attribute;
    public float volume;
    public float pitch;
    public int isLoop;
    public int is3D;
    public string soundTypeString;
    public string soundClipPath;

    [NonSerialized]
    public SoundType soundType;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;

    //[HideInInspector]
    //public AudioSource source;  

    //임시(각각의 여러 개체가 같은 종류의 사운드를 가질 때, 각 객체가 스스로 사운드 등록 및 파괴될 시, 등록해제

    public void InitalizeEnums()
    {
        if (Enum.TryParse(soundTypeString, out SoundType parsedType))
        {
            soundType = parsedType;
        }
        else
        {
            Debug.LogError($"아이템 '{name}'에 유효하지 않은 아이템 타입: {soundTypeString}");

            soundType = SoundType.Non;
        }
    }
}
