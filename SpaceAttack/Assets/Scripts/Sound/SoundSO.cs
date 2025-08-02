using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "new Sound", menuName = "Sound/sounds")]
public class SoundSO : ScriptableObject
{
    public string soundKey;
    public string soundName;
    public float volume;
    public float pitch;
    public bool loop;
    public int is3D;
    public SoundType soundType;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;

    public override string ToString()
    {
        return $"[{soundKey}] ({soundType}) - 크기: {volume}, 속도: {pitch}, 반복: {loop}, 3차원사운드: {is3D}";
    }
}
