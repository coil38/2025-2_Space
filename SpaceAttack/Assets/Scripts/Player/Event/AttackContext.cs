using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackContext
{
    public GameObject target { get; set; }  
    public float damageRate { get; set; }
    public Vector3 attackDir { get; set; }
    public float attackRateSume { get; set; }
    public float damageRateSume { get; set; }
    public float weaponDamagekRateSume { get; set; }
    public float skillDamageRateSume { get; set; }
    public ChipAttackType attackType { get; set; }
    public float addedCritRate { get; set; }
    public float addedCritChanceRate { get; set; }

    //-------------------유물용 변수------------------------
    public bool IsCritical { get; set; }
    public bool IsReattack { get; set; }

    public AttackContext(GameObject target, float damageRate, Vector3 attackDir, ChipAttackType type,float addedCritRate, float addedCriChanceRate, float a_rateSume, float d_rateSume, float w_d_rateSume, float s_d_rateSume)
    {
        this.target = target;
        this.damageRate = damageRate;
        this.attackDir = attackDir;
        this.attackType = type;
        this.addedCritChanceRate = addedCriChanceRate;
        this.addedCritRate = addedCritRate;
        this.attackRateSume = a_rateSume;
        this.damageRateSume = d_rateSume;
        this.weaponDamagekRateSume = w_d_rateSume;
        this.skillDamageRateSume = s_d_rateSume;
    }
}
