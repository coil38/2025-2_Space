using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHPUI : MonoBehaviour
{
    [Header("HPSlotPrefab")]
    [SerializeField] private GameObject HPSlot;       //체력 슬롯 프리팹
    [SerializeField] private GameObject ShildHpSlot;  //방어막 체력 슬롯 프리팹

    [Header("ReduceHPAnimation")]
    [SerializeField] private float r_ani_PunchScale;
    [SerializeField] private float r_ani_Duration = 0.3f;
    [SerializeField] private int r_ani_repeatCount = 5;

    private Queue<Transform> HpSlots = new Queue<Transform>();
    private Queue<Transform> ShildSlots = new Queue<Transform>();

    void Start()
    {
        GenerateHPSlot();
    }

    public void ReduceHPUI(int hp, int shild_hp, int damage)        //체력UI 감소 함수
    {
        int damageCount = damage;          //줄어들 데미지 횟수

        while (damageCount > 0 && hp > 0)
        {
            Transform target = this.transform;
            bool halfHeartExist = hp % 2 == 1; //절반 체력 존재여부

            if (shild_hp > 0)
            {
                halfHeartExist = shild_hp % 2 == 1;  //절반 쉴드 존재여부
                foreach (var shildSlot in ShildSlots)
                {
                    if (shildSlot.GetChild(2).gameObject.activeSelf || shildSlot.GetChild(1).gameObject.activeSelf)  //절반 혹은 최대 체력이 활성화 되었을 때
                    {
                        target = shildSlot;  //마지막 대상을 갱신시킨다.
                    }
                }
                //LogUtil.Log($"쉴드 방어_반칸 : 현재 쉴드: {shild_hp}");
            }
            else
            {
                halfHeartExist = hp % 2 == 1; //절반 체력 존재여부

                foreach (var hpSlot in HpSlots)
                {
                    if (hpSlot.GetChild(2).gameObject.activeSelf || hpSlot.GetChild(1).gameObject.activeSelf)  //절반 혹은 최대 체력이 활성화 되었을 때
                    {
                        target = hpSlot;  //마지막 대상을 갱신시킨다.
                    }
                }
                //LogUtil.Log("체력 방어_반칸");
            }

            if (target != this.transform && halfHeartExist)  //절반 체력이 존재한다면
            {
                target.GetChild(1).gameObject.SetActive(false);
                ChangeUIAnimation(target, true);             //체력감소 애니메이션 재생
            }
            else if (target != this.transform && !halfHeartExist) //풀 체력만 존재한다면
            {
                target.GetChild(1).gameObject.SetActive(true);
                target.GetChild(2).gameObject.SetActive(false);
                ChangeUIAnimation(target, true);             //체력감소 애니메이션 재생
            }

            hp--;
            damageCount--;
            shild_hp--;
        }
    }

    private void ChangeUIAnimation(Transform target, bool isReducing)
    {
        if (isReducing)  //체력이 감소하는 연출일 경우 랜덤으로 체력이미지가 흔들리는 연출
        {
            float random_x = Random.Range(-1f, 1f);
            float random_y = Random.Range(-1f, 1f);

            Vector3 randomVector = new Vector2(random_x, random_y);
            randomVector = randomVector.normalized * r_ani_PunchScale;

            target.DOPunchPosition(randomVector, r_ani_Duration, r_ani_repeatCount)
                  .OnComplete(() => target.transform.localPosition = Vector3.zero);
        }
    }

    public void GenerateHPSlot()             //체력UI 재구성 함수
    {
        int maxHp = PlayerStatus.m_maxhp;
        int hp = PlayerStatus.m_hp;
        int shildHp = PlayerStatus.shild_hp;

        int maxHeartCount = maxHp / 2 + (maxHp % 2 == 1 ? 1 : 0);

        int heartCount = hp / 2;
        bool halfHeartExist = hp % 2 == 1;

        int currentHeartCount = HpSlots.Count;

        for (int i = 0; i < maxHeartCount; i++)  //빈어있는 최대체력 생성
        {
            if (currentHeartCount <= 0)
            {
                GameObject hpSlot = Instantiate(HPSlot);
                hpSlot.transform.SetParent(this.transform);

                Transform hpImage = hpSlot.transform.GetChild(0);  // 1은 절반 체력, 2는 가득찬 체력 : 이들을 비활성화
                hpImage.GetChild(1).gameObject.SetActive(false);
                hpImage.GetChild(2).gameObject.SetActive(false);

                HpSlots.Enqueue(hpImage);   //이미 만들어진 체력슬롯 재사용
            }
            else
            {
                if (HpSlots.TryPeek(out var result))
                {
                    result.GetChild(1).gameObject.SetActive(false);
                    result.GetChild(2).gameObject.SetActive(false);
                }
            }

            currentHeartCount--;
        }

        foreach (var hpSlot in HpSlots)
        {
            if (heartCount <= 0 && !halfHeartExist) break;
            else if (heartCount <= 0 && halfHeartExist)
            {
                hpSlot.GetChild(1).gameObject.SetActive(true);   //절반 체력을 활성화 시킨다
                break;
            }

            hpSlot.GetChild(2).gameObject.SetActive(true);   //풀체력을 활성화 시킨다
            heartCount--;
        }

        int maxShildCount = shildHp / 2 + (shildHp % 2 == 1 ? 1 : 0);
        int shildCount = shildHp / 2;
        bool halfShildExist = shildHp % 2 == 1;

        int currentShildCount = ShildSlots.Count;

        for (int i = 0; i < maxShildCount; i++)
        {
            if (currentShildCount <= 0)
            {
                GameObject shild = Instantiate(ShildHpSlot);
                shild.transform.SetParent(this.transform);

                Transform shildImage = shild.transform.GetChild(0);  // 1은 절반 체력, 2는 가득찬 체력 : 이들을 비활성화
                shildImage.GetChild(1).gameObject.SetActive(false);
                shildImage.GetChild(2).gameObject.SetActive(false);

                ShildSlots.Enqueue(shildImage);   //이미 만들어진 체력슬롯 재사용
            }
            else
            {
                if (ShildSlots.TryPeek(out var result))
                {
                    result.GetChild(1).gameObject.SetActive(false);
                    result.GetChild(2).gameObject.SetActive(false);
                }
            }
            currentShildCount--;
        }

        foreach (var shildSlot in ShildSlots)
        {
            if (shildCount <= 0 && !halfShildExist) break;
            else if (shildCount <= 0 && halfShildExist)
            {
                shildSlot.GetChild(1).gameObject.SetActive(true);   //절반 체력을 활성화 시킨다
                break;
            }

            shildSlot.GetChild(2).gameObject.SetActive(true);   //풀체력을 활성화 시킨다
            shildCount--;
        }
    }


}
