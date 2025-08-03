using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    void Start()
    {
        SoundDatabaseSO database = DataManager.instance._soundDatabase;    //사운드 데이터 베이스 받기

        if (database == null)
        {
            Debug.Log("사운드 데이터 베이스가 존재하지 않다");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
