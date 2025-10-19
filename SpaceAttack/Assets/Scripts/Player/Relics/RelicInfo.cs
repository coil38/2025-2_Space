using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RelicInfo
{
    public int id;  //효과ID
    public float n;
    public float z;
    public float y;
    public float w;

    public RelicInfo(int id, float n = 0f, float z = 0, float y = 0, float w = 0)
    {
        this.id = id;
        this.n = n;
        this.z = z;
        this.y = y;
        this.w = w;
    }
}
