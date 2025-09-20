using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherChipset : BaseChipset
{
    protected override void OnEnable()      //임시
    {
        chipSetName = "Archer";
        description = "그냥 저냥 평범한 칩셋(직업)2";
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
