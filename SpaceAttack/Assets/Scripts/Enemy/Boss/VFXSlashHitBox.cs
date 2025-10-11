using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSlashHitBox : MonoBehaviour
{
    [Header("발사체 설정")]
    public float speed = 10f;          // 이동 속도
    public float lifeTime = 5f;        // 자동 제거 시간

    [Header("데미지 수치")]
    public float damage = 10f;

    private Vector3 moveDir;
    private ParticleSystem ps;
    private float timer;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        timer = lifeTime;
    }

    // FireProjectile에서 방향을 받아옴
    public void SetDirection(Vector3 dir)
    {
        moveDir = dir.normalized;

        Quaternion rot = Quaternion.LookRotation(moveDir);

        rot *= Quaternion.Euler(-90f, 0f, 0f);

        transform.rotation = rot;
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            StopAndDestroy(); 
        }
    }

    //파티클 충돌처리
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
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
            StopAndDestroy();
        }
    }

    private void StopAndDestroy()
    {
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // 0.1초 후 루트 삭제
        StartCoroutine(DestroyRootNextFrame());
    }

    private IEnumerator DestroyRootNextFrame()
    {
        yield return null; 
        if (transform != null)
        {
            Transform root = transform.root;
            if (root != null)
            {
                Destroy(root.gameObject);
            }
        }
    }
}
