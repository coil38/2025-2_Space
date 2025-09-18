using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RelicInfo
{
    public int id { get; private set; }   //효과ID
    public float n { get; private set; } //상수값1
    public int z { get; private set; } //자연수값1

    public RelicInfo(int _id, float _n = 0f, int _z = 0)
    {
        n = _n;
        id = _id;
        z = _z;
    }
}
