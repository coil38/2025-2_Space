using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    [Range(0f, 1f)] public float volume;

    [Range(0.1f, 3f)] public float pitch;

    public bool isLoop;
    public AudioClip clip;

    [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;            //싱글톤 패턴

    public List<Sound> sounds = new List<Sound>();
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.name = sound.name;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.isLoop;
            sound.source.clip = sound.clip;
        }
    }

    public void PlaySound(string name)
    {
        Sound playsound = sounds.Find(s => s.name == name);

        if (playsound != null)
        {
            playsound.source.Play();
        }
    }

    public void PauseSound(string name)
    {
        Sound pauseSound = sounds.Find(s => s.name == name);

        if (pauseSound != null)
        {
            pauseSound.source.Pause();
        }
    }

    public void StopAllSounds()
    {
        foreach (var sound in sounds)
        {
            sound.source.Stop();
        }
    }
}
