using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RelicInfo
{
    public int id;  //효과ID
    public float n; //상수값1
    public int z;   //자연수값1

    public RelicInfo(int _id, float _n = 0f, int _z = 0)
    {
        n = _n;
        id = _id;
        z = _z;
    }
}
