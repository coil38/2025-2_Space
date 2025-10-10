using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testsd : MonoBehaviour
{
    [Header("생성할 이펙트 프리팹")]
    public GameObject slashEffectPrefab;

    [Header("이펙트 생성 위치 기준")]
    public Transform spawnPoint;

    [Header("발사 간격 (초)")]
    public float fireRate = 0.2f;

    private float lastFireTime;

    void Update()
    {
        // 스페이스바 누를 때
        if (Input.GetKey(KeyCode.G))
        {
            // fireRate 간격으로 반복 발사
            if (Time.time - lastFireTime > fireRate)
            {
                SpawnSlashEffect();
                lastFireTime = Time.time;
            }
        }
    }

    void SpawnSlashEffect()
    {
        if (slashEffectPrefab == null)
        {
            Debug.LogWarning("슬래시 이펙트 프리팹이 연결되지 않았습니다!");
            return;
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject newEffect = Instantiate(slashEffectPrefab, spawnPos, spawnRot);

        // 이펙트가 끝나면 자동으로 삭제 (파티클 재생시간 + 1초 정도)
        Destroy(newEffect, 3f);
    }
}
