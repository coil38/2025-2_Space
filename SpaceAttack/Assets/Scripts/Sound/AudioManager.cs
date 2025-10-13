using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    public bool isLoop;
    public AudioClip clip;

    [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;   // 싱글톤
    public List<Sound> sounds = new List<Sound>();

    private AudioSource bgmSource;        
    private Coroutine fadeCoroutine;     

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
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

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 1f;
    }

    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // 같은 BGM이면 다시 재생하지 않음
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    public void FadeOutBGM(float fadeTime = 1.5f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutCoroutine(fadeTime));
    }

    private IEnumerator FadeOutCoroutine(float fadeTime)
    {
        float startVolume = bgmSource.volume;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }

    public void PlaySound(string name)
    {
        Sound playsound = sounds.Find(s => s.name == name);
        if (playsound != null)
            playsound.source.Play();
    }

    public void PauseSound(string name)
    {
        Sound pauseSound = sounds.Find(s => s.name == name);
        if (pauseSound != null)
            pauseSound.source.Pause();
    }

    public void StopSound(string name)
    {
        Sound stopSound = sounds.Find(s => s.name == name);
        if (stopSound != null)
            stopSound.source.Stop();
    }

    public void StopAllSounds()
    {
        foreach (var sound in sounds)
            sound.source.Stop();
    }
}

