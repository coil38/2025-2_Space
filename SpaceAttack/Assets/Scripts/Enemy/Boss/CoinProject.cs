using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinProject : MonoBehaviour
{
    private GameObject warningZone;
    public int damage = 10;
    private bool hasCollided = false;
    private bool soundPlayed = false;



    public void Init(GameObject warning)
    {
        warningZone = warning;
    }

    private void Update()
    {
        if (hasCollided || soundPlayed) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2.5f))
        {
            if (hit.collider.CompareTag("Plan")) 
            {
                soundPlayed = true;
                CoinSoundManager.Instance.PlayGroundHit(); 
            }
        }
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

            CoinSoundManager.Instance.PlayPlayerHit(); 

            if (warningZone != null)
                Destroy(warningZone);

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Plan"))
        {
            if (warningZone != null)
                Destroy(warningZone);

            Destroy(gameObject);
        }
    }
    public void PlayFallSound()
    {
        CoinSoundManager.Instance.PlayGroundHit();
    }
}