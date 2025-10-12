using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;   //싱글톤 패턴

    [SerializeField] private SoundDatabaseSO soundDatabase;  //사운드 데이터 베이스
    [SerializeField] private LevelDatabaseSO levelDatabase;  //레벨 데이터 베이스
    [SerializeField] private RelicDatabaseSO relicDatabase;  //유물 데이터 베이스

    [SerializeField] private GameObject relicObject;         //유물 프리팹
    [SerializeField] private GameObject[] chipsetPrfs;       //칩셋 프리팹

    public GameObject _relicObject
    {
        get { return relicObject; }
    }

    public GameObject GetChipsetPrfByName(string name)
    {
        foreach(var chipset in chipsetPrfs)
            if (chipset.GetComponent<ChipSetType>().chipSetName == name)
                return chipset;
        return null;
    }

    public SoundDatabaseSO _soundDatabase
    {
        get { return soundDatabase; }
    }
    public LevelDatabaseSO _levelDatabase
    {
        get { return levelDatabase; }
    }

    public RelicDatabaseSO _RelicDatabase
    {
        get { return  relicDatabase; }
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
}
