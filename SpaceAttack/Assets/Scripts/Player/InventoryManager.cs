using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private ChipSetType _chipSet;
    public ChipSetType chipSet
    {
        get { return _chipSet; }
        set
        { 
            if (_chipSet != null)        //칩셋에 이미 있을 경우
            {
                RemoveChipsetToPlayerAttack(_chipSet);
                DropChipset(_chipSet);

                _chipSet = value;        //새로운 칩셋 설정
                SetChipsetObject();
                SetChipsetToPlayerAttack();
            }
            else
            {
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

    private void DropChipset(ChipSetType m_chipSet)
    {
        //월드 드랍 연출

        //Color color = m_chipSet.gameObject.GetComponent<SpriteRenderer>().color;   //해당 칩셋을 원래 상태로 변경
        //color.a = 1f;
        //m_chipSet.gameObject.GetComponent<SpriteRenderer>().color = color;

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

//-----------------------------------------------------------------------------------------유물 획득용---------------------------------------------------------------------------------------

    private List<RelicSO> relics = new List<RelicSO>();
    public RelicSO[] _relics
    {
        get { return relics.ToArray(); }
    }

    public BaseRelic relic
    {
        set
        {
            int id = value.relicId;
            RelicSO relic = DataManager.instance._RelicDatabase.GetRelicById(id);

            if (relic == null)
            {
                LogUtil.LogError($"Id_{id}인 유물이 없습니다.");
                return;
            }

            if (relic.relicInfos == null)
                LogUtil.LogError("RelicInfo인스턴스가 존재하지 않습니다.");

            if (currentDarkMaterial + relic.darkMaterialCount <= 100)  //최대용량을 넘지 않을 경우
            {
                LogUtil.Log($"{relic.relicName}_유물 획득, 현재 암흑게이지수치: {currentDarkMaterial}, 현재 보유중인 유물개수: {relics.Count}");

                PlayerUIManager.instance.SetPlayerItem(relic);                     //유물 이미지 할당
                PlayerUIManager.instance.ChangeDarkMaterialUI(true, relic.darkMaterialCount); //암흑물질 채워지는 UI연출
                relics.Add(relic);                                     //유물 데이터 추가
                AddRelicEffects(relic);

                currentDarkMaterial += relic.darkMaterialCount;
                Destroy(value.gameObject);                             //획득한 유물 파괴
            }
            else
            {
                LogUtil.Log("가방이 가득 찼습니다.");
            }
            //암흑게이지 총량 체크
        }
    }

    private float currentDarkMaterial;

    public void SetSavedRelics(RelicSO[] relics)       //저장된 유물을 불러오는 함수 (저장 시스템용)
    {
        this.relics = new List<RelicSO>(relics);       //저장

        foreach (var relic in relics)
        {
            PlayerUIManager.instance.ChangeDarkMaterialUI(true, relic.darkMaterialCount); //암흑물질 채워지는 UI 갱신
            AddRelicEffects(relic);
            currentDarkMaterial += relic.darkMaterialCount;
        }
    }

    public void InitialInventoryDatas()              //인벤토리 초기화 (저장 시스템용)
    {
        relics.Clear();
        currentDarkMaterial = 0;
        PlayerUIManager.instance.ResetDarkMaterialUI();
        PlayerUIManager.instance.ClearPlayerItem();

        if (_chipSet != null)
        {
            RemoveChipsetToPlayerAttack(_chipSet);
            DropChipset(_chipSet);
            _chipSet = null;
        }
    }

    public void DropRelic(RelicSO relicSO)
    {
        //월드 드랍 연출
        GameObject temp = DataManager.instance._relicObject;
        BaseRelic relic = Instantiate(temp, transform.position, temp.transform.rotation).GetComponent<BaseRelic>();
        relic.Initialize(relicSO.relicID, relicSO.relicName, relicSO.iconSprite); //생성한 유물 오브젝트에 유물정보 갱신
    }

    public void RemoveRelic(RelicSO m_relicSO)
    {
        PlayerUIManager.instance.ChangeDarkMaterialUI(false, m_relicSO.darkMaterialCount); //암흑물질 감소하는 UI연출
        currentDarkMaterial -= m_relicSO.darkMaterialCount;     //암흑물질 감소
        relics.Remove(m_relicSO);                               //유물 데이터 삭제
        RelicEffectManager.ApplyRelicEffect(m_relicSO, false);  //유물 효과 제거
    }

    private void AddRelicEffects(RelicSO m_relicSO)        //유물 효과 실행부
    {
        RelicEffectManager.ApplyRelicEffect(m_relicSO, true);
    }
}
