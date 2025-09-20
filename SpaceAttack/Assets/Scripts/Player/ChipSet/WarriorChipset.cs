using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarriorChipset : BaseChipset
{
    protected override void OnEnable()
    {
        chipSetName = "Warrior";
        description = "그냥 저냥 평범한 칩셋(직업)";
        weapon = _weapon;
        skills = _skills;
        iconImage = _iconImage;
        prefab = _prefab;
        animator = _animator;

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}
