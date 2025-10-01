using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanProjectile : MonoBehaviour
{
    [Header("데미지")]
    public int damage = 5;

    [Header("폭발 및 경고")]
    public float explodeDelay = 2f;                  // 바닥에 닿고 나서 폭발 전 대기시간
    public GameObject crossExplosionPrefab;          // 폭발 십자가 Prefab

    private Rigidbody rb;
    private bool hasCollided = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        rb.useGravity = true;
        rb.velocity = dir * 10f + Vector3.up * 5f; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        if (collision.gameObject.CompareTag("Plan"))
        {
            Debug.Log("Hit Plan! Creating cross explosion");

            transform.position = new Vector3(transform.position.x, collision.contacts[0].point.y, transform.position.z);
            rb.isKinematic = true;

            if (crossExplosionPrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.3f;
                GameObject cross = Instantiate(crossExplosionPrefab, spawnPos, Quaternion.identity);

                CrossExplosion explosion = cross.GetComponent<CrossExplosion>();
                if (explosion != null)
                {
                    explosion.damage = damage;
                    explosion.originCan = this;  
                }
            }
        }
    }
}
