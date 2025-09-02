using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [Header("회복량")]
    public int healAmount = 1;

    [Header("수명")]
    public float lifeTime = 10f; // 일정 시간 지나면 사라짐

    private void Start()
    {
        // 일정 시간 뒤 자동 삭제
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어 체력 회복
            PlayerStatus.m_hp = Mathf.Min(PlayerStatus.m_hp + healAmount, PlayerStatus.m_maxhp);

            // UI 갱신 (필요하다면)
            if (PlayerUIManager.instance != null)
                PlayerUIManager.instance.ResetHpUI();

            // 하트 오브젝트 제거
            Destroy(gameObject);
        }
    }
}
