using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public abstract void SetRelicAttackValue(object obj, PlayerEvent e);
    public abstract void SetCoolDownValue(object obj, PlayerEvent e);

}
