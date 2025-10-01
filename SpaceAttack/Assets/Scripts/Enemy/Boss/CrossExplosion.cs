using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossExplosion : MonoBehaviour
{
    public int damage = 5;
    public float duration = 0.5f; // 폭발 표시 시간
    public float crossLength = 3f;
    public float crossWidth = 1f;

    [HideInInspector] public CanProjectile originCan; // 캔 참조

    [Header("사운드")]
    public AudioClip explosionSound; // 폭발 소리
    public float volume = 1f;

    private void Start()
    {
        StartCoroutine(ExplosionRoutine());
    }

    IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(duration);

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, volume);
        }

        Collider[] hitZ = Physics.OverlapBox(transform.position,
            new Vector3(crossLength / 2, 1f, crossWidth / 2),
            Quaternion.identity);

        Collider[] hitX = Physics.OverlapBox(transform.position,
            new Vector3(crossWidth / 2, 1f, crossLength / 2),
            Quaternion.identity);

        List<GameObject> hitObjects = new List<GameObject>();
        ApplyDamage(hitZ, hitObjects);
        ApplyDamage(hitX, hitObjects);

        if (originCan != null)
            Destroy(originCan.gameObject);

        Destroy(gameObject);
    }

    private void ApplyDamage(Collider[] colliders, List<GameObject> hitObjects)
    {
        foreach (var col in colliders)
        {
            if (col.CompareTag("Player") && !hitObjects.Contains(col.gameObject))
            {
                AttackInfo info = new AttackInfo
                {
                    attacker = gameObject,
                    damage = damage,
                    attackDirection = (col.transform.position - transform.position).normalized
                };
                PlayerStatus.Instance.ApplyDamage(info);
                hitObjects.Add(col.gameObject);
            }
        }
    }
}
