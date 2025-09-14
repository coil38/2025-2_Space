using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarriorChipset : ChipSetType
{
    public override WeaponType weapon { get; protected set; }
    public override SkillType[] skills { get; protected set; }
    public override string chipSetName { get; protected set; }
    public override string description { get; protected set; }
    public override Sprite iconImage { get; protected set; }
    public override GameObject prefab { get; protected set; }
    public override Animator animator { get; protected set; }

    //인스펙터창에서 대상 할당
    [SerializeField] private WeaponType _weapon;
    [SerializeField] private SkillType[] _skills;
    [SerializeField] private Sprite _iconImage;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Animator _animator;

    private Queue<float> n_DamageRates = new Queue<float>();
    private Queue<float> s_DamageRates = new Queue<float>();  //임시 ( 유물이 장착 또는 해지에 따라서 특정 수치가 빠져야됨 )
    public override void SetCorrectionValue(object obj, FindCorectionValueEvent e)
    {
        if (e.correctablility)  //공격력 보정치 주입
        {
            n_DamageRates.Enqueue(e.damageCorrection);
            UpdateAttackDamage();
        }

        if (e.unlockability)  //스킬 해금
        {
            int unlockNum = e.skillNumber;
            foreach (var skill in _skills)
            {
                if (skill.unLockedNumber == unlockNum)  //해금
                {
                    skill.canUse = true;
                }
            }

            //UI스킬 해금 연출
        }
    }

    private void UpdateAttackDamage()
    {
        float n_DamageRate = 100f;
        foreach (var rate in n_DamageRates)
        {
            n_DamageRate += rate;
        }
        _weapon.damage = (PlayerStatus.normalDamage * n_DamageRate / 100f) * _weapon.damageRate;  //공격력 연산
        LogUtil.Log($"총 공격력: {_weapon.damage}, 누적 공격수치: {n_DamageRate}");
        foreach (var skill in _skills)
            skill.damage = (PlayerStatus.normalDamage * n_DamageRate / 100f) * skill.damageRate;
    }

    private void OnEnable()
    {
        chipSetName = "Warrior";
        description = "그냥 저냥 평범한 칩셋(직업)";
        weapon = _weapon;
        skills = _skills;
        iconImage = _iconImage;
        prefab = _prefab;
        animator = _animator;

        StartCoroutine(SetEvent());
    }

    private void OnDisable()
    {
        EventManager.f_CorrectionValueEvent.correctionEventHandler -= SetCorrectionValue;  //공격력, 스킬 보정 이벤트 구독 해지
    }

    private IEnumerator SetEvent()
    {
        yield return new WaitUntil(() => EventManager.f_CorrectionValueEvent != null);
        EventManager.f_CorrectionValueEvent.correctionEventHandler += SetCorrectionValue;  //공격력, 스킬 보정 이벤트 구독
    }
}
