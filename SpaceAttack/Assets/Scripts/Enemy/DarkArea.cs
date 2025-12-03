using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkArea : MonoBehaviour
{
    public float duration = 5f;
    public float damage = 1;
    public float damageInterval = 0.5f; 

    [SerializeField] private Vector3 areaSize = new Vector3(1f, 1f, 1f);
    [SerializeField] private LayerMask playerLayer;

    private void Start()
    {
        StartCoroutine(DamageLoop());
        Destroy(gameObject, duration);
    }

    private IEnumerator DamageLoop()
    {
        while (true)
        {
            Collider[] hits = Physics.OverlapBox(transform.position, areaSize / 2f, Quaternion.identity, playerLayer);
            foreach (var hit in hits)
            {
                PlayerStatus player = hit.GetComponent<PlayerStatus>();
                if (player != null)
                {
                    AttackInfo info = new AttackInfo
                    {
                        damage = damage,
                        attackDirection = Vector3.zero,
                        attacker = null
                    };
                    player.ApplyDamage(info);
                }
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
