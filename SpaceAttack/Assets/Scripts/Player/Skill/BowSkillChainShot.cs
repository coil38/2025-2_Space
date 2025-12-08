using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSkillChainShot : SkillType
{
    public GameObject arrowPrf;             //화살 프리팹
    public GameObject guidedArrowPrf;       //유도 화살 프리팹

    private float fireCount = 5f;          //튕겨지는 횟수
    private float detectDistance = 6f;     //튕겨지는 것을 감지하는 거리

    public override void OnEnable()
    {
        unLockedNumber = 1;
        chipsetCompID = 108;
        base.OnEnable();
    }

    public override void UpdateInfo() { base.UpdateInfo(); }

    public override void CheckUse(Vector3 currentPos)
    {
        //if (!isUnLocked) return;                                      //해금여부에 따른 스킬 사용 여부

        _currentPos = currentPos;

        if (PlayerInputController.skill1Action.triggered)  //플레이어 입력감지
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            if (coolTimer.IsRunning()) return;                      //쿨타임 체크

            PlayerTimeSystem.SetChipTimer(0.2f, ChipAttackType.Skill);
            PlayerTimeSystem.w_SkillTimer.Start();                 //다음 공격 전 대기 체크 시작

            isAttacking = true;
            coolTimer.Start();         //쿨타임 시작

            PlayerAniInfo aniInfo = new PlayerAniInfo("isBowAttacking", AniType.Trrigger, 1f / 0.3f);  //공격 애니메이션 실행
            PlayAniMation(aniInfo);
            // 사운드 추가

            PlayerTimeSystem.w_BaseAttackTimer.Start();                                 //공격 타이머 시작

            attackDirection = GetAttackDirection(currentPos);   //플레이어 기준 마우스 방향 얻기

            Use();
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void Use()
    {
        StartCoroutine(StartFire());
    }

    private IEnumerator StartFire()
    {
        float count = fireCount;
        currentHittedTarget = null;
        genPos = transform.position;

        detectLayer |= (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Boss"));

        while (count > 0)
        {
            GameObject arrow = null;

            if (arrowPrf == null) yield break;

            Quaternion quaternion = Quaternion.LookRotation(attackDirection, Vector3.up);

            if (count == fireCount)
            {
                Vector3 startPos = genPos + attackDirection * 0.3f;
                OnVisualAttackRange(startPos, attackDistance, 0.5f, attackDirection, attackTime);

                arrow = Instantiate(arrowPrf, startPos, quaternion);
                arrow.GetComponent<WeaponArrow>().SetEvent(SetHittedTarget);
                arrow.GetComponent<WeaponArrow>().Fire(attackDirection, 10f, damageRate, attackDistance, addedCritChanceRate, addedCritRate, chipset, ChipAttackType.Skill);
            }
            else
            {
                Collider[] cols = Physics.OverlapSphere(genPos, detectDistance, detectLayer);
                foreach (var col in cols)
                {
                    if (currentHittedTarget != col.gameObject)
                    {
                        //LogUtil.Log("감지된 대상: " + col.gameObject.name);

                        arrow = Instantiate(guidedArrowPrf, genPos, quaternion);
                        arrow.GetComponent<WeaponGuidedArrow>()
                            .Fire(attackDirection, 10f, damageRate, attackDistance, addedCritChanceRate, addedCritRate, chipset, ChipAttackType.Skill, col.gameObject, currentHittedTarget);

                        currentHittedTarget = col.gameObject;
                        genPos = col.transform.position;
                        break;
                    }
                }
                //LogUtil.Log("감지 종료");
            }
            yield return new WaitUntil(() => arrow.gameObject == null);
            count--;
            //LogUtil.Log("현재 횟수" + count);

            if (currentHittedTarget == null)
            {
                //LogUtil.Log("현재 횟수" + count + "피격 대상 없음 및 종료");
                yield break; //현재 피격된 대상이 없을 경우, 체인샷 종료
            }
        }
        //LogUtil.Log("체인 공격 종료");
    }

    private GameObject currentHittedTarget;
    private Vector3 genPos;
    LayerMask detectLayer;
    private void SetHittedTarget(GameObject target)
    {
        currentHittedTarget = target;
        genPos = target.transform.position;
    }
}
