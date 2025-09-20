using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTransparency : MonoBehaviour
{
    public Transform player;               // 플레이어
    public float transparentAlpha = 0.4f;  // 투명해질 알파
    public float fadeSpeed = 5f;           // 투명도 변경 속도

    private SpriteRenderer[] spriteRenderers;
    private float[] originalAlphas;

    void Start()
    {
        // 보스 아래 모든 SpriteRenderer 가져오기
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("BossTransparency: Boss에 SpriteRenderer가 없습니다!");
            return;
        }

        // 원래 알파값 저장
        originalAlphas = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalAlphas[i] = spriteRenderers[i].color.a;
        }
    }

    void Update()
    {
        if (player == null || spriteRenderers.Length == 0) return;

        bool playerBehindBoss = player.position.y < transform.position.y;
        float targetAlpha = playerBehindBoss ? transparentAlpha : 1f; // 1f 대신 원래 알파 사용 가능

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color c = spriteRenderers[i].color;
            float alphaTarget = playerBehindBoss ? transparentAlpha : originalAlphas[i];
            c.a = Mathf.Lerp(c.a, alphaTarget, Time.deltaTime * fadeSpeed);
            spriteRenderers[i].color = c;
        }
    }
}
