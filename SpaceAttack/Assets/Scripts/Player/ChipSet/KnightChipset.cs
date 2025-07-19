using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightChipset : ChipSetType
{
    public override WeaponType weapon { get; protected set; }
    public override SkillType[] skills { get; protected set; }
    public override string chipSetName { get; protected set; }
    public override string description { get; protected set; }
    public override Sprite iconImage { get; protected set; }
    public override GameObject prefab { get; protected set; }

    //인스펙터창에서 대상 할당
    [SerializeField] private WeaponType _weapon;
    [SerializeField] private SkillType[] _skills;
    [SerializeField] private Sprite _iconImage;
    [SerializeField] private GameObject _prefab;

    public override void SetCorrectionValue(Object obj, FindCorectionValueEvent e)
    {
        Debug.Log($"{e.test}를 스킬 보정치에 주입");
    }

    void OnEnable()      //임시
    {
        chipSetName = "검사";
        description = "그냥 저냥 평범한 칩셋(직업)";
        weapon = _weapon;
        skills = _skills;
        iconImage = _iconImage;
        prefab = _prefab;
    }

    void Update()
    {

    }
}
