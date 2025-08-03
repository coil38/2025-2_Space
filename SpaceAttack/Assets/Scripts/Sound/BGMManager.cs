using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    void Start()
    {
        //Invoke("Test", 1f);
        //Invoke("Test2", 5f);
    }

    void Update()
    {
        
    }

    void Test()
    {
        SoundManager.instance.RegisterGameObject(gameObject, "None");
        SoundManager.instance.PlaySound(gameObject, "EarthBossBattleStage_2");
    }

    void Test2()
    {
        //SoundManager.instance.StopSound(gameObject, "EarthBossBattleStage_2");
    }
}
