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
                Debug.Log("현 보유중인 칩셋이 있음");
                RemoveChipsetToPlayerAttack(_chipSet);
                DropChipset(_chipSet);

                _chipSet = value;        //새로운 칩셋 설정
                SetChipsetObject();
                SetChipsetToPlayerAttack();
            }
            else
            {
                Debug.Log("현 보유중인 칩셋이 없음");
                _chipSet = value;
                SetChipsetObject();
                SetChipsetToPlayerAttack();
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
        //월드 드랍 연출


        Color color = m_chipSet.gameObject.GetComponent<SpriteRenderer>().color;   //해당 칩셋을 원래 상태로 변경
        color.a = 1f;
        m_chipSet.gameObject.GetComponent<SpriteRenderer>().color = color;

        m_chipSet.gameObject.transform.SetParent(null);                  //해당 칩셋을 Player 자식으로 넣기 해제
    }

    private void RemoveChipsetToPlayerAttack(ChipSetType m_chipSet)
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

    private void SetChipsetObject()              //월드의 칩셋 오브젝트 설정
    {
        Color color = _chipSet.gameObject.GetComponent<SpriteRenderer>().color;   //해당 칩셋을 투명상태로 변경
        color.a = 0f;
        _chipSet.gameObject.GetComponent<SpriteRenderer>().color = color;

        _chipSet.gameObject.transform.SetParent(this.transform);                  //해당 칩셋을 Player 자식으로 넣기
        _chipSet.gameObject.transform.localPosition = Vector3.zero;
    }


    private void SetChipsetToPlayerAttack()        //PlayerAttack 스크립트에 접근 구현
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
