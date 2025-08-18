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
    private PlayerMovementAnimationController aniController;
    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        aniController = GetComponent<PlayerMovementAnimationController>();
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

        m_chipSet.gameObject.GetComponent<Collider>().enabled = true;   //감지 가능상태로 변경
    }

    private void RemoveChipsetToPlayerAttack(ChipSetType m_chipSet)
    {
        playerAttack.WeaponType = null;

        //PMAC용
        aniController.SetAnimator(m_chipSet.animator, m_chipSet.name, false);  //공격본의 애니메이터 null처리
        m_chipSet.weapon.weaponAniDelegate -= aniController.OnAttackObj;   //공격본 활성화함수 체인 해지 처리

        //CAC용
        ChipsetAnimationController temp = _chipSet.GetComponent<ChipsetAnimationController>();
        temp.SetAnimator(null);                                //공격본의 애니메이터 전달
        _chipSet.weapon.weaponAniDelegate -= temp.PlayAttackAnimation;    //애니메이션 실행 코드 체인 구독 처리

        playerAttack.SkillTypes = null;
        foreach (var skill in m_chipSet.skills)
        {
            skill.skillAniDelegate -= aniController.OnAttackObj;    //공격본 활성화함수 체인 해지 처리
            skill.skillAniDelegate -= temp.PlayAttackAnimation;    //애니메이션 실행 코드 체인 해지 처리

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

        _chipSet.gameObject.GetComponent<Collider>().enabled = false;   //감지 가능상태로 변경
    }


    private void SetChipsetToPlayerAttack()        //PlayerAttack 스크립트에 접근 구현
    {
        playerAttack.WeaponType = _chipSet.weapon;
        PlayerTimeSystem.w_BaseAttackTimer = _chipSet.weapon.w_AttackTimer;  //대기시간 설정

        //PMAC용
        aniController.SetAnimator(_chipSet.animator, _chipSet.name, true);  //공격본의 애니메이터 할당처리
        _chipSet.weapon.weaponAniDelegate += aniController.OnAttackObj;   //공격본 활성화함수 체인 구독 처리

        //CAC용
        ChipsetAnimationController temp = _chipSet.GetComponent<ChipsetAnimationController>();
        temp.SetAnimator(aniController.attackAnimator);                   //공격본의 애니메이터 전달
        _chipSet.weapon.weaponAniDelegate += temp.PlayAttackAnimation;    //애니메이션 실행 코드 체인 구독 처리

        playerAttack.SkillTypes = _chipSet.skills;
        PlayerTimeSystem.w_SkillTimer = _chipSet.skills[0].s_AttackTimer;  //대기시간 설정

        foreach (var skill in _chipSet.skills)
        {
            skill.skillAniDelegate += aniController.OnAttackObj;   //공격본 활성화함수 체인 구독 처리
            skill.skillAniDelegate += temp.PlayAttackAnimation;    //애니메이션 실행 코드 체인 구독 처리

            skill.attackAnimator = GetComponent<Animator>();
            skill.lineRenderer = GetComponent<LineRenderer>();
        }
    }
}
