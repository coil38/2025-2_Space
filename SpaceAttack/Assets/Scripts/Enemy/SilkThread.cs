using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilkThread : MonoBehaviour
{
    private Transform target;
    private float damage = 1f;

    public float speed = 10f;
    public float lifeTime = 5f;

    private bool isRed; 

    public float maxHealth = 1f;
    private float currentHealth;

    [SerializeField] private AudioClip hitClip;     // 터질 때 소리
  
    public void Init(Transform target, float damage, bool isRed, float speed)
    {
        this.target = target;
        this.damage = damage;
        this.isRed = isRed;
        this.speed = speed;   
    }

    private void Start()
    {
        currentHealth = maxHealth;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        transform.up = dir;
    }

    public void ApplyDamage(AttackInfo info)
    {
        currentHealth -= info.damage;

        if (currentHealth <= 0f)
        {
            if (hitClip != null)
                AudioSource.PlayClipAtPoint(hitClip, transform.position);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        AttackInfo info = new AttackInfo { damage = this.damage };

        PlayerStatus p = other.GetComponent<PlayerStatus>();
        if (p != null)
            p.ApplyDamage(info);

        if (hitClip != null)
            AudioSource.PlayClipAtPoint(hitClip, transform.position);

        Destroy(gameObject);
    }

}
