using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChipAttackType
{
    Weapon,
    Skill
}

public abstract class ChipSetType : MonoBehaviour
{
    public  WeaponType weapon;
    public SkillType[] skills;
    public string chipSetName;
    public string description;
    public Sprite iconImage;
    public GameObject prefab;
    public Animator animator;
    public abstract void SetCorrectionValue(object obj, PlayerEvent e);
    public abstract void SetChipAttackRate(object obj, PlayerEvent e);
    public abstract void SetChipDamageRate(object obj, PlayerEvent e);
    public abstract void SetChipWeaponDamageRate(object obj, PlayerEvent e);
    public abstract void SetChipSkillDamageRate(object obj, PlayerEvent e);
    public abstract void SetCoolDownRate(object obj, PlayerEvent e);
    public abstract void Attack(GameObject target, float damageRate, Vector3 dir, ChipAttackType type);

}
