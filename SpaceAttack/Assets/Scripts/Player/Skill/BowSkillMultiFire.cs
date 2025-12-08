using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class BowSkillMultiFire : SkillType
{
    public GameObject arrowPrf;             //화살 프리팹

    private float fireAngle = 15f;          //발사 각도

    public override void OnEnable()
    {
        unLockedNumber = 2;
        chipsetCompID = 109;
        base.OnEnable();
    }

    public override void UpdateInfo() { base.UpdateInfo(); }

    public override void CheckUse(Vector3 currentPos)
    {
        //if (!isUnLocked) return;                                      //해금여부에 따른 스킬 사용 여부

        _currentPos = currentPos;

        if (PlayerInputController.skill2Action.triggered)  //플레이어 입력감지
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            if (coolTimer.IsRunning()) return;                      //쿨타임 체크

            //PlayerTimeSystem.SetChipTimer(0.2f, ChipAttackType.Skill);
            //PlayerTimeSystem.w_SkillTimer.Start();                 //다음 공격 전 대기 체크 시작

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
        Vector3[] directions = new Vector3[5]
        {
            attackDirection,
            Quaternion.Euler(0f, fireAngle, 0f) * attackDirection,
            Quaternion.Euler(0f, - fireAngle, 0f) * attackDirection,
            Quaternion.Euler(0f, + 2 * fireAngle, 0f) * attackDirection,
            Quaternion.Euler(0f, - 2 * fireAngle, 0f) * attackDirection
        };

        Vector3[] startPoses = new Vector3[5];
        for(int i = 0; i < startPoses.Length; i++)
            startPoses[i] = transform.position + directions[i] * 0.3f;

        OnVisualAttackRanges(startPoses, attackDistance, 0.5f, directions, attackTime);
        for (int z = 0; z < startPoses.Length; z++)
        {
            LogUtil.Log("화살 방향: " + directions[z]);
            FireArrow(startPoses[z] , directions[z]);
        }
    }

    private void FireArrow(Vector3 startPos, Vector3 fireDirection)
    {
        if (arrowPrf == null) return;

        Quaternion quaternion = Quaternion.LookRotation(fireDirection, Vector3.up);

        GameObject arrow = Instantiate(arrowPrf, startPos, quaternion);
        if (arrow != null)
        {
            LogUtil.Log("화살이 생성 되었습니다.");
            arrow.GetComponent<WeaponArrow>().Fire(fireDirection, 15f, damageRate, attackDistance, addedCritChanceRate, addedCritRate, chipset, ChipAttackType.Skill);
        }
    }
}
