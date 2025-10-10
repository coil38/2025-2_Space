using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSlashHitBox : MonoBehaviour
{
    [Header("데미지 수치")]
    public float damage = 10f;

    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Unity가 자동으로 호출하는 메서드 (Send Collision Messages가 켜져 있어야 함)
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어와 파티클 충돌 감지됨!");

            AttackInfo info = new AttackInfo
            {
                damage = damage,
                attackDirection = (other.transform.position - transform.position).normalized,
                attacker = this.gameObject
            };

            PlayerStatus.Instance.ApplyDamage(info);

            StopAndDestroy();
        }
        else if (other.CompareTag("Wall"))
        {
            Debug.Log("벽과 파티클 충돌 감지됨!");
            StopAndDestroy();
        }
    }

    private void StopAndDestroy()
    {
        if (ps != null)
        {
            ps.Stop(); // 파티클 방출 중지
        }
        Destroy(gameObject); // 잠깐 잔상 남기고 삭제
    }
}
