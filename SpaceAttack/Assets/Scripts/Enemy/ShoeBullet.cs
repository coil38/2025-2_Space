using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeBullet : MonoBehaviour
{
    [Header("총알 정보")]
    public float damage = 1f;
    public float lifetime = 3f;      
    public float knockback = 2f;

    private Rigidbody rb;

    [Header("사운드 설정")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
    }
    public void Init(Vector3 velocity)
    {
        rb.velocity = velocity;

        bool isFacingRight = velocity.x > 0;

        BillBoard bb = GetComponent<BillBoard>();
        if (bb != null)
        {
            bb.SetFacingRight(isFacingRight);
        }

        Destroy(gameObject, lifetime);  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus ps = other.GetComponent<PlayerStatus>();
            if (ps)
            {
                AttackInfo info = new AttackInfo
                {
                    damage = damage,
                    attackDirection = transform.forward * knockback
                };
                ps.ApplyDamage(info);
            }

            if (hitSound)
            {
                GameObject soundObj = new GameObject("HitSound");
                AudioSource source = soundObj.AddComponent<AudioSource>();
                source.clip = hitSound;
                source.Play();
                Destroy(soundObj, hitSound.length);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
