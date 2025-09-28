using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinProject : MonoBehaviour
{
    private GameObject warningZone;
    public int damage = 10;
    private bool hasCollided = false;

    public void Init(GameObject warning)
    {
        warningZone = warning;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어 맞았을 때 데미지 적용
            AttackInfo info = new AttackInfo
            {
                attacker = gameObject,
                damage = damage,
                attackDirection = (collision.transform.position - transform.position).normalized
            };

            PlayerStatus.Instance.ApplyDamage(info);

            // 장판 + 코인 제거
            if (warningZone != null)
                Destroy(warningZone);

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Plan"))
        {
            // 땅에 닿았을 때 데미지 없음, 그냥 제거
            if (warningZone != null)
                Destroy(warningZone);

            Destroy(gameObject);
        }
    }
}