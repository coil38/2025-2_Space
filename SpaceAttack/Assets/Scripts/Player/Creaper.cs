using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creaper : MonoBehaviour
{
    public float detectionRange = 5f;
    public float speed = 5f;
    public float explosionForce = 10f;
    public float explosionRadius = 2f;
    public LayerMask playerLayer;
    public Animator animator;

    private Transform player;
    private bool isCharging = false;

    void Update()
    {
        if (!isCharging)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
            if (hit != null)
            {
                player = hit.transform;
                isCharging = true;
                animator.SetTrigger("Run");
            }
        }
        else
        {
            if (player != null)
            {
                Vector2 direction = player.position - transform.position;
                if (direction.x != 0)
                {
                    GetComponent<SpriteRenderer>().flipX = direction.x < 0;
                }

                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

                float dist = Vector2.Distance(transform.position, player.position);
                if (dist < 1f)
                {
                    Explode();
                }
            }
        }
    }

    void Explode()
    {
        animator.SetTrigger("Explode");

        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (var hit in players)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
