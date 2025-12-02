using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [Header("오브젝트 체력")]
    public float maxHealth = 50f;   
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public void ApplyDamage(AttackInfo info)
    {
        currentHealth -= info.damage;

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
