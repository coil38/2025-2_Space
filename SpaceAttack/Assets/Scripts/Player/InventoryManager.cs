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
        m_chipSet.gameObject.transform.SetParent(null);                  //해당 칩셋을 Player 자식으로 넣기 해제
        Destroy(m_chipSet.gameObject);                                   //칩셋 오브젝트 파괴
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

        //PMAC용
        aniController.SetAnimator(_chipSet.animator, _chipSet.name, true);  //공격본의 애니메이터 할당처리
        _chipSet.weapon.weaponAniDelegate += aniController.OnAttackObj;   //공격본 활성화함수 체인 구독 처리

        //CAC용
        ChipsetAnimationController temp = _chipSet.GetComponent<ChipsetAnimationController>();
        temp.SetAnimator(aniController.attackAnimator);                   //공격본의 애니메이터 전달
        _chipSet.weapon.weaponAniDelegate += temp.PlayAttackAnimation;    //애니메이션 실행 코드 체인 구독 처리

        playerAttack.SkillTypes = _chipSet.skills;

        foreach (var skill in _chipSet.skills)
        {
            skill.skillAniDelegate += aniController.OnAttackObj;   //공격본 활성화함수 체인 구독 처리
            skill.skillAniDelegate += temp.PlayAttackAnimation;    //애니메이션 실행 코드 체인 구독 처리

            skill.attackAnimator = GetComponent<Animator>();
            skill.lineRenderer = GetComponent<LineRenderer>();
        }
    }

//-----------------------------------------------------------------------------------------유물 획득용---------------------------------------------------------------------------------------

    private Dictionary<int, RelicSO> relics = new Dictionary<int, RelicSO>();
    private int relicInstanceId = 0;
    public RelicSO[] _relics
    {
        get 
        {
            List<RelicSO> temp = new List<RelicSO>();
            foreach (var r in relics)
                temp.Add(r.Value);
            return temp.ToArray(); 
        }

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

            if (currentDarkMaterial + relic.darkMaterialCount <= PlayerStatus.maxDarkMaterialCount)  //최대용량을 넘지 않을 경우
            {
                //LogUtil.Log($"{relic.relicName}_유물 획득, 현재 암흑게이지수치: {currentDarkMaterial}, 현재 보유중인 유물개수: {relics.Count}");
                int relicInstanceID = AddRelic(relic);
                PlayerUIManager.instance.SetPlayerItem(relic, relicInstanceID);               //유물 이미지 할당
                PlayerUIManager.instance.ChangeDarkMaterialUI(true, relic.darkMaterialCount); //암흑물질 채워지는 UI연출
                AddRelicEffects(relic, relicInstanceID);                           //유물 데이터 추가

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

    private float currentDarkMaterial;   //플레이어 인벤토리 최대 용량

    public void SetSavedRelics(RelicSO[] relics)       //저장된 유물을 불러오는 함수 (저장 시스템용)
    {
        relicInstanceId = 0;
        foreach (var relic in relics)
        {
            AddRelic(relic);       //저장
            PlayerUIManager.instance.ChangeDarkMaterialUI(true, relic.darkMaterialCount); //암흑물질 채워지는 UI 갱신
            currentDarkMaterial += relic.darkMaterialCount;
        }
        foreach (var relic in this.relics)
            AddRelicEffects(relic.Value, relic.Key);
    }

    public void InitialInventoryDatas()              //인벤토리 초기화 (저장 시스템용)
    {
        relics.Clear();
        relicInstanceId = 0;

        currentDarkMaterial = 0;
        PlayerUIManager.instance.ResetDarkMaterialUI();   //암흑 물질 UI 초기화
        PlayerUIManager.instance.ClearPlayerItem();       //유물 인벤 UI 초기화

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

    public void RemoveRelic(RelicSO m_relicSO, int relicInstanceId)
    {
        PlayerUIManager.instance.ChangeDarkMaterialUI(false, m_relicSO.darkMaterialCount); //암흑물질 감소하는 UI연출
        currentDarkMaterial -= m_relicSO.darkMaterialCount;     //암흑물질 감소
        relics.Remove(relicInstanceId);                               //유물 데이터 삭제
        RelicEffectManager.ApplyRelicEffect(m_relicSO, false, relicInstanceId);  //유물 효과 제거
    }

    private void AddRelicEffects(RelicSO m_relicSO ,int relicInstanceId)        //유물 효과 실행부
    {
        RelicEffectManager.ApplyRelicEffect(m_relicSO, true, relicInstanceId);
    }

    private int AddRelic(RelicSO relicSO)
    {
        relicInstanceId++;
        relics.Add(relicInstanceId, relicSO);
        return relicInstanceId;
    }
}
