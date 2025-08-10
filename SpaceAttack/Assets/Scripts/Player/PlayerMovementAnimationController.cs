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
    public GameObject FrontMoveObj;
    public GameObject BackMoveObj;
    public GameObject SideMoveObj;

    private Animator frontMoveAnimator;
    private Animator backMoveAnimator;
    private Animator sideMoveAnimator;

    private MoveDirection moveDirection;
    private MoveDirection currentDirection;
    private PlayerStatus playerStatus;
    void Start()
    {
        frontMoveAnimator = FrontMoveObj.GetComponent<Animator>();
        backMoveAnimator = BackMoveObj.GetComponent<Animator>();
        sideMoveAnimator = SideMoveObj.GetComponent<Animator>();

        playerStatus = GetComponent<PlayerStatus>();
    }

    public void UpdateMoveDirection(float horizontal, float Vertical) //검사순서 --> 위,아래 --> 사이드 (이유: 위아래 애니메이션을 더 잘만들어서 )
    {
        if (Vertical > 0) moveDirection = MoveDirection.Back;         //위 입력이 있을 때, 이동방향 --> 뒤
        else if (Vertical < 0) moveDirection = MoveDirection.Front;   //아래 입력이 있을 때, 이동방향 --> 앞
        else if (Mathf.Abs(horizontal) > 0)                           //사이드 입력이 있을 때, 이동방향 --> 사이드
        {
            moveDirection = MoveDirection.Side;

            if (playerStatus == null) return;
            if ((horizontal > 0 && playerStatus.m_FacingRight) || (horizontal < 0 && !playerStatus.m_FacingRight)) //(입력 - 좌, 캐릭터 - 오) || (입력 - 우, 캐릭터 - 좌) --> 반전
            {
                playerStatus.Flip();
            }
        }

        if (currentDirection != moveDirection)  //방향이 바뀌었을 경우, 애니메이션오브젝트 활성화 여부 결정
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

        currentDirection = moveDirection;  //최신 방향 갱신
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
                    frontMoveAnimator.SetTrigger("Dash");
                }
                else if (moveDirection == MoveDirection.Back)
                {
                    backMoveAnimator.SetTrigger("Dash");
                }
                else if (moveDirection == MoveDirection.Side)
                {
                    sideMoveAnimator.SetFloat("DashSpeed", Mathf.Clamp(1.66f / dashDuration, 0.5f, 5f));   //1.66은 대쉬 애니메이션 Length | 0.3초동안 실행되게 설정
                    sideMoveAnimator.SetTrigger("Dash");

                    //Debug.Log(1.66f / dashDuration / 20));
                }
                break;

        }
    }

    private void ResetAnimationObj()    // 애니메이션 오브젝트들 초기화함수
    {
        FrontMoveObj.SetActive(false);
        SideMoveObj.SetActive(false);
        BackMoveObj.SetActive(false);
    }
}
