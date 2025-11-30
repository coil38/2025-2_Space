using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMatter : MonoBehaviour
{
    public GameObject areaEffectPrefab;
    public float speed = 10f;
    public float arcHeight = 0.5f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float travelTime = 1f;
    private float elapsed = 0f;

    public void Launch(Vector3 target)
    {
        startPos = transform.position;
        targetPos = target;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / travelTime);
        float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
        transform.position = Vector3.Lerp(startPos, targetPos, t) + Vector3.up * height;

        if (t >= 1f)
        {
            if (areaEffectPrefab != null)
            {
                Vector3 spawnPos = targetPos;

                if (Physics.Raycast(targetPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f, LayerMask.GetMask("Plan")))
                {
                    spawnPos.y = hit.point.y + 0.01f; // 바닥에서 0.05 위로 띄움
                }

                Instantiate(areaEffectPrefab, spawnPos, areaEffectPrefab.transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}
