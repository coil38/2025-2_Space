using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private bool facingRight = true;  // 기본 오른쪽 방향

    public void SetFacingRight(bool right)
    {
        facingRight = right;
    }

    void LateUpdate()
    {
        // 카메라를 바라보게 하면서 Y축만 0도 또는 180도로 좌우 뒤집기
        transform.forward = Camera.main.transform.forward;

        if (!facingRight)
        {
            // Y축 180도 회전해서 좌우 반전
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
