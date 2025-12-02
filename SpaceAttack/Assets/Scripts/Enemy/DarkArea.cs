using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkArea : MonoBehaviour
{
    public float duration = 5f;
    public float damage = 1;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus player = other.GetComponent<PlayerStatus>();
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
    }
}
