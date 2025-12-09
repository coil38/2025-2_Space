using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection
{
    Front,
    Back,
    Side
}

public class PlayerMovementAnimationController : MonoBehaviour
{
    [Header("애니메이션 오브젝트들")]
    public PlayerAniRenderer FrontMoveRenderer;
    public PlayerAniRenderer BackMoveRenderer;
    public PlayerAniRenderer SideMoveRenderer;
    public PlayerAniRenderer AttackMoveRenderer;

    [Header("무기 오브젝트들")]
    public GameObject SwordObj;

    private Animator frontMoveAnimator;
    private Animator backMoveAnimator;
    private Animator sideMoveAnimator;

    private MoveDirection moveDirection;
    public MoveDirection currentDirection { get; private set; }
    private PlayerStatus playerStatus;

    [HideInInspector] public Animator attackAnimator;

    void Start()
    {
        frontMoveAnimator = FrontMoveRenderer.GetComponent<Animator>();
        backMoveAnimator = BackMoveRenderer.GetComponent<Animator>();
        sideMoveAnimator = SideMoveRenderer.GetComponent<Animator>();

        playerStatus = GetComponent<PlayerStatus>();
    }

    public void UpdateMoveDirection(float horizontal, float Vertical) //검사순서 --> 위,아래 --> 사이드 (이유: 위아래 애니메이션을 더 잘만들어서 )
    {
        if (Vertical > 0) //위 입력이 있을 때, 이동방향 --> 뒤
        {
            if (moveDirection == MoveDirection.Side && Mathf.Abs(horizontal) > 0.2f)
            {
                if(Vertical > 0.4f) moveDirection = MoveDirection.Back; //예외처리(사이드 이동중, 위 이동시, 바로 전환 안됨
            }
            else moveDirection = MoveDirection.Back;
        }
        else if (Vertical < 0) //아래 입력이 있을 때, 이동방향 --> 앞
        {
            if (moveDirection == MoveDirection.Side && Mathf.Abs(horizontal) > 0.2f)
            {
                if(Vertical < - 0.4f) moveDirection = MoveDirection.Front;
            }
            else moveDirection = MoveDirection.Front;
        }
        else if (Mathf.Abs(horizontal) > 0)                           //사이드 입력이 있을 때, 이동방향 --> 사이드
        {
            moveDirection = MoveDirection.Side;

            if (playerStatus == null) return;
            if ((horizontal > 0 && playerStatus.m_FacingRight) || (horizontal < 0 && !playerStatus.m_FacingRight)) //(입력 - 좌, 캐릭터 - 오) || (입력 - 우, 캐릭터 - 좌) --> 반전
            {
                playerStatus.Flip();
            }
        }

        if (PlayerMoveAniCondition.IsAnimating()) return; //현재 공격중일 경우, 리턴처리

        if (currentDirection != moveDirection)
        {
            SetDirection(); //방향이 바뀌었을 경우, 애니메이션오브젝트 활성화 여부 결정
        }
        else if (PlayerMoveAniCondition.IsResetAni())
        {
            SetDirection();
            //LogUtil.Log("자자자자자자자자자자ㅏㄱㄱㄱㄱㄱㄱㄱㄱ");
        }

        currentDirection = moveDirection;  //최신 방향 갱신
    }

    private void SetDirection()
    {
        ResetAnimationObj();
        switch (moveDirection)
        {
            case MoveDirection.Front:
                FrontMoveRenderer.ChangeRenderersAlapha(1);
                break;
            case MoveDirection.Back:
                BackMoveRenderer.ChangeRenderersAlapha(1);
                break;
            case MoveDirection.Side:
                SideMoveRenderer.ChangeRenderersAlapha(1);
                break;
        }
    }

