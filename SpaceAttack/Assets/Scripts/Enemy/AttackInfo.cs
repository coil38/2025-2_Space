using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AttackInfo
{
    public float damage;
    public Vector3 attackDirection;

    public AttackInfo(float damage, Vector3 dir)
    {
        this.damage = damage;
        this.attackDirection = dir;
    }
}
