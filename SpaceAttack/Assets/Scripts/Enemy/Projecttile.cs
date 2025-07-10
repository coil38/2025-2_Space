using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projecttile : MonoBehaviour
{
    public float speed = 5f;
    public float maxLifeTime = 5f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    public float damage = 1f;  

    public void Init(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = moveDirection * speed;
        Destroy(gameObject, maxLifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.collider.CompareTag("Player"))
        //{
        //    PlayerStatus playerStatus = collision.collider.GetComponent<PlayerStatus>();   //오류가 나서 주석처리함
        //    if (playerStatus != null)
        //    {
        //        playerStatus.ApplyDamage(damage);  // 데미지를 적용
        //    }
        //    Destroy(gameObject);
        //}
        //else if (collision.collider.CompareTag("Wall"))
        //{
        //    Destroy(gameObject);
        //}
    }
}