    public void PlayAnimation(string name, float horizontal = 0, float Vertical = 0, float dashDuration = 0)
    {
        switch (name)
        {
            case "Move":
                if (moveDirection == MoveDirection.Front) frontMoveAnimator.SetFloat("Move", Mathf.Abs(Vertical));
                else if (moveDirection == MoveDirection.Back) backMoveAnimator.SetFloat("Move", Mathf.Abs(Vertical));
                else if (moveDirection == MoveDirection.Side) sideMoveAnimator.SetFloat("Move", Mathf.Abs(horizontal));
                break;

            case "Dash":
                if (moveDirection == MoveDirection.Front)
                {
                    frontMoveAnimator.SetFloat("DashSpeed", Mathf.Clamp(2f / dashDuration - 0.05f, 0.5f, 20f));
                    frontMoveAnimator.SetTrigger("Dash");
                }
                else if (moveDirection == MoveDirection.Back)
                {
                    backMoveAnimator.SetFloat("DashSpeed", Mathf.Clamp(2f / dashDuration - 0.05f, 0.5f, 20f));
                    backMoveAnimator.SetTrigger("Dash");
                }
                else if (moveDirection == MoveDirection.Side)
                {
                    sideMoveAnimator.SetFloat("DashSpeed", Mathf.Clamp(1.66f / dashDuration - 0.05f, 0.5f, 20f));   //1.66은 대쉬 애니메이션 Length | 0.3초동안 실행되게 설정
                    sideMoveAnimator.SetTrigger("Dash");

                    //Debug.Log(1.66f / dashDuration / 20));
                }
                break;

            case "Hit":
                moveDirection = MoveDirection.Front;
                SetDirection();
                frontMoveAnimator.SetFloat("HitSpeed", Mathf.Clamp(0.667f / PlayerTimeSystem.m_stunTime - 0.05f, 0.6f, 20f));
                frontMoveAnimator.SetTrigger("Hit");
                break;

            case "Dead":
                moveDirection = MoveDirection.Front;
                SetDirection();
                frontMoveAnimator.SetTrigger("Dead");
                break;
        }
    }

    public void EndDeadAndRestart()
    {
        moveDirection = MoveDirection.Front;
        SetDirection();
        frontMoveAnimator.SetTrigger("IsReStart");
    }

    public void ResetAni()     //속박상태 해제에 사용
    {
        PlayerMoveAniCondition.EndAni();
        moveDirection = MoveDirection.Front;
        SetDirection();
    }

    public void SetAnimator(Animator animator, string chipsetName, bool isAdding)  //공격본에 애니메이터 설정
    {
        if (isAdding)
        {
            AttackMoveRenderer.GetComponent<Animator>().runtimeAnimatorController = animator.runtimeAnimatorController;
            attackAnimator = AttackMoveRenderer.GetComponent<Animator>();

            ResetWeaponObj();
            switch (chipsetName)
            {
                case "Warrior":
                    SwordObj.SetActive(true);
                    break;
                case "Archer":
                    Debug.Log("궁수 애니메이션 연결_완");
                    break;
            }
        }
        else
        {
            AttackMoveRenderer.GetComponent<Animator>().runtimeAnimatorController = null;
            attackAnimator = null;

            ResetWeaponObj();  //무기 초기화
        }
    }

    public void OnAttackObj(PlayerAniInfo _aniInfo)   //공격할시, 실행됨 (이벤트 체인)
    {
        if (PlayerMoveAniCondition.IsAnimating()) return; //현재 공격중일 경우, 리턴처리
        PlayerMoveAniCondition.StartAni();
        ResetAnimationObj();
        AttackMoveRenderer.ChangeRenderersAlapha(1);
    }

    public void ResetAnimationObj()    // 애니메이션 오브젝트들 초기화함수
    {
        FrontMoveRenderer.ChangeRenderersAlapha(0);
        SideMoveRenderer.ChangeRenderersAlapha(0);
        BackMoveRenderer.ChangeRenderersAlapha(0);
        AttackMoveRenderer.ChangeRenderersAlapha(0);

        Vector3 initialPos = new Vector3(0, -1, 0);
        FrontMoveRenderer.transform.localPosition = initialPos;
        SideMoveRenderer.transform.localPosition = initialPos;
        BackMoveRenderer.transform.localPosition = initialPos;
        AttackMoveRenderer.transform.localPosition = initialPos;

        FrontMoveRenderer.transform.localRotation = Quaternion.identity;
        SideMoveRenderer.transform.localRotation = Quaternion.identity;
        BackMoveRenderer.transform.localRotation = Quaternion.identity;
        AttackMoveRenderer.transform.localRotation = Quaternion.identity;
    }

    public void ResetAttackAnimation()
    {
        PlayerMoveAniCondition.EndAni();
    }

    private void ResetWeaponObj()
    {
        SwordObj.SetActive(false);
        //추가 예정
    }
}
