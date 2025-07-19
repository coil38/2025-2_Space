using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private ChipSetType _chipSet;
    public ChipSetType chipSet
    {
        set
        { 
            if (_chipSet != null)        //칩셋에 이미 있을 경우
            {
                RemoveChipset(_chipSet);
                DropChipset(_chipSet);
                _chipSet = value;
                SetChipset();
            }
            else
            {
                _chipSet = value;
                SetChipset();
            }
        }
    }

    private PlayerAttack playerAttack;
    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        
    }

    private void DropChipset(ChipSetType m_chipSet)
    {
        //월드 드랍
        //Instantiate(m_chipSet.prefab);
    }

    private void RemoveChipset(ChipSetType m_chipSet)
    {
        playerAttack.WeaponType = null;
        m_chipSet.weapon.attackAnimator = null;

        playerAttack.SkillTypes = null;
        foreach (var skill in m_chipSet.skills)
        {
            skill.attackAnimator = null;
            skill.lineRenderer = null;
        }
    }

    private void SetChipset()
    {
        playerAttack.WeaponType = _chipSet.weapon;
        TimeSystem.w_w_AttackTimer = _chipSet.weapon.w_AttackTimer;  //대기시간 설정
        _chipSet.weapon.attackAnimator = GetComponent<Animator>();  //애니메이터 전달

        playerAttack.SkillTypes = _chipSet.skills;
        TimeSystem.s_w_AttackTimer = _chipSet.skills[0].s_AttackTimer;  //대기시간 설정

        foreach (var skill in _chipSet.skills)
        {
            skill.attackAnimator = GetComponent<Animator>();
            skill.lineRenderer = GetComponent<LineRenderer>();
        }
    }
}
