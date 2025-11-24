using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAniRenderer : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ChangeRenderersAlapha(float a)
    {
        if (spriteRenderers == null || spriteRenderers.Length <= 0)
        {
            Debug.LogError($"{gameObject.name}에 스프라이트 랜더러 배열일 없음");
            return;
        }
        Color[] colors = new Color[spriteRenderers.Length];

        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            colors[i] = spriteRenderers[i].color;
            colors[i].a = a;
            spriteRenderers[i].color = colors[i];
        }
    }
}
