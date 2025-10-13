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
    private Transform player;

    public void SetSafeZones(List<GameObject> zones)
    {
        safeZones = zones;
    }

    void OnEnable()
    {
        player = PlayerStatus.Instance?.transform;
        StartCoroutine(ActivateDamage());
    }

    IEnumerator ActivateDamage()
    {
        yield return new WaitForSeconds(delayTime);

        active = true;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
            mr.material.color = new Color(1f, 0f, 0f, 1f);

        Destroy(gameObject, 0.5f);
    }

    void Update()
    {
        if (!active || hasDamaged || player == null) return;

        Vector3 playerPosXZ = new Vector3(player.position.x, 0f, player.position.z);
        Vector3 zonePosXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        float zoneRadius = Mathf.Max(transform.localScale.x, transform.localScale.z) / 2f;

        if (Vector3.Distance(playerPosXZ, zonePosXZ) > zoneRadius) return;

        foreach (var safe in safeZones)
        {
            if (safe == null) continue;
            Vector3 safePosXZ = new Vector3(safe.transform.position.x, 0f, safe.transform.position.z);
            float safeRadius = Mathf.Max(safe.transform.localScale.x, safe.transform.localScale.z) / 2f;

            if (Vector3.Distance(playerPosXZ, safePosXZ) <= safeRadius)
            {
                return;
            }
        }
        AttackInfo info = new AttackInfo
        {
            damage = damage,
            attacker = this.gameObject
        };
        PlayerStatus.Instance.ApplyDamage(info);
        hasDamaged = true;
    }

    private void OnDestroy()
    {
        foreach (var zone in safeZones)
        {
            if (zone != null)
                Destroy(zone);
        }
    }
}
