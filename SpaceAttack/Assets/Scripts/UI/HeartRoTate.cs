using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartRoTate : MonoBehaviour
{
    [Header("회전 속도 (Y축 기준)")]
    public float rotationSpeed = 90f; // 1초에 90도 회전

    void Update()
    {
        // Y축 기준으로 계속 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}
