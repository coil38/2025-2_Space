using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChipSetType : MonoBehaviour
{
    public abstract WeaponType weapon { get; protected set; }
    public abstract SkillType[] skills { get; protected set; }
    public abstract string chipSetName { get; protected set; }
    public abstract string description { get; protected set; }
    public abstract Sprite iconImage { get; protected set; }
    public abstract GameObject prefab { get; protected set; }
    public abstract void SetCorrectionValue(Object obj, FindCorectionValueEvent e);

}
