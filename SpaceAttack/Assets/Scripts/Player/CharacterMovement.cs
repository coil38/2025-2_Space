using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private PlayerStatus playerState;
    private InventoryManager inventory;
    private Rigidbody rb;
    private Animator animator;

    private Vector3 currentDir = Vector3.zero;
    private Vector3 velocity;
    private Coroutine currentCor;

    private bool isMoving;
    private LayerMask wallLayer;
    private LayerMask itemLayer;
    private void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<InventoryManager>();

        wallLayer |= 1 << LayerMask.NameToLayer("Wall");
        itemLayer |= 1 << LayerMask.NameToLayer("Item");
    }

    public void Move()  //플레이어 이동
    {
        float horizotal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(horizotal, 0, vertical).normalized;

        if (dir != Vector3.zero) currentDir = dir.normalized;  //현재 방향값이 0이 아닐 때만 전달
        else
        {
            if (currentCor == null)
            {
                currentCor = StartCoroutine(E_ChangeCurrentDir());
            }
        }

        if (dir.magnitude > 0.1f && !isMoving)
        {
            isMoving = true;
            playerState.m_Particle.Play();   //이동하기 시작하면 파티클 재생

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound("Run");
        }
        else if(dir.magnitude < 0.1f && isMoving)
        {
            isMoving = false;
            playerState.m_Particle.Stop();       //이동이 멈추면 파티클 종료

            if (AudioManager.instance != null)
                AudioManager.instance.PauseSound("Run");
        }

        if ((horizotal < 0 && playerState.m_FacingRight) || (horizotal > 0 && !playerState.m_FacingRight)) //(입력 - 좌, 캐릭터 - 오) || (입력 - 우, 캐릭터 - 좌) --> 반전
        {
            //Debug.Log($"실행된다.");
            playerState.Flip();
        }

        animator.SetFloat("Horizontal", horizotal);               //애니메이션 파리미터 전달
        animator.SetFloat("Vertical", vertical);

        rb.MovePosition(rb.position + dir * playerState.m_speed * Time.deltaTime);   //플레이어 이동
    }

    public void Dash()  //대쉬
    {
        isMoving = false;
        playerState.m_Particle.Stop();    //이동이 멈추면 파티클 종료
        playerState.d_Particle.Play();    //대쉬할 경우, 파티클 재생

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PauseSound("Run");
            AudioManager.instance.PlaySound("Dash");
        }

        animator.SetBool("IsDashing", true);

        float dashDur = playerState.m_DashDruation;

        //if (Physics.Raycast(transform.position, currentDir, out RaycastHit hit, dashDur, wallLayer))   //벽이 있을 경우의 예외처리(이동거리, 이동시간)
        //{
        //    dashDur = Vector3.Distance(hit.point, transform.position) - 0.25f;
        //    dashDur = Mathf.Max(dashDur, 0f);
        //}

        //Debug.Log(dashDur);

        TimeSystem.w_dashTimer.Start();   //대쉬 타이머 시작 (0.15 초 동안)
        //TimeSystem.invincibilityTimer.Start();  //대쉬 후, 잠시동안 무적 시작 (0.1 동안)
        TimeSystem.deshTimer.Start();     //대쉬 대기 시간 (0.1 초 동안)

        if(dashDur >= 0.5f)
            StartCoroutine(PlayerDash(dashDur));
    }

    private IEnumerator PlayerDash(float dashDur)
    {
        Timer dashTimer = TimeSystem.deshTimer;
        float dashTime = TimeSystem.m_DashTime;

        Vector3 currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPos = currentPos + currentDir.normalized * dashDur;

        while (true)
        {
           float timer = dashTimer.GetRemainingTimer() / dashTime;

            Vector3 move = Vector3.Lerp(currentPos, targetPos, 1 - timer);
            rb.MovePosition(move);

            if (timer <= 0.1f)
            {
                break;
            }
            yield return null;
        }
    }

    private IEnumerator E_ChangeCurrentDir()
    {
        yield return new WaitForSeconds(0.1f);
        currentDir = - Vector3.forward;
        currentCor = null;
    }

    public void CheckItem()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, playerState.itemDetectDistance, itemLayer);

        foreach (var item in items)
        {
            ChipSetType chipset = item.gameObject.GetComponent<ChipSetType>();

            if (chipset != null) // 감지 대상이 칩셋이면 칩셋받기
            {
                inventory.chipSet = chipset;
                //Destroy(item.gameObject, 1f);
                Debug.Log("아이템 획득");
            }
        }
    }
}
