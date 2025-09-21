using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damage = 10;
    public float delayTime = 2f;   
    private bool active = false;
    private bool hasDamaged = false; 

    private List<GameObject> safeZones = new List<GameObject>();

    public void SetSafeZones(List<GameObject> zones)
    {
        safeZones = zones;
    }

    void OnEnable()
    {
        StartCoroutine(ActivateDamage());
    }

    IEnumerator ActivateDamage()
    {
        yield return new WaitForSeconds(delayTime);

        active = true;

        // 빨간 장판 색 진하게
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
            mr.material.color = new Color(1f, 0f, 0f, 1f); 

        Destroy(gameObject, 0.5f);
    }

    private void OnDestroy()
    {
        foreach (var zone in safeZones)
        {
            if (zone != null)
                Destroy(zone);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!active || hasDamaged) return; 

        if (other.CompareTag("Player"))
        {
            AttackInfo info = new AttackInfo
            {
                damage = damage,
                attacker = this.gameObject
            };
            PlayerStatus.Instance.ApplyDamage(info);

            hasDamaged = true; 
        }
    }
}
