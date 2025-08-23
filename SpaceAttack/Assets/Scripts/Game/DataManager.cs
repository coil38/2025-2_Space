using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;   //싱글톤 패턴

    public SoundDatabaseSO _soundDatabase;
    public static SoundDatabaseSO soundDatabase;
    void Awake()
    {
        if (_soundDatabase != null)
        {
            soundDatabase = _soundDatabase;
        }
        else
        {
            Debug.LogError($"{soundDatabase}가 할당이 안되었습니다.");
        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    SceneManager.LoadScene(1);
        //}

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    if (_soundDatabase == null)
        //    {
        //        Debug.Log("사운드 데이터 베이스가 사라짐");
        //    }
        //    else
        //    {
        //        Debug.Log("그대로 있음");
        //    }
        //}
    }
}
