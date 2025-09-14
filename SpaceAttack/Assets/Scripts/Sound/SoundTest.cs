using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound2
{
    public string name;

    [Range(0f, 1f)] public float volume;

    [Range(0.1f, 3f)] public float pitch;

    public bool isLoop;
    public AudioClip clip;

    [HideInInspector] public AudioSource source;
}

public class SoundTest : MonoBehaviour
{
    public List<Sound2> sound = new List<Sound2>();
    void Start()
    {
        SoundDatabaseSO database = DataManager.instance._soundDatabase;    //사운드 데이터 베이스 받기

        if (database == null)
        {
            LogUtil.Log("사운드 데이터 베이스가 존재하지 않다");
            return;
        }

        foreach (var s in database.sounds)
        {
            Sound2 temp = new Sound2();
            temp.name = s.soundName;
            temp.volume = s.volume;
            temp.pitch = s.pitch;
            temp.isLoop = s.loop;
            temp.clip = s.clip;
        }
    }
}
