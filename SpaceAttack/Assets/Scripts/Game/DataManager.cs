using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;   //싱글톤 패턴

    [SerializeField] private SoundDatabaseSO soundDatabase;  //사운드 데이터 베이스
    [SerializeField] private LevelDatabaseSO levelDatabase;  //레벨 데이터 베이스
    [SerializeField] private RelicDatabaseSO relicDatabase;  //유물 데이터 베이스

    public SoundDatabaseSO _soundDatabase
    {
        get { return soundDatabase; }
        private set {  soundDatabase = value; }
    }
    public LevelDatabaseSO _levelDatabase
    {
        get { return levelDatabase; }
        private set { levelDatabase = value; }
    }

    public RelicDatabaseSO _RelicDatabase
    {
        get { return  relicDatabase; }
        private set {  relicDatabase = value; }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
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
