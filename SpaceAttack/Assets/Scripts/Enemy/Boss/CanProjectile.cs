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
        rb.velocity = dir * 4f + Vector3.up * 2.5f; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        // PLAN에서만 처리
        if (collision.gameObject.CompareTag("Plan"))
        {
            if (hasCollided) return;
            hasCollided = true;

            transform.position = new Vector3(
                transform.position.x,
                collision.contacts[0].point.y,
                transform.position.z);

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

    public void InitRandomDirection(float force)
    {
        rb.useGravity = true;

        float randY = Random.Range(0f, 360f);
        Vector3 dir = Quaternion.Euler(0, randY, 0) * Vector3.forward;

        rb.velocity = dir * force + Vector3.up * 3.5f;
    }
}
