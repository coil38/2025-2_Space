using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SoundData
{
    public int soundID;
    public string name;
    public string attribute;
    public float volume;
    public float pitch;
    public int isLoop;
    public int is3D;
    public string soundClipPath;

    [NonSerialized]
    public SoundType soundType;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
}
