using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AttackInfo
{
    public float damage {  get; set; }
    public Vector3 attackDirection { get; set; }
    public float attackForce { get; set; }

    public GameObject attacker { get; set; }
    public AttackInfo(float damage, Vector3 dir, float mass = 1f, GameObject attacker = null)
    {
        this.damage = damage;
        this.attackDirection = dir;
        this.attackForce = mass * 100f;
        this.attacker = attacker;

    }

    public void SetAttackInfo(float damage, Vector3 dir,float mass = 1f, GameObject attacker = null)
    {
        this.damage = damage;
        this.attackDirection = dir;
        this.attackForce = mass * 100f;
        this.attacker = attacker;
    }

    public float CheckAndSetCritical(float damage, float criticalRate, float criticalHitRate)
    {
        float randomValue = Random.value;
        if (randomValue < criticalRate) //크리티컬 처리
        {
            damage += damage * criticalHitRate;
        }

        return damage;
    }
}
