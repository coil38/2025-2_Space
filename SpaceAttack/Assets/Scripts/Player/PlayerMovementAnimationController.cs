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
    public GameObject FrontMoveObj;
    public GameObject BackMoveObj;
    public GameObject SideMoveObj;
    public GameObject AttackObj;

    [Header("무기 오브젝트들")]
    public GameObject SwordObj;

    private Animator frontMoveAnimator;
    private Animator backMoveAnimator;
    private Animator sideMoveAnimator;

    private MoveDirection moveDirection;
    private MoveDirection currentDirection;
    private PlayerStatus playerStatus;

    private bool isAttacking;   //공격여부 내부 변수
    [HideInInspector] public Animator attackAnimator;

    void Start()
    {
        frontMoveAnimator = FrontMoveObj.GetComponent<Animator>();
        backMoveAnimator = BackMoveObj.GetComponent<Animator>();
        sideMoveAnimator = SideMoveObj.GetComponent<Animator>();

        playerStatus = GetComponent<PlayerStatus>();
    }

    void Update()
    {
        if (PlayerEndParamBehaviour.isEndAttack)  //모든 칩셋의 공격 이름 BaseAttack으로 통일 ( 항상 반복 )
        {
            PlayerEndParamBehaviour.isEndAttack = false;

            if (isAttacking)   //공격 중일 때만 종료처리
            {
                AttackObj.SetActive(false);
                isAttacking = false;

                ResetAnimationObj();
                SideMoveObj.SetActive(true);  //방향설정
            }
        }
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

        if (isAttacking) return; //현재 공격중일 경우, 리턴처리

        if (currentDirection != moveDirection)  //방향이 바뀌었을 경우, 애니메이션오브젝트 활성화 여부 결정
        {
            SetDirection();
        }

        currentDirection = moveDirection;  //최신 방향 갱신
    }

    public void SetDirection()
    {
        ResetAnimationObj();
        switch (moveDirection)
        {
            case MoveDirection.Front:
                FrontMoveObj.SetActive(true);
                break;
            case MoveDirection.Back:
                BackMoveObj.SetActive(true);
                break;
            case MoveDirection.Side:
                SideMoveObj.SetActive(true);
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
                    frontMoveAnimator.SetFloat("DashSpeed", Mathf.Clamp(2f / dashDuration, 0.5f, 5f));
                    frontMoveAnimator.SetTrigger("Dash");
                }
                else if (moveDirection == MoveDirection.Back)
                {
                    backMoveAnimator.SetFloat("DashSpeed", Mathf.Clamp(2f / dashDuration, 0.5f, 5f));
                    backMoveAnimator.SetTrigger("Dash");
                }
                else if (moveDirection == MoveDirection.Side)
                {
                    sideMoveAnimator.SetFloat("DashSpeed", Mathf.Clamp(1.66f / dashDuration, 0.5f, 5f));   //1.66은 대쉬 애니메이션 Length | 0.3초동안 실행되게 설정
                    sideMoveAnimator.SetTrigger("Dash");

                    //Debug.Log(1.66f / dashDuration / 20));
                }
                break;

            case "Hit":

                isAttacking = false;  //공격도 초기화 처리
                moveDirection = MoveDirection.Front;
                SetDirection();
                frontMoveAnimator.SetFloat("HitSpeed", Mathf.Clamp(0.667f / PlayerTimeSystem.m_stunTime - 0.05f, 0.6f, 5f));
                frontMoveAnimator.SetTrigger("Hit");
                break;
        }
    }

    public void SetAnimator(Animator animator, string chipsetName, bool isAdding)  //공격본에 애니메이터 설정
    {
        if (isAdding)
        {
            AttackObj.GetComponent<Animator>().runtimeAnimatorController = animator.runtimeAnimatorController;
            attackAnimator = AttackObj.GetComponent<Animator>();

            ResetWeaponObj();
            switch (chipsetName)
            {
                case "Warrior":
                    SwordObj.SetActive(true);
                    break;
            }
        }
        else
        {
            AttackObj.GetComponent<Animator>().runtimeAnimatorController = null;
            attackAnimator = null;

            ResetWeaponObj();  //무기 초기화
        }
    }

    public void OnAttackObj(PlayerAniInfo _aniInfo)
    {
        isAttacking = true;  //공격활성화 처리
        ResetAnimationObj();
        AttackObj.SetActive(true);
    }

    public void ResetAnimationObj()    // 애니메이션 오브젝트들 초기화함수
    {
        FrontMoveObj.SetActive(false);
        SideMoveObj.SetActive(false);
        BackMoveObj.SetActive(false);
        AttackObj.SetActive(false);
    }

    public void ResetAttackAnimation()
    {
        isAttacking = false;
    }

    private void ResetWeaponObj()
    {
        SwordObj.SetActive(false);
        //추가 예정
    }
}
