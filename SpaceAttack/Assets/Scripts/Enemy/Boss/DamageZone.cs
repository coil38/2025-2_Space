using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damage = 10;
    public float delayTime = 2f;   // 장판 표시 후 공격까지 딜레이
    private bool active = false;

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
            mr.material.color = Color.red * 0.8f;

        // 👉 여기서는 안전장판 그대로 두고
        // 빨간 장판이 사라질 때 같이 지우도록 Destroy 순서를 변경
        Destroy(gameObject, 0.5f);
    }

    private void OnDestroy()
    {
        // 빨간 장판이 사라질 때 안전장판도 같이 삭제
        foreach (var zone in safeZones)
        {
            if (zone != null)
                Destroy(zone);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!active) return;

        if (other.CompareTag("Player"))
        {
            AttackInfo info = new AttackInfo
            {
                damage = damage,
                attacker = this.gameObject
            };
            PlayerStatus.Instance.ApplyDamage(info);
        }
    }
}
